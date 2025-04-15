// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

namespace McProtocolNext;

/// <summary>
/// PLC 连接错误异常
/// </summary>
public class PlcConnectionException : Exception {
    /// <summary>
    /// 初始化 <see cref="PlcConnectionException"/> 新实例
    /// </summary>
    /// <param name="message">异常消息</param>
    public PlcConnectionException(string message) : base(message) { }

    /// <summary>
    /// 初始化 <see cref="PlcConnectionException"/> 新实例
    /// </summary>
    /// <param name="message">异常消息</param>
    /// <param name="innerException">内部异常</param>
    public PlcConnectionException(string message, Exception innerException) : base(message, innerException) { }
}
