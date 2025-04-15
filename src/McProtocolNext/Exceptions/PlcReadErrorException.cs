// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

namespace McProtocolNext;

/// <summary>
/// PLC 读取数据错误异常
/// </summary>
public class PlcReadErrorException : Exception {
    /// <summary>
    /// 初始化 <see cref="PlcReadErrorException"/> 新实例
    /// </summary>
    /// <param name="message">异常消息</param>
    public PlcReadErrorException(string message) : base(message) { }

    /// <summary>
    /// 初始化 <see cref="PlcReadErrorException"/> 新实例
    /// </summary>
    /// <param name="message">异常消息</param>
    /// <param name="innerException">内部异常</param>
    public PlcReadErrorException(string message, Exception innerException) : base(message, innerException) { }
}
