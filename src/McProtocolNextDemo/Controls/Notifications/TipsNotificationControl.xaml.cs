// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace McProtocolNextDemo.Controls.Notifications;
/// <summary>
/// TipsNotificationControl.xaml 的交互逻辑
/// </summary>
public partial class TipsNotificationControl : UserControl {
    private readonly DispatcherTimer _closeTimer;
    private TimeSpan _timeLeft;
    private bool _isClosing = false;

    private const int TOTAL_SECONDS = 5;

    #region 属性

    private string _title = "Undefined";
    /// <summary>
    /// 获取或设置标题
    /// </summary>
    public string Title {
        get => _title;
        set { _title = value; OnPropertyChanged(nameof(Title)); }
    }

    private string _message = "Undefined";
    /// <summary>
    /// 获取或设置消息内容
    /// </summary>
    public string Message {
        get => _message;
        set { _message = value; OnPropertyChanged(nameof(Message)); }
    }

    private PackIconKind _iconKind = PackIconKind.Information;
    /// <summary>
    /// 获取或设置图标
    /// </summary>
    public PackIconKind IconKind {
        get => _iconKind;
        set { _iconKind = value; OnPropertyChanged(nameof(IconKind)); }
    }

    private string _backgroundColor = "#FFDDDDDD";
    /// <summary>
    /// 获取或设置背景颜色
    /// </summary>
    public string BackgroundColor {
        get => _backgroundColor;
        set { _backgroundColor = value; OnPropertyChanged(nameof(BackgroundColor)); }
    }

    private string _iconColor = "#FF888888";
    /// <summary>
    /// 获取或设置图标颜色
    /// </summary>
    public string IconColor {
        get => _iconColor;
        set { _iconColor = value; OnPropertyChanged(nameof(IconColor)); }
    }

    private string _titleColor = "#FF333333";
    /// <summary>
    /// 获取或设置标题颜色
    /// </summary>
    public string TitleColor {
        get => _titleColor;
        set { _titleColor = value; OnPropertyChanged(nameof(TitleColor)); }
    }

    private string _messageColor = "#FF555555";
    /// <summary>
    /// 获取或设置信息内容颜色
    /// </summary>
    public string MessageColor {
        get => _messageColor;
        set { _messageColor = value; OnPropertyChanged(nameof(MessageColor)); }
    }

    /// <summary>
    /// 获取标题的可见性，根据 Title 是否为空决定
    /// </summary>
    public Visibility TitleVisibility => string.IsNullOrWhiteSpace(Title) ? Visibility.Collapsed : Visibility.Visible;

    #endregion

    /// <summary>
    /// 触发属性更改通知的事件
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 触发属性更改通知
    /// </summary>
    /// <param name="propertyName">更改的属性名称</param>
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// 交互的点击事件
    /// </summary>
    public event Action? OnClick;

    /// <summary>
    /// 通知关闭事件
    /// </summary>
    public event Action? OnClose;

    /// <summary>
    /// 构造函数，初始化 <see cref="TipsNotificationControl"/> 新实例
    /// </summary>
    public TipsNotificationControl() {
        InitializeComponent();
        DataContext = this;

        _closeTimer = new DispatcherTimer {
            Interval = TimeSpan.FromSeconds(1)
        };

        _closeTimer.Tick += CloseTimer_Tick;
        _timeLeft = TimeSpan.FromSeconds(TOTAL_SECONDS);

        RootBorder.MouseEnter += RootBorder_MouseEnter;
        RootBorder.MouseLeave += RootBorder_MouseLeave;
    }

    /// <summary>
    /// 显示通知的动画
    /// </summary>
    public void Show() {
        var fadeIn = new DoubleAnimation {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(300)
        };
        BeginAnimation(OpacityProperty, fadeIn);

        _timeLeft = TimeSpan.FromSeconds(TOTAL_SECONDS);
        _closeTimer.Start();
    }

    /// <summary>
    /// 关闭通知的动画
    /// </summary>
    public void Close() {
        if (_isClosing) {
            return;
        }

        _isClosing = true;
        var fadeOut = new DoubleAnimation {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(520)
        };

        var scaleDown = new DoubleAnimation {
            From = 1,
            To = 0.5,
            Duration = TimeSpan.FromMilliseconds(520)
        };

        ScaleTransform scaleTransform = new(1.0, 1.0);
        RenderTransform = scaleTransform;
        RenderTransformOrigin = new Point(0.5, 0.5);

        fadeOut.Completed += (s, e) => {
            if (_isClosing) {
                OnClose?.Invoke();
            }
        };

        BeginAnimation(OpacityProperty, fadeOut);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleDown);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleDown);
    }

    /// <summary>
    /// 鼠标在通知上单击后的事件处理
    /// </summary>
    private void RootBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        OnClick?.Invoke();
        Close();
    }

    /// <summary>
    /// 定时器Tick事件的处理，用于自动关闭通知
    /// </summary>
    private void CloseTimer_Tick(object? sender, EventArgs e) {
        _timeLeft -= _closeTimer.Interval;
        if (_timeLeft > TimeSpan.Zero) {
            return;
        }

        _closeTimer.Stop();
        Close();
    }

    /// <summary>
    /// 鼠标进入通知区域时的事件处理
    /// </summary>
    private void RootBorder_MouseEnter(object sender, MouseEventArgs e) {
        _isClosing = false;
        _closeTimer.Stop();
        _timeLeft = TimeSpan.FromSeconds(TOTAL_SECONDS);

        var fadeIn = new DoubleAnimation {
            From = Opacity,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(200)
        };
        var scaleUp = new DoubleAnimation {
            From = RenderTransform.Value.M11,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(200)
        };

        BeginAnimation(OpacityProperty, fadeIn);
        if (RenderTransform is not ScaleTransform scaleTransform) {
            return;
        }

        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleUp);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleUp);
    }

    /// <summary>
    /// 鼠标离开通知区域时的事件处理
    /// </summary>
    private void RootBorder_MouseLeave(object sender, MouseEventArgs e) {
        if (_timeLeft > TimeSpan.Zero) {
            _closeTimer.Start();
        }
    }
}
