// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using MaterialDesignThemes.Wpf;

namespace McProtocolNextDemo.Controls.Notifications;

/// <summary>
/// 通知记录的数据模型
/// </summary>
/// <param name="title">标题</param>
/// <param name="message">内容</param>
/// <param name="type">类型</param>
/// <param name="onClick">点击回调</param>
public class NotificationRecord(
    string title,
    string message,
    NotificationType type,
    Action? onClick = null
    ) {
    /// <summary>
    /// 获取通知标题
    /// </summary>
    public string Title { get; init; } = title;

    /// <summary>
    /// 获取通知内容
    /// </summary>
    public string Message { get; init; } = message;

    /// <summary>
    /// 获取通知类型
    /// </summary>
    public NotificationType Type { get; init; } = type;

    /// <summary>
    /// 获取通知时间
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.Now;

    /// <summary>
    /// 获取点击回调
    /// </summary>
    public Action? OnClick { get; init; } = onClick;

    /// <summary>
    /// 获取通知图标类型
    /// </summary>
    public PackIconKind IconKind { get; init; } = GetIconForType(type);

    /// <summary>
    /// 根据通知类型获取相应的图标
    /// </summary>
    private static PackIconKind GetIconForType(NotificationType type) {
        return type switch {
            NotificationType.Info => PackIconKind.InformationVariantCircle,
            NotificationType.Success => PackIconKind.CheckCircle,
            NotificationType.Warning => PackIconKind.Alert,
            NotificationType.Error => PackIconKind.Error,
            _ => PackIconKind.InformationVariantCircle,
        };
    }
}
