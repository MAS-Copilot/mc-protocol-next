// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

namespace McProtocolNextDemo.Models;

/// <summary>
/// 即时信息数据模型
/// </summary>
public class InstantMessageModel {
    /// <summary>
    /// 获取或设置时间戳
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// 获取或设置信息内容，仅在初始化时可设置
    /// </summary>
    public string Message { get; init; } = "N/A";

    /// <summary>
    /// 获取或设置信息等级，仅在初始化时可设置
    /// </summary>
    public InfoLevel Level { get; init; }
}
