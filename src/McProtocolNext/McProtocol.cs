// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using System.Buffers;
using System.IO;
using System.Net.Sockets;

namespace McProtocolNext;

internal class McProtocol(IMcCommunicationConfig config) : IMcProtocol {
    private readonly IMcCommunicationConfig _mcConfig = config;

    #region 私有字段

    private bool _disposed;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private readonly McFrame _mcFrame = (McFrame)Enum.Parse(typeof(McFrame), config.ProtocolFrame);
    private readonly SemaphoreSlim _streamLock = new(1, 1);

    #endregion

    public IMcCommunicationConfig Configuration { get; } = config;

    public bool CheckPlcConnection() {
        return _client != null && _client.Connected;
    }

    public async Task ConnectToPlcAsync(CancellationToken cts = default) {
        try {
            await OpenAsync(cts).ConfigureAwait(false);
        } catch (OperationCanceledException) {
            throw;
        } catch (Exception ex) {
            throw new PlcConnectionException($"PLC connection failed: {ex.Message}", ex);
        }
    }

    public void DisconnectFromPlc() {
        DoDisconnect();
    }

    public void Dispose() {
        if (_disposed) {
            return;
        }

        DoDisconnect();
        _disposed = true;
    }

    public async Task<bool[]> ReadBitsAsync(string deviceType, int startAddress, int length, CancellationToken cts = default) {
        try {
            int byteLength = (length / 16 + ((length % 16 > 0) ? 1 : 0)) * 2;
            var bytes = await ReadDeviceBlockAsync(
                MitsubishiHelper.ParsePlcDeviceType(deviceType),
                startAddress,
                byteLength,
                cts
            ).ConfigureAwait(false);

            if (bytes.Length < byteLength) {
                throw new PlcReadErrorException($"Not enough data read from PLC. Expected at least {byteLength} bytes, but got {bytes.Length} bytes.");
            }

            cts.ThrowIfCancellationRequested();
            bool[] bits = new bool[length];
            for (int i = 0; i < length; i++) {
                bits[i] = (bytes[i / 8] >> (i % 8) & 1) == 1;
            }

            return bits;
        } catch (OperationCanceledException) {
            throw;
        } catch (Exception ex) {
            throw new PlcReadErrorException($"Error reading bits from PLC. Device: {deviceType}, StartAddress: {startAddress}, Length: {length}.", ex);
        }
    }

    public async Task<object?> ReadStructAsync(Type structType, int startAddress, CancellationToken cts = default) {
        try {
            int numBytes = MitsubishiHelper.GetStructSize(structType);
            int numRegisters = (int)Math.Ceiling(numBytes / 2.0);

            byte[] bytes = await ReadDeviceBlockAsync(
                PlcDeviceType.D,
                startAddress,
                numRegisters,
                cts
            ).ConfigureAwait(false);

            if (bytes.Length != numBytes) {
                return null;
            }

            return MitsubishiHelper.BytesToStruct(bytes, structType);
        } catch (OperationCanceledException) {
            throw;
        } catch (Exception ex) {
            throw new PlcWriteErrorException(
                $"Failed to read structure from PLC. structType: {structType.FullName} StartAddress: {startAddress}.", ex);
        }
    }

    public async Task<T?> ReadStructAsync<T>(int startAddress, CancellationToken cts = default) where T : struct {
        try {
            int numBytes = MitsubishiHelper.GetStructSize(typeof(T));
            int numRegisters = (int)Math.Ceiling(numBytes / 2.0);

            byte[] bytes = await ReadDeviceBlockAsync(
                PlcDeviceType.D,
                startAddress,
                numRegisters,
                cts
            ).ConfigureAwait(false);

            if (bytes.Length != numBytes) {
                return null;
            }

            return MitsubishiHelper.BytesToStruct<T>(bytes);
        } catch (OperationCanceledException) {
            throw;
        } catch (Exception ex) {
            throw new PlcWriteErrorException(
                $"Failed to read structure from PLC. Type: {typeof(T).FullName} StartAddress: {startAddress}.", ex);
        }
    }

    public async Task<short[]> ReadWordsAsync(string deviceType, int startAddress, int length, CancellationToken cts = default) {
        try {
            int byteLength = length * 2;
            var bytes = await ReadDeviceBlockAsync(
                MitsubishiHelper.ParsePlcDeviceType(deviceType),
                startAddress,
                byteLength,
                cts
            ).ConfigureAwait(false);

            if (bytes.Length < byteLength) {
                throw new PlcReadErrorException($"Not enough data read from PLC. Expected at least {byteLength} bytes, but got {bytes.Length} bytes.");
            }

            cts.ThrowIfCancellationRequested();
            var words = new short[length];
            for (int i = 0; i < length; i++) {
                words[i] = (short)(bytes[i * 2] | (bytes[i * 2 + 1]) << 8);
            }

            return words;
        } catch (OperationCanceledException) {
            throw;
        } catch (Exception ex) {
            throw new PlcReadErrorException(
                $"Error reading words from PLC. Device: {deviceType}, StartAddress: {startAddress}, Length: {length}", ex);
        }
    }

    public async Task<T[]> ReadWordsAsync<T>(string deviceType, int startAddress, int length, CancellationToken cts = default) {
        try {
            int byteLength = length * MitsubishiHelper.GetTypeByteLength(typeof(T));
            var bytes = await ReadDeviceBlockAsync(
                MitsubishiHelper.ParsePlcDeviceType(deviceType),
                startAddress,
                byteLength,
                cts
            ).ConfigureAwait(false);

            if (bytes.Length < byteLength) {
                throw new PlcReadErrorException($"Not enough data read from PLC. Expected at least {byteLength} bytes, but got {bytes.Length} bytes.");
            }

            T[] result = new T[length];
            for (int i = 0; i < length; i++) {
                byte[] dataBytes = new byte[MitsubishiHelper.GetTypeByteLength(typeof(T))];
                Array.Copy(bytes, i * dataBytes.Length, dataBytes, 0, dataBytes.Length);
                result[i] = (T)MitsubishiHelper.GetValueFromBytes(dataBytes, typeof(T));
            }

            return result;
        } catch (OperationCanceledException) {
            throw;
        } catch (Exception ex) {
            throw new PlcReadErrorException(
                $"Error reading data from PLC. Device: {deviceType}, StartAddress: {startAddress}, Length: {length}.", ex);
        }
    }

    public async Task TestPlcConnectionAsync(CancellationToken cts = default) {
        try {
            await OpenAsync(cts).ConfigureAwait(false);
        } catch (OperationCanceledException) {
            throw;
        } catch (Exception ex) {
            throw new PlcConnectionException($"Test PLC connection failed: {ex.Message}", ex);
        }

        DisconnectFromPlc();
    }

    public async Task<bool> TryReconnectToPlcAsync(int maxAttempts, short awaitTime = 1000, CancellationToken cts = default) {
        int attempt = 0;
        while (attempt < maxAttempts) {
            try {
                await OpenAsync(cts).ConfigureAwait(false);
                return true;
            } catch (OperationCanceledException) {
                throw;
            } catch {
                await Task.Delay(awaitTime, cts);
            }

            attempt++;
        }

        return false;
    }

    public async Task<bool> TryReconnectToPlcAsync(CancellationToken cts = default) {
        try {
            await OpenAsync(cts).ConfigureAwait(false);
            return true;
        } catch {
            return false;
        }
    }

    public async Task WriteBitsAsync(string deviceType, int startAddress, bool[] values, CancellationToken cts = default) {
        try {
            int numBits = values.Length;
            int byteLength = (numBits / 16 + ((numBits % 16 > 0) ? 1 : 0)) * 2;
            byte[] bytes = new byte[byteLength];

            for (int i = 0; i < numBits; i++) {
                int wordIndex = i / 16;
                int bitInWord = i % 16;
                int byteIndex = wordIndex * 2 + (bitInWord / 8);
                int bitInByte = bitInWord % 8;

                if (values[i]) {
                    bytes[byteIndex] |= (byte)(1 << bitInByte);
                } else {
                    bytes[byteIndex] &= (byte)~(1 << bitInByte);
                }
            }

            _ = await WriteDeviceBlockAsync(
                MitsubishiHelper.ParsePlcDeviceType(deviceType),
                startAddress,
                byteLength / 2,
                bytes,
                cts
            ).ConfigureAwait(false);
        } catch (OperationCanceledException) {
            throw;
        } catch (Exception ex) {
            throw new PlcWriteErrorException(
                $"Error writing bits to PLC. Device: {deviceType}, StartAddress: {startAddress}, Length: {values.Length}.", ex);
        }
    }

    public async Task WriteStructAsync(object structValue, int startAddress, CancellationToken cts = default) {
        try {
            byte[] bytes = MitsubishiHelper.StructToBytes(structValue);
            int numRegisters = (int)Math.Ceiling(bytes.Length / 2.0);

            _ = await WriteDeviceBlockAsync(
                PlcDeviceType.D,
                startAddress,
                numRegisters,
                bytes,
                cts
            ).ConfigureAwait(false);
        } catch (OperationCanceledException) {
            throw;
        } catch (Exception ex) {
            throw new PlcWriteErrorException(
                $"Failed to write structure to PLC. structType: {structValue.GetType().FullName} StartAddress: {startAddress}.", ex);
        }
    }

    public async Task WriteWordsAsync(string deviceType, int startAddress, short[] values, CancellationToken cts = default) {
        try {
            int byteLength = values.Length * 2;
            byte[] bytes = new byte[byteLength];

            for (int i = 0; i < values.Length; i++) {
                byte[] shortBytes = BitConverter.GetBytes(values[i]);
                Array.Copy(shortBytes, 0, bytes, i * 2, 2);
            }

            _ = await WriteDeviceBlockAsync(
                MitsubishiHelper.ParsePlcDeviceType(deviceType),
                startAddress,
                byteLength / 2,
                bytes,
                cts
            ).ConfigureAwait(false);
        } catch (OperationCanceledException) {
            throw;
        } catch (Exception ex) {
            throw new PlcWriteErrorException(
                $"Error writing words to PLC. Device: {deviceType}, StartAddress: {startAddress}, Length: {values.Length}.", ex);
        }
    }

    public async Task WriteWordsAsync<T>(string deviceType, int startAddress, T[] values, CancellationToken cts = default) {
        try {
            var length = MitsubishiHelper.GetTypeByteLength(typeof(T));
            var byteLength = values.Length * length;
            var bytes = new byte[byteLength];

            for (int i = 0; i < values.Length; i++) {
                var valueBytes = MitsubishiHelper.GetBytes(values[i]!);
                Array.Copy(valueBytes, 0, bytes, i * length, length);
            }

            _ = await WriteDeviceBlockAsync(
                MitsubishiHelper.ParsePlcDeviceType(deviceType),
                startAddress,
                byteLength / 2,
                bytes,
                cts
            ).ConfigureAwait(false);
        } catch (OperationCanceledException) {
            throw;
        } catch (Exception ex) {
            throw new PlcWriteErrorException(
                $"Error writing data to PLC. Device: {deviceType}, StartAddress: {startAddress}, Length: {values.Length}.", ex);
        }
    }

    #region 私有方法

    private async Task OpenAsync(CancellationToken cts = default) {
        if (_client != null && _client.Client != null && _client.Client.Connected) {
            return;
        }

        _client?.Dispose();
        _client = new TcpClient();

        List<byte> list = new(12);
        list.AddRange(BitConverter.GetBytes(1u));
        list.AddRange(BitConverter.GetBytes(45000u));
        list.AddRange(BitConverter.GetBytes(5000u));

        try {
            _ = _client.Client.IOControl(IOControlCode.KeepAliveValues, [.. list], null);
            await _client.ConnectAsync(_mcConfig.Ip, _mcConfig.Port, cts).ConfigureAwait(false);
            _stream = _client.GetStream();
        } catch (OperationCanceledException) {
            _client.Dispose();
            _client = null;
            throw;
        }
    }

    private async Task<int> WriteDeviceBlockAsync(PlcDeviceType type, int address, int size, byte[] data, CancellationToken cts = default) {
        var mcCommand = new McCommand(_mcFrame);
        var (sdCommand, length) = _mcFrame switch {
            McFrame.MC3E => CommandHelper.BuildMc3E(type, address, size, mcCommand, data),
            McFrame.MC4E => CommandHelper.BuildMc4E(type, address, size, mcCommand, data),
            McFrame.MC1E => CommandHelper.BuildMc1E(address, size, mcCommand, data),
            _ => throw new Exception("Message frame not supported"),
        };

        byte[] rtResponse = await TryExecutionAsync(
            sdCommand, length, cts: cts
        ).ConfigureAwait(false);

        return mcCommand.SetResponse(rtResponse);
    }

    private async Task<byte[]> ReadDeviceBlockAsync(PlcDeviceType type, int address, int size, CancellationToken cts = default) {
        var mcCommand = new McCommand(_mcFrame);
        var (sdCommand, length) = _mcFrame switch {
            McFrame.MC3E => CommandHelper.BuildMc3E(type, address, size, mcCommand),
            McFrame.MC4E => CommandHelper.BuildMc4E(type, address, size, mcCommand),
            McFrame.MC1E => CommandHelper.BuildMc1E(address, size, mcCommand),
            _ => throw new Exception("Message frame not supported"),
        };

        byte[] rtResponse = await TryExecutionAsync(
            sdCommand, length, cts: cts
        ).ConfigureAwait(false);

        _ = mcCommand.SetResponse(rtResponse);
        byte[] rtData = mcCommand.Response;

        return rtData;
    }

    private async Task<byte[]> TryExecutionAsync(
        byte[] command,
        int minlength,
        int maxRetries = 10,
        TimeSpan? retryDelay = null,
        CancellationToken cts = default) {

        byte[] response;
        int retryCount = 0;

        do {
            response = await ExecuteAsync(command, cts).ConfigureAwait(false);
            retryCount++;

            if (MitsubishiHelper.IsIncorrectResponse(_mcFrame, response, minlength)) {
                if (retryCount >= maxRetries) {
                    throw new PlcReadErrorException($"Could not get the correct value from the PLC");
                }

                if (retryDelay.HasValue) {
                    await Task.Delay(retryDelay.Value, cts).ConfigureAwait(false);
                }
            }
        } while (MitsubishiHelper.IsIncorrectResponse(_mcFrame, response, minlength));

        return response;
    }

    private async Task<byte[]> ExecuteAsync(byte[] command, CancellationToken cts = default) {
        if (_stream == null) {
            throw new InvalidOperationException("The network flow is uninitialized");
        }

        await _streamLock.WaitAsync(cts).ConfigureAwait(false);
        try {
            await _stream.WriteAsync(command, cts).ConfigureAwait(false);
            await _stream.FlushAsync(cts).ConfigureAwait(false);

            using MemoryStream memoryStream = new();
            byte[] buffer = ArrayPool<byte>.Shared.Rent(256);

            try {
                int bytesRead;
                while ((bytesRead = await _stream.ReadAsync(buffer, cts).ConfigureAwait(false)) > 0) {
                    memoryStream.Write(buffer, 0, bytesRead);
                    if (bytesRead < buffer.Length) {
                        break;
                    }
                }

                if (memoryStream.Length == 0) {
                    throw new Exception("The connection has been disconnected");
                }

                return memoryStream.ToArray();
            } finally {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        } finally {
            _ = _streamLock.Release();
        }
    }

    private void DoDisconnect() {
        _stream?.Close();
        _client?.Close();
        _client = null;
        _stream = null;
    }

    #endregion
}
