// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using McProtocolNext;
using McProtocolNextDemo.Abstractions;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace McProtocolNextDemo.Models;

/// <summary>
/// MC 通讯参数配置
/// </summary>
public class McProtocolConfig : ViewModelBase, IMcCommunicationConfig {
    /// <summary>
    /// 创建当前 <see cref="McProtocolConfig"/> 实例的深表副本
    /// </summary>
    /// <returns>这个实例的一个新的深表副本</returns>
    public object Clone() {
        return Clone<McProtocolConfig>();
    }

    /// <summary>
    /// 以指定的类型创建当前实例的深表副本
    /// </summary>
    /// <typeparam name="T">要克隆的对象的类型，该类型必须实现 <see cref="IMcCommunicationConfig"/></typeparam>
    /// <returns>一个类型为 T 的新对象，这个对象是这个实例的一个深表副本</returns>
    public T Clone<T>() where T : IMcCommunicationConfig {
        var json = JsonConvert.SerializeObject(this);
        return JsonConvert.DeserializeObject<T>(json) ?? throw new InvalidOperationException("Clone failed.");
    }

    /// <inheritdoc/>
    public string ProtocolName { get; set; } = "MC";

    private string _ip = string.Empty;
    /// <inheritdoc/>
    public string Ip {
        get => _ip;
        set => SetField(ref _ip, value, nameof(Ip));
    }

    private string _protocolFrame = string.Empty;
    /// <inheritdoc/>
    public string ProtocolFrame {
        get => _protocolFrame;
        set => SetField(ref _protocolFrame, value, nameof(ProtocolFrame));
    }

    private short _port;
    /// <inheritdoc/>
    public short Port {
        get => _port;
        set => SetField(ref _port, value, nameof(Port));
    }

    private short _heartbeatInterval;
    /// <inheritdoc/>
    public short HeartbeatInterval {
        get => _heartbeatInterval;
        set => SetField(ref _heartbeatInterval, value, nameof(HeartbeatInterval));
    }

    private string _bitSendRegister = string.Empty;
    /// <inheritdoc/>
    public string BitSendRegister {
        get => _bitSendRegister;
        set => SetField(ref _bitSendRegister, value, nameof(BitSendRegister));
    }

    private int _bitSendStartAddress;
    /// <inheritdoc/>
    public int BitSendStartAddress {
        get => _bitSendStartAddress;
        set => SetField(ref _bitSendStartAddress, value, nameof(BitSendStartAddress));
    }

    private string _bitReceiveRegister = string.Empty;
    /// <inheritdoc/>
    public string BitReceiveRegister {
        get => _bitReceiveRegister;
        set => SetField(ref _bitReceiveRegister, value, nameof(BitReceiveRegister));
    }

    private int _bitReceiveStartAddress;
    /// <inheritdoc/>
    public int BitReceiveStartAddress {
        get => _bitReceiveStartAddress;
        set => SetField(ref _bitReceiveStartAddress, value, nameof(BitReceiveStartAddress));
    }

    private int _bitAddressRange;
    /// <inheritdoc/>
    public int BitAddressRange {
        get => _bitAddressRange;
        set => SetField(ref _bitAddressRange, value, nameof(BitAddressRange));
    }

    private string _wordSendRegister = string.Empty;
    /// <inheritdoc/>
    public string WordSendRegister {
        get => _wordSendRegister;
        set => SetField(ref _wordSendRegister, value, nameof(WordSendRegister));
    }

    private int _wordSendStartAddress;
    /// <inheritdoc/>
    public int WordSendStartAddress {
        get => _wordSendStartAddress;
        set => SetField(ref _wordSendStartAddress, value, nameof(WordSendStartAddress));
    }

    private string _wordReceiveRegister = string.Empty;
    /// <inheritdoc/>
    public string WordReceiveRegister {
        get => _wordReceiveRegister;
        set => SetField(ref _wordReceiveRegister, value, nameof(WordReceiveRegister));
    }

    private int _wordReceiveStartAddress;
    /// <inheritdoc/>
    public int WordReceiveStartAddress {
        get => _wordReceiveStartAddress;
        set => SetField(ref _wordReceiveStartAddress, value, nameof(WordReceiveStartAddress));
    }

    private int _wordAddressRange;
    /// <inheritdoc/>
    public int WordAddressRange {
        get => _wordAddressRange;
        set => SetField(ref _wordAddressRange, value, nameof(WordAddressRange));
    }

    private short _runSleepTime;
    /// <inheritdoc/>
    public short RunSleepTime {
        get => _runSleepTime;
        set => SetField(ref _runSleepTime, value, nameof(RunSleepTime));
    }

    private string _executionMode = string.Empty;
    /// <inheritdoc/>
    public string ExecutionMode {
        get => _executionMode;
        set => SetField(ref _executionMode, value, nameof(ExecutionMode));
    }

    private short _maxRetries;
    /// <inheritdoc/>
    public short MaxRetries {
        get => _maxRetries;
        set => SetField(ref _maxRetries, value, nameof(MaxRetries));
    }

    private short _readTimeout;
    /// <inheritdoc/>
    public short ReadTimeout {
        get => _readTimeout;
        set => SetField(ref _readTimeout, value, nameof(ReadTimeout));
    }

    private short _writeTimeout;
    /// <inheritdoc/>
    public short WriteTimeout {
        get => _writeTimeout;
        set => SetField(ref _writeTimeout, value, nameof(WriteTimeout));
    }

    private short _executionTimeout;
    /// <inheritdoc/>
    public short ExecutionTimeout {
        get => _executionTimeout;
        set => SetField(ref _executionTimeout, value, nameof(ExecutionTimeout));
    }

    private short _taskExecutionAdr;
    /// <inheritdoc/>
    public short TaskExecutionAdr {
        get => _taskExecutionAdr;
        set => SetField(ref _taskExecutionAdr, value);
    }

    private short _taskCompletedAdr;
    /// <inheritdoc/>
    public short TaskCompletedAdr {
        get => _taskCompletedAdr;
        set => SetField(ref _taskCompletedAdr, value);
    }

    private short _readDataAdr;
    /// <inheritdoc/>
    public short ReadDataAdr {
        get => _readDataAdr;
        set => SetField(ref _readDataAdr, value);
    }

    private short _writeDataAdr;
    /// <inheritdoc/>
    public short WriteDataAdr {
        get => _writeDataAdr;
        set => SetField(ref _writeDataAdr, value);
    }

    /// <summary>
    /// 获取协议帧选项
    /// </summary>
    [JsonIgnore]
    public ObservableCollection<string> ProtocolFrameOptions { get; } = [
        "MC1E", "MC3E", "MC4E"];

    /// <summary>
    /// 获取发送位寄存器选项
    /// </summary>
    [JsonIgnore]
    public ObservableCollection<string> SendBitRegisterOptions { get; } = [
        "M", "SM", "L", "F", "X", "Y", "B", "S"];

    /// <summary>
    /// 获取发送字寄存器选项
    /// </summary>
    [JsonIgnore]
    public ObservableCollection<string> SendWordRegisterOptions { get; } = [
        "D", "W", "R", "ZR", "SD", "SW", "Z"];

    /// <summary>
    /// 获取接收位寄存器选项
    /// </summary>
    [JsonIgnore]
    public ObservableCollection<string> ReceiveBitRegisterOptions { get; } = [
        "M", "SM", "L", "F", "X", "Y", "B", "S"];

    /// <summary>
    /// 获取接收字寄存器选项
    /// </summary>
    [JsonIgnore]
    public ObservableCollection<string> ReceiveWordRegisterOptions { get; } = [
        "D", "W", "R", "ZR", "SD", "SW", "Z"];

    /// <summary>
    /// 获取模式集合
    /// </summary>
    [JsonIgnore]
    public ObservableCollection<string> ExecutionModes { get; } = ["Sequential", "Concurrent"];
}
