// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using McProtocolNextDemo.Common;
using McProtocolNextDemo.Models;

namespace McProtocolNextDemo.Helpers;

/// <summary>
/// 即时信息帮助类
/// </summary>
public static class InstantMessageHelper {
    /// <summary>
    /// 创建一条即时信息
    /// </summary>
    /// <remarks>
    /// 优先通过键值从语言文化中尝试获取指定的文本，如果未找到则使用原文本
    /// </remarks>
    /// <param name="localizedKey">信息内容</param>
    /// <param name="level">信息等级</param>
    /// <returns><see cref="InstantMessageModel"/> 对象</returns>
    public static InstantMessageModel Create(string localizedKey, InfoLevel level = InfoLevel.Info) {
        var fullMessage = GlobalLanguageManager.TryGetString(localizedKey);
        return new InstantMessageModel { Message = fullMessage ?? localizedKey, Level = level };
    }

    /// <summary>
    /// 创建一条即时信息
    /// </summary>
    /// <remarks>
    /// 优先通过键值从语言文化中尝试获取指定的文本，如果未找到则使用原文本
    /// </remarks>
    /// <param name="messages">信息内容</param>
    /// <param name="level">信息等级</param>
    /// <returns><see cref="InstantMessageModel"/> 对象</returns>
    public static InstantMessageModel Create(InfoLevel level = InfoLevel.Info, params string[] messages) {
        var finalMessage = string.Concat(messages.Select(m => GlobalLanguageManager.TryGetString(m) ?? m));
        return new InstantMessageModel { Message = finalMessage, Level = level };
    }

    /// <summary>
    /// 创建一条即时信息
    /// </summary>
    /// <remarks>
    /// 优先通过键值从语言文化中尝试获取指定的文本，如果未找到则使用原文本
    /// </remarks>
    /// <param name="messages">一个或多个消息内容字符串</param>
    /// <returns>包含合并后的消息和默认信息等级（InfoLevel.Info）的 <see cref="InstantMessageModel"/> 对象</returns>
    public static InstantMessageModel Create(params string[] messages) {
        return Create(InfoLevel.Info, messages);
    }
}
