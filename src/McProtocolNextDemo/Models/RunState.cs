// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

namespace McProtocolNextDemo.Models;

/// <summary>
/// 表示状态的枚举
/// </summary>
public enum RunState {
    /// <summary>
    /// 未启动
    /// </summary>
    Stopped,

    /// <summary>
    /// 启动中
    /// </summary>
    Starting,

    /// <summary>
    /// 正在运行
    /// </summary>
    Running,

    /// <summary>
    /// 停止中
    /// </summary>
    Stopping
}
