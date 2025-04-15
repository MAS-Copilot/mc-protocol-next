// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

namespace McProtocolNext;

/// <summary>
/// 与三菱 MC 协议的 PLC 设备进行通信的接口
/// </summary>
public interface IMcProtocol : IDisposable {
    /// <summary>
    /// 获取通讯配置
    /// </summary>
    IMcCommunicationConfig Configuration { get; }

    /// <summary>
    /// 异步从PLC读取字数据
    /// </summary>
    /// <param name="deviceType">指定设备类型</param>
    /// <param name="startAddress">起始地址</param>
    /// <param name="length">要读取的数据长度</param>
    /// <param name="cts">取消令牌</param>
    /// <returns>异步操作任务结果，包含读取值的整数数组</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="PlcReadErrorException"></exception>
    Task<short[]> ReadWordsAsync(string deviceType, int startAddress, int length, CancellationToken cts = default);

    /// <summary>
    /// 异步从PLC读取字数据
    /// </summary>
    /// <typeparam name="T">数据类型： `short`、`int`、`float`、`double` </typeparam>
    /// <param name="deviceType">指定设备类型</param>
    /// <param name="startAddress">起始地址</param>
    /// <param name="length">要读取的数据长度</param>
    /// <param name="cts">取消令牌</param>
    /// <returns>异步操作任务结果，包含读取值的泛型数组</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="PlcReadErrorException"></exception>
    Task<T[]> ReadWordsAsync<T>(string deviceType, int startAddress, int length, CancellationToken cts = default);

    /// <summary>
    /// 异步从PLC读取位数据
    /// </summary>
    /// <param name="deviceType">指定设备类型</param>
    /// <param name="startAddress">起始地址</param>
    /// <param name="length">要读取的位的数量</param>
    /// <param name="cts">取消令牌</param>
    /// <returns>异步操作任务结果，包含读取位状态的布尔值数组</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="PlcReadErrorException"></exception>
    Task<bool[]> ReadBitsAsync(string deviceType, int startAddress, int length, CancellationToken cts = default);

    /// <summary>
    /// 异步从PLC读取结构体数据
    /// </summary>
    /// <param name="structType">结构体类型</param>
    /// <param name="startAddress">起始地址</param>
    /// <param name="cts">取消令牌</param>
    /// <returns>异步操作任务结果，返回结构体数据</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="PlcReadErrorException"></exception>
    Task<object?> ReadStructAsync(Type structType, int startAddress, CancellationToken cts = default);

    /// <summary>
    /// 异步从PLC读取结构体数据
    /// </summary>
    /// <typeparam name="T">结构体类型</typeparam>
    /// <param name="startAddress">起始地址</param>
    /// <param name="cts">取消令牌</param>
    /// <returns>异步操作任务结果，返回结构体数据</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="PlcReadErrorException"></exception>
    Task<T?> ReadStructAsync<T>(int startAddress, CancellationToken cts = default) where T : struct;

    /// <summary>
    /// 异步写入字数据到PLC
    /// </summary>
    /// <param name="deviceType">指定设备类型</param>
    /// <param name="startAddress">起始地址</param>
    /// <param name="values">要写入的整数数组</param>
    /// <param name="cts">取消令牌</param>
    /// <returns>异步操作任务结果</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="PlcWriteErrorException"></exception>
    Task WriteWordsAsync(string deviceType, int startAddress, short[] values, CancellationToken cts = default);

    /// <summary>
    /// 异步写入字数据到PLC
    /// </summary>
    /// <typeparam name="T">数据类型： `short`、`int`、`float`、`double` </typeparam>
    /// <param name="deviceType">指定设备类型</param>
    /// <param name="startAddress">起始地址</param>
    /// <param name="values">要写入的数据数组</param>
    /// <param name="cts">取消令牌</param>
    /// <returns>异步操作任务结果</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="PlcWriteErrorException"></exception>
    Task WriteWordsAsync<T>(string deviceType, int startAddress, T[] values, CancellationToken cts = default);

    /// <summary>
    /// 异步写入位数据到PLC
    /// </summary>
    /// <param name="deviceType">指定设备类型</param>
    /// <param name="startAddress">起始地址</param>
    /// <param name="values">要写入的布尔值数组，表示位状态</param>
    /// <param name="cts">取消令牌</param>
    /// <returns>异步操作任务结果</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="PlcWriteErrorException"></exception>
    Task WriteBitsAsync(string deviceType, int startAddress, bool[] values, CancellationToken cts = default);

    /// <summary>
    /// 异步写入结构体数据到PLC
    /// </summary>
    /// <param name="structValue">结构体值</param>
    /// <param name="startAddress">起始地址</param>
    /// <param name="cts">取消令牌</param>
    /// <returns>异步操作任务结果</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="PlcWriteErrorException"></exception>
    Task WriteStructAsync(object structValue, int startAddress, CancellationToken cts = default);

    /// <summary>
    ///  检查PLC连接状态
    /// </summary>
    /// <returns>连接活跃返回true，否则返回false</returns>
    bool CheckPlcConnection();

    /// <summary>
    /// 异步连接到PLC设备
    /// </summary>
    /// <param name="cts">取消令牌</param>
    /// <returns>异步操作任务结果</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="PlcConnectionException"></exception>
    Task ConnectToPlcAsync(CancellationToken cts = default);

    /// <summary>
    /// 异步测试PLC连接，成功后自动关闭
    /// </summary>
    /// <remarks>
    /// 如果手动取消，自行处理 OperationCanceledException 异常
    /// </remarks>
    /// <param name="cts">取消令牌</param>
    /// <returns>异步操作任务结果</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="PlcConnectionException"></exception>
    Task TestPlcConnectionAsync(CancellationToken cts = default);

    /// <summary>
    /// 异步尝试重连到PLC
    /// </summary>
    /// <remarks>
    /// 在中途断线时调用此方法将尝试重连接
    /// </remarks>
    /// <param name="cts">取消令牌</param>
    /// <returns>成功连接则返回 true，否则返回 false</returns>
    Task<bool> TryReconnectToPlcAsync(CancellationToken cts = default);

    /// <summary>
    /// 异步尝试重连到PLC
    /// </summary>
    /// <remarks>
    /// 在中途断线时调用此方法将尝试重连接
    /// </remarks>
    /// <param name="maxAttempts">最多尝试次数</param>
    /// <param name="awaitTime">等待时间（毫秒）</param>
    /// <param name="cts">取消令牌</param>
    /// <returns>成功连接则返回 true，否则返回 false</returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<bool> TryReconnectToPlcAsync(int maxAttempts, short awaitTime = 1000, CancellationToken cts = default);

    /// <summary>
    /// 关闭PLC连接
    /// </summary>
    void DisconnectFromPlc();
}
