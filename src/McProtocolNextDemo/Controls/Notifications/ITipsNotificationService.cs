// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using System.Windows;

namespace McProtocolNextDemo.Controls.Notifications;

/// <summary>
/// Tips 通知提示服务接口
/// </summary>
public interface ITipsNotificationService {
    /// <summary>
    /// 信息提示
    /// </summary>
    /// <remarks>
    /// 优先查找本地语言文化资源使用，如果未找到则使用原文本
    /// </remarks>
    /// <param name="message">内容</param>
    /// <param name="position">通知显示的位置</param>
    /// <param name="onClick">点击时的回调操作</param>
    /// <param name="ownerWindow">指定窗口作为通知的宿主</param>
    void Info(
        string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null);

    /// <summary>
    /// 信息提示
    /// </summary>
    /// <remarks>
    /// 优先查找本地语言文化资源使用，如果未找到则使用原文本
    /// </remarks>
    /// <param name="title">标题</param>
    /// <param name="message">内容</param>
    /// <param name="position">通知显示的位置</param>
    /// <param name="onClick">点击时的回调操作</param>
    /// <param name="ownerWindow">指定窗口作为通知的宿主</param>
    void Info(
        string title,
        string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null);

    /// <summary>
    /// 成功提示
    /// </summary>
    /// <remarks>
    /// 优先查找本地语言文化资源使用，如果未找到则使用原文本
    /// </remarks>
    /// <param name="message">内容</param>
    /// <param name="position">通知显示的位置</param>
    /// <param name="onClick">点击时的回调操作</param>
    /// <param name="ownerWindow">指定窗口作为通知的宿主</param>
    void Success(
        string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null);

    /// <summary>
    /// 成功提示
    /// </summary>
    /// <remarks>
    /// 优先查找本地语言文化资源使用，如果未找到则使用原文本
    /// </remarks>
    /// <param name="title">标题</param>
    /// <param name="message">内容</param>
    /// <param name="position">通知显示的位置</param>
    /// <param name="onClick">点击时的回调操作</param>
    /// <param name="ownerWindow">指定窗口作为通知的宿主</param>
    void Success(
        string title,
        string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null);

    /// <summary>
    /// 警告提示
    /// </summary>
    /// <remarks>
    /// 优先查找本地语言文化资源使用，如果未找到则使用原文本
    /// </remarks>
    /// <param name="message">内容</param>
    /// <param name="position">通知显示的位置</param>
    /// <param name="onClick">点击时的回调操作</param>
    /// <param name="ownerWindow">指定窗口作为通知的宿主</param>
    void Warning(
        string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null);

    /// <summary>
    /// 警告提示
    /// </summary>
    /// <remarks>
    /// 优先查找本地语言文化资源使用，如果未找到则使用原文本
    /// </remarks>
    /// <param name="title">标题</param>
    /// <param name="message">内容</param>
    /// <param name="position">通知显示的位置</param>
    /// <param name="onClick">点击时的回调操作</param>
    /// <param name="ownerWindow">指定窗口作为通知的宿主</param>
    void Warning(
        string title,
        string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null);

    /// <summary>
    /// 错误提示
    /// </summary>
    /// <remarks>
    /// 优先查找本地语言文化资源使用，如果未找到则使用原文本
    /// </remarks>
    /// <param name="message">内容</param>
    /// <param name="position">通知显示的位置</param>
    /// <param name="onClick">点击时的回调操作</param>
    /// <param name="ownerWindow">指定窗口作为通知的宿主</param>
    void Error(
        string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null);

    /// <summary>
    /// 错误提示
    /// </summary>
    /// <remarks>
    /// 优先查找本地语言文化资源使用，如果未找到则使用原文本
    /// </remarks>
    /// <param name="title">标题</param>
    /// <param name="message">内容</param>
    /// <param name="position">通知显示的位置</param>
    /// <param name="onClick">点击时的回调操作</param>
    /// <param name="ownerWindow">指定窗口作为通知的宿主</param>
    void Error(
        string title, string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null);
}
