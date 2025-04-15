// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

namespace McProtocolNextDemo.Controls.Notifications;

/// <summary>
/// 通知历史记录服务的接口
/// </summary>
/// <remarks>
/// 用于存储、查看整个应用程序生命周期的所有通知
/// </remarks>
public interface INotificationHistoryService {
    /// <summary>
    /// 通知数据更新事件
    /// </summary>
    event EventHandler<NotificationRecord> NotificationsUpdated;

    /// <summary>
    /// 添加一条通知到历史记录
    /// </summary>
    /// <param name="notification">通知信息</param>
    void AddNotification(NotificationRecord notification);

    /// <summary>
    /// 获取所有的通知历史记录
    /// </summary>
    /// <returns>通知记录的只读列表</returns>
    IReadOnlyList<NotificationRecord> GetAllNotifications();

    /// <summary>
    /// 清除所有通知历史记录
    /// </summary>
    void ClearNotifications();

    /// <summary>
    /// 异步将所有通知保存到指定的文件路径
    /// </summary>
    /// <param name="filePath">保存文件的路径</param>
    /// <returns>异步任务</returns>
    Task SaveNotificationsToFileAsync(string filePath);
}
