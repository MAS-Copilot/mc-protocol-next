// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

namespace McProtocolNext;

/// <summary>
/// 三菱Plc命令
/// </summary>
internal class McCommand(McFrame mcFrame) {
    public McFrame FrameType { get; } = mcFrame;

    public byte[] Response { get; private set; } = [];

    #region 私有字段

    private readonly uint _serialNumber = 0x0001u;      // 序列号，从1开始，标识每次通信的唯一性    
    private readonly uint _networkNumber = 0x0000u;     // 网络号，默认值为0   
    private readonly uint _pcNumber = 0x00FFu;          // PC号，默认广播地址0xFF    
    private readonly uint _ioNumber = 0x03FFu;          // IO号，标识设备地址，默认值0x03FF    
    private readonly uint _channelNumber = 0x0000u;     // 通道号，默认值为0    
    private readonly uint _cpuTimer = 0x0010u;          // CPU定时器，默认值16（0x0010）    
    private int _resultCode;                            // 结果代码，用于存储通信状态

    #endregion

    /// <summary>
    /// 设置MC1E命令
    /// </summary>
    /// <param name="subheader">子头数据</param>
    /// <param name="data">命令数据</param>
    /// <returns>完整的命令字节数组</returns>
    public byte[] SetCommandMC1E(byte subheader, byte[] data) {
        var ret = new List<byte>(data.Length + 4) {
            subheader,
            (byte)_pcNumber,
            (byte)_cpuTimer,
            (byte)(_cpuTimer >> 8)
        };

        ret.AddRange(data);
        return [.. ret];
    }

    /// <summary>
    /// 设置 MC3E 命令
    /// </summary>
    /// <param name="mainCommand">主命令</param>
    /// <param name="subCommand">子命令</param>
    /// <param name="data">命令数据</param>
    /// <returns>完整的命令字节数组</returns>
    public byte[] SetCommandMC3E(uint mainCommand, uint subCommand, byte[] data) {
        var dataLength = (uint)(data.Length + 6);
        List<byte> ret = new(data.Length + 20);
        uint frame = 0x0050u;
        ret.Add((byte)frame);
        ret.Add((byte)(frame >> 8));

        ret.AddRange([
            (byte)_networkNumber,                           // 网络编号
            (byte)_pcNumber,                                // PC编号
            (byte)_ioNumber, (byte)(_ioNumber >> 8),        // IO编号
            (byte)_channelNumber,                           // 通道编号
            (byte)dataLength, (byte)(dataLength >> 8),      // 数据长度
            (byte)_cpuTimer, (byte)(_cpuTimer >> 8),        // CPU计时器
            (byte)mainCommand, (byte)(mainCommand >> 8),    // 主命令
            (byte)subCommand, (byte)(subCommand >> 8)       // 子命令
        ]);

        ret.AddRange(data);
        return [.. ret];
    }

    /// <summary>
    /// 设置 MC4E 命令
    /// </summary>
    /// <param name="mainCommand">主命令</param>
    /// <param name="subCommand">子命令</param>
    /// <param name="data">命令数据</param>
    /// <returns>完整的命令字节数组</returns>
    public byte[] SetCommandMC4E(uint mainCommand, uint subCommand, byte[] data) {
        uint dataLength = (uint)(data.Length + 6);
        List<byte> ret = new(data.Length + 20);
        uint frame = 0x0054u;
        ret.Add((byte)frame);
        ret.Add((byte)(frame >> 8));

        ret.AddRange([
            (byte)_serialNumber, (byte)(_serialNumber >> 8), 0x00, 0x00,                    // 序列号和空数据
            (byte)_networkNumber, (byte)_pcNumber, (byte)_ioNumber, (byte)(_ioNumber >> 8), // 网络和PC、IO编号
            (byte)_channelNumber, (byte)dataLength, (byte)(dataLength >> 8),                // 通道编号和数据长度
            (byte)_cpuTimer, (byte)(_cpuTimer >> 8),                                        // CPU计时器
            (byte)mainCommand, (byte)(mainCommand >> 8),                                    // 主命令
            (byte)subCommand, (byte)(subCommand >> 8)                                       // 子命令
        ]);

        ret.AddRange(data);
        return [.. ret];
    }

    /// <summary>
    /// 设置响应数据
    /// </summary>
    /// <param name="response">响应字节数据</param>
    /// <returns>结果码</returns>
    public int SetResponse(byte[] response) {
        int min;
        switch (FrameType) {
            case McFrame.MC1E:
                min = 2;
                if (min > response.Length) {
                    break;
                }

                _resultCode = response[min - 2];
                Response = new byte[response.Length - 2];

                if (min + Response.Length > response.Length) {
                    throw new ArgumentException($"Response array length ({response.Length}) is insufficient for copying {Response.Length} bytes starting at offset {min}.");
                }

                Buffer.BlockCopy(response, min, Response, 0, Response.Length);

                break;
            case McFrame.MC3E:
            case McFrame.MC4E:
                min = FrameType == McFrame.MC3E ? 11 : 15;
                if (min > response.Length) {
                    break;
                }

                var btCount = new[] { response[min - 4], response[min - 3] };
                var btCode = new[] { response[min - 2], response[min - 1] };
                int rsCount = BitConverter.ToUInt16(btCount, 0);
                _resultCode = BitConverter.ToUInt16(btCode, 0);
                Response = new byte[rsCount - 2];
                if (min + Response.Length > response.Length) {
                    throw new ArgumentException($"Response array length ({response.Length}) is insufficient for copying {Response.Length} bytes starting at offset {min}.");
                }

                Buffer.BlockCopy(response, min, Response, 0, Response.Length);

                break;
            default:
                throw new Exception("Frame type not supported.");
        }

        return _resultCode;
    }
}
