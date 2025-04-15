// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using McProtocolNextDemo.Abstractions;

namespace McProtocolNextDemo.Models;

/// <summary>
/// 包含所有与应用程序设定相关的配置信息
/// </summary>
public class SettingsConfig : ViewModelBase {
    /// <summary>
    /// 主题更改事件
    /// </summary>
    public event EventHandler? OnAppTheme;

    /// <summary>
    /// 获取 MC 协议配置
    /// </summary>
    public McProtocolConfig McProtocols { get; } = new();

    private string _appTheme = string.Empty;
    /// <summary>
    /// 获取或设置主题
    /// </summary>
    public string AppTheme {
        get => _appTheme;
        set {
            if (SetField(ref _appTheme, value)) {
                OnAppTheme?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private string _fonts = string.Empty;
    /// <summary>
    /// 获取或设置字体
    /// </summary>
    public string Fonts {
        get => _fonts;
        set => SetField(ref _fonts, value);
    }
}
