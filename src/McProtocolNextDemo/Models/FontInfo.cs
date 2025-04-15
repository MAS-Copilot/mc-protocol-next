// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using System.Windows.Media;

namespace McProtocolNextDemo.Models;

/// <summary>
/// 字体家族信息数据模型
/// </summary>
/// <param name="name">字体名称</param>
/// <param name="fontFamily">字体家族</param>
public class FontInfo(string name, FontFamily fontFamily) {
    /// <summary>
    /// 获取字体名称
    /// </summary>
    public string Name { get; init; } = name;

    /// <summary>
    /// 获取字体家族
    /// </summary>
    public FontFamily FontFamily { get; init; } = fontFamily;
}
