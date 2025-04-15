// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using McProtocolNextDemo.ViewModels.Windows;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace McProtocolNextDemo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    private bool _isMsgAutoScroll = true;

    /// <summary>
    /// 获取视图模型实例
    /// </summary>
    public MainViewModel ViewModel { get; }

    /// <summary>
    /// 构造函数，初始化 <see cref="MainWindow"/> 新实例
    /// </summary>
    public MainWindow(MainViewModel viewModel) {
        ViewModel = viewModel;
        DataContext = ViewModel;
        ViewModel.OnMessageUpdated += (_, _) => ScrollToBottom();
        InitializeComponent();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        if (e.ClickCount == 2) {

        } else {
            GetWindow((DependencyObject)sender).DragMove();
        }
    }

    private void ScrollToBottom() {
        if (_isMsgAutoScroll && InstantMessageListBox.Items.Count > 0) {
            InstantMessageListBox.ScrollIntoView(InstantMessageListBox.Items[^1]);
        }
    }

    private void ListBox_MouseEnter(object sender, MouseEventArgs e) {
        if (Equals(sender, InstantMessageListBox)) {
            _isMsgAutoScroll = false;
        }
    }

    private void ListBox_MouseLeave(object sender, MouseEventArgs e) {
        if (Equals(sender, InstantMessageListBox)) {
            _isMsgAutoScroll = true;
        }
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
        _ = Process.Start(new ProcessStartInfo {
            FileName = e.Uri.ToString(),
            UseShellExecute = true
        });
        e.Handled = true;
    }
}
