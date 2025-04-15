// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace McProtocolNextDemo.Controls.Notifications;

internal class NotificationHistoryService : INotificationHistoryService {
    private readonly ConcurrentBag<NotificationRecord> _notifications = [];

    public event EventHandler<NotificationRecord>? NotificationsUpdated;

    public void AddNotification(NotificationRecord notification) {
        _notifications.Add(notification);
        NotificationsUpdated?.Invoke(null, notification);
    }

    public void ClearNotifications() {
        while (!_notifications.IsEmpty) {
            _ = _notifications.TryTake(out _);
        }
    }

    public IReadOnlyList<NotificationRecord> GetAllNotifications() {
        return [.. _notifications];
    }

    public async Task SaveNotificationsToFileAsync(string filePath) {
        var allNotifications = GetAllNotifications();
        StringBuilder sb = new();

        foreach (var notification in allNotifications) {
            string timestamp = notification.Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff")
                .Replace("\t", " ")
                .Replace("\n", " ")
                .Replace("\r", " ");
            string type = notification.Type.ToString()
                .Replace("\t", " ")
                .Replace("\n", " ")
                .Replace("\r", " ");

            string title = string.IsNullOrWhiteSpace(notification.Title) ? "无标题" : notification.Title;
            title = title
                .Replace("\t", " ")
                .Replace("\n", " ")
                .Replace("\r", " ");

            string message = notification.Message;

            _ = sb.AppendLine($"{timestamp}\t{type}\t{title}\t{message}");
        }

        await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);
    }
}
