// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using MaterialDesignThemes.Wpf;
using McProtocolNextDemo.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace McProtocolNextDemo.Controls.Notifications;

internal class TipsNotificationService(INotificationHistoryService historyService) : ITipsNotificationService {
    public void Info(
        string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null) {

        var localizedMessage = GlobalLanguageManager.TryGetString(message);
        ShowNotification(
            string.Empty,
            localizedMessage ?? message,
            PackIconKind.InformationVariantCircle,
            position,
            onClick,
            backgroundColor: "#F0F4F8",
            iconColor: "#007BFF",
            titleColor: "#333333",
            messageColor: "#666666",
            NotificationType.Info,
            ownerWindow);
    }

    public void Info(
        string title, string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null) {

        var localizedTitle = GlobalLanguageManager.TryGetString(title);
        var localizedMessage = GlobalLanguageManager.TryGetString(message);
        ShowNotification(
            localizedTitle ?? title,
            localizedMessage ?? message,
            PackIconKind.InformationVariantCircle,
            position,
            onClick,
            backgroundColor: "#F0F4F8",
            iconColor: "#007BFF",
            titleColor: "#333333",
            messageColor: "#666666",
            NotificationType.Info,
            ownerWindow);
    }

    public void Success(
        string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null) {

        var localizedMessage = GlobalLanguageManager.TryGetString(message);
        ShowNotification(
            string.Empty,
            localizedMessage ?? message,
            PackIconKind.CheckCircle,
            position,
            onClick,
            backgroundColor: "#FFE8F5E9",
            iconColor: "#FF4CAF50",
            titleColor: "#FF2E7D32",
            messageColor: "#FF388E3C",
            NotificationType.Success,
            ownerWindow);
    }

    public void Success(
        string title,
        string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null) {

        var localizedTitle = GlobalLanguageManager.TryGetString(title);
        var localizedMessage = GlobalLanguageManager.TryGetString(message);
        ShowNotification(
            localizedTitle ?? title,
            localizedMessage ?? message,
            PackIconKind.CheckCircle,
            position,
            onClick,
            backgroundColor: "#FFE8F5E9",
            iconColor: "#FF4CAF50",
            titleColor: "#FF2E7D32",
            messageColor: "#FF388E3C",
            NotificationType.Success,
            ownerWindow);
    }

    public void Warning(
        string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null) {

        var localizedMessage = GlobalLanguageManager.TryGetString(message);
        ShowNotification(
            string.Empty,
            localizedMessage ?? message,
            PackIconKind.Alert,
            position,
            onClick,
            backgroundColor: "#FFFFF8E1",
            iconColor: "#FFFFC107",
            titleColor: "#FFF57F17",
            messageColor: "#FF6D4C41",
            NotificationType.Warning,
            ownerWindow);
    }

    public void Warning(
        string title,
        string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null) {

        var localizedTitle = GlobalLanguageManager.TryGetString(title);
        var localizedMessage = GlobalLanguageManager.TryGetString(message);
        ShowNotification(
            localizedTitle ?? title,
            localizedMessage ?? message,
            PackIconKind.Alert,
            position,
            onClick,
            backgroundColor: "#FFFFF8E1",
            iconColor: "#FFFFC107",
            titleColor: "#FFF57F17",
            messageColor: "#FF6D4C41",
            NotificationType.Warning,
            ownerWindow);
    }

    public void Error(
        string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null) {

        var localizedMessage = GlobalLanguageManager.TryGetString(message);
        ShowNotification(
            string.Empty,
            localizedMessage ?? message,
            PackIconKind.Error,
            position,
            onClick,
            backgroundColor: "#FFFFEBEE",
            iconColor: "#FFF44336",
            titleColor: "#FFD32F2F",
            messageColor: "#FFB71C1C",
            NotificationType.Error,
            ownerWindow);
    }

    public void Error(
        string title,
        string message,
        InfoPosition position = InfoPosition.BottomRight,
        Action? onClick = null,
        Window? ownerWindow = null) {

        var localizedTitle = GlobalLanguageManager.TryGetString(title);
        var localizedMessage = GlobalLanguageManager.TryGetString(message);
        ShowNotification(
            localizedTitle ?? title,
            localizedMessage ?? message,
            PackIconKind.Error,
            position,
            onClick,
            backgroundColor: "#FFFFEBEE",
            iconColor: "#FFF44336",
            titleColor: "#FFD32F2F",
            messageColor: "#FFB71C1C",
            NotificationType.Error,
            ownerWindow);
    }

    #region 私有方法

    /// <summary>
    /// 显示通知
    /// </summary>
    /// <param name="title">通知标题</param>
    /// <param name="message">通知内容</param>
    /// <param name="iconKind">图标类型，使用Material Design图标</param>
    /// <param name="position">通知显示的位置</param>
    /// <param name="onClick">点击通知时的回调函数</param>
    /// <param name="backgroundColor">背景颜色的字符串表示</param>
    /// <param name="iconColor">图标颜色的字符串表示</param>
    /// <param name="titleColor">标题颜色的字符串表示</param>
    /// <param name="messageColor">消息颜色的字符串表示</param>
    /// <param name="type">通知类型</param>
    /// <param name="ownerWindow">通知所属的窗口</param>
    /// <exception cref="InvalidOperationException">当无法找到显示通知的窗口时抛出</exception>        
    private void ShowNotification(
        string title,
        string message,
        PackIconKind iconKind,
        InfoPosition position,
        Action? onClick,
        string backgroundColor,
        string iconColor,
        string titleColor,
        string messageColor,
        NotificationType type,
        Window? ownerWindow = null) {

        _ = Application.Current.Dispatcher.BeginInvoke(() => {
            var notificationRecord = new NotificationRecord(
                title, message, type, onClick);
            historyService.AddNotification(notificationRecord);

            Window? targetWindow = ownerWindow ?? TryGetActiveWindow() ?? Application.Current.MainWindow;
            if (ownerWindow == null && targetWindow != null &&
                (targetWindow.Width < 720 || targetWindow.Height < 450)) {
                targetWindow = Application.Current.MainWindow;
            }

            if (targetWindow == null) {
                // TODO: 处理如果没有活动的窗口，例如，应用程序最小化时使用托盘通知
                return;
            }

            Panel? host = targetWindow.FindName("TipsNotificationHost") as Panel;
            if (host == null) {
                targetWindow = Application.Current.MainWindow;
                host = targetWindow.FindName("TipsNotificationHost") as Panel;
                if (host == null) {
                    throw new InvalidOperationException($"窗口 '{targetWindow.Title}' 中未找到名为 'TipsNotificationHost' 的容器");
                }
            }

            var existingNotifications = host.Children.OfType<TipsNotificationControl>().ToList();
            foreach (var existingNotification in existingNotifications) {
                existingNotification.Close();
            }

            var notification = new TipsNotificationControl {
                Title = title,
                Message = message,
                IconKind = iconKind,
                BackgroundColor = backgroundColor,
                IconColor = iconColor,
                TitleColor = titleColor,
                MessageColor = messageColor
            };

            notification.OnClick += onClick;
            notification.OnClose += () => host.Children.Remove(notification);

            SetNotificationPosition(notification, position);
            _ = host.Children.Add(notification);
            notification.Show();
        });
    }

    /// <summary>
    /// 尝试获取当前激活的窗口
    /// </summary>
    /// <returns>当前激活的窗口对象，如果没有激活窗口则返回null</returns>
    private static Window? TryGetActiveWindow() {
        return Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window.IsActive);
    }

    /// <summary>
    /// 设置通知控件的位置
    /// </summary>
    /// <param name="notification">通知控件实例</param>
    /// <param name="position">通知的位置枚举</param>
    private static void SetNotificationPosition(TipsNotificationControl notification, InfoPosition position) {
        double startX = 0;
        const double endX = 0;
        const double duration = 520;

        switch (position) {
            case InfoPosition.Center:
                notification.HorizontalAlignment = HorizontalAlignment.Center;
                notification.VerticalAlignment = VerticalAlignment.Center;
                break;
            case InfoPosition.TopLeft:
                notification.HorizontalAlignment = HorizontalAlignment.Left;
                notification.VerticalAlignment = VerticalAlignment.Top;
                notification.Margin = new Thickness(20, 20, 0, 0);
                startX = -300;
                break;
            case InfoPosition.TopRight:
                notification.HorizontalAlignment = HorizontalAlignment.Right;
                notification.VerticalAlignment = VerticalAlignment.Top;
                notification.Margin = new Thickness(0, 20, 20, 0);
                startX = 300;
                break;
            case InfoPosition.BottomLeft:
                notification.HorizontalAlignment = HorizontalAlignment.Left;
                notification.VerticalAlignment = VerticalAlignment.Bottom;
                notification.Margin = new Thickness(20, 0, 0, 20);
                startX = -300;
                break;
            case InfoPosition.BottomRight:
            default:
                notification.HorizontalAlignment = HorizontalAlignment.Right;
                notification.VerticalAlignment = VerticalAlignment.Bottom;
                notification.Margin = new Thickness(0, 0, 20, 20);
                startX = 300;
                break;
        }

        ApplyAnimation(notification, startX, endX, duration);
    }

    /// <summary>
    /// 应用平移动画到通知控件
    /// </summary>
    /// <param name="notification">通知控件实例</param>
    /// <param name="from">动画起始位置</param>
    /// <param name="to">动画终止位置</param>
    /// <param name="duration">动画持续时间，单位为毫秒</param>
    private static void ApplyAnimation(TipsNotificationControl notification, double from, double to, double duration) {
        var slideInAnimation = new DoubleAnimation {
            From = from,
            To = to,
            Duration = TimeSpan.FromMilliseconds(duration),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        var transform = notification.RenderTransform as TranslateTransform ?? new TranslateTransform();
        notification.RenderTransform = transform;
        transform.BeginAnimation(TranslateTransform.XProperty, slideInAnimation);
    }

    #endregion
}
