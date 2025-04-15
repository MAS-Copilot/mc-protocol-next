// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

namespace McProtocolNext;

/// <summary>
/// MC 协议参数配置接口
/// </summary>
public interface IMcCommunicationConfig : ICloneable {
    /// <summary>
    /// 获取或设置 Ip 地址
    /// </summary>
    public string Ip { get; set; }

    /// <summary>
    /// 获取或设置心跳发送间隔(s)
    /// </summary>
    public short HeartbeatInterval { get; set; }

    /// <summary>
    /// 获取或设置运行休眠时间(ms)
    /// </summary>
    public short RunSleepTime { get; set; }

    /// <summary>
    /// 获取或设置协议帧
    /// </summary>
    public string ProtocolFrame { get; set; }

    /// <summary>
    /// 获取或设置端口
    /// </summary>
    public short Port { get; set; }

    /// <summary>
    /// 获取或设置发送位信号使用的寄存器类型
    /// </summary>
    public string BitSendRegister { get; set; }

    /// <summary>
    /// 获取或设置发送位信号寄存器的起始地址
    /// </summary>
    public int BitSendStartAddress { get; set; }

    /// <summary>
    /// 获取或设置接收位信号使用的寄存器类型
    /// </summary>
    public string BitReceiveRegister { get; set; }

    /// <summary>
    /// 获取或设置接收位信号寄存器的起始地址
    /// </summary>
    public int BitReceiveStartAddress { get; set; }

    /// <summary>
    /// 获取或设置位地址范围
    /// </summary>
    public int BitAddressRange { get; set; }

    /// <summary>
    /// 获取或设置发送字信号使用的寄存器类型
    /// </summary>
    public string WordSendRegister { get; set; }

    /// <summary>
    /// 获取或设置发送字信号寄存器的起始地址
    /// </summary>
    public int WordSendStartAddress { get; set; }

    /// <summary>
    /// 获取或设置接收字信号使用的寄存器类型
    /// </summary>
    public string WordReceiveRegister { get; set; }

    /// <summary>
    /// 获取或设置接收字信号寄存器的起始地址
    /// </summary>
    public int WordReceiveStartAddress { get; set; }

    /// <summary>
    /// 获取或设置字地址范围
    /// </summary>
    public int WordAddressRange { get; set; }

    /// <summary>
    /// 获取或设置读取超时时间(ms)
    /// </summary>
    public short ReadTimeout { get; set; }

    /// <summary>
    /// 获取或设置写入超时时间（ms）
    /// </summary>
    public short WriteTimeout { get; set; }

    /// <summary>
    /// 获取或设置执行超时时间（ms）
    /// </summary>
    public short ExecutionTimeout { get; set; }

    /// <summary>
    /// 获取或设置任务执行起始地址
    /// </summary>
    public short TaskExecutionAdr { get; set; }

    /// <summary>
    /// 获取或设置任务完成起始地址
    /// </summary>
    public short TaskCompletedAdr { get; set; }

    /// <summary>
    /// 获取或设置数据读取起始地址
    /// </summary>
    public short ReadDataAdr { get; set; }

    /// <summary>
    /// 获取或设置数据写入起始地址
    /// </summary>
    public short WriteDataAdr { get; set; }

    /// <summary>
    /// 获取或设置连接失败时的重试次数上限
    /// </summary>
    public short MaxRetries { get; set; }

    /// <summary>
    /// 获取或设置执行模式。Sequential 顺序执行 Concurrent 并行执行
    /// </summary>
    public string ExecutionMode { get; set; }

    /// <summary>
    /// 泛型克隆方法，返回指定类型的克隆实例
    /// </summary>
    /// <typeparam name="T">克隆实例的类型</typeparam>
    /// <returns>克隆后的实例</returns>
    T Clone<T>() where T : IMcCommunicationConfig;
}
