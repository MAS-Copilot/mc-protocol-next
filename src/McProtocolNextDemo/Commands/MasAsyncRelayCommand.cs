// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using McProtocolNextDemo.Controls.Notifications;
using McProtocolNextDemo.Helpers;
using System.Windows.Input;

namespace McProtocolNextDemo.Commands;

/// <summary>
/// 异步执行操作的命令
/// 允许将异步操作绑定到UI命令上
/// </summary>
/// <remarks>
/// 创建 <see cref="MasAsyncRelayCommand"/>> 的新实例
/// </remarks>
/// <param name="executeAsync">定义要异步执行的委托任务。此参数不能为 null</param>
/// <param name="canExecute">定义命令是否可执行的逻辑。它接收一个可选的参数，当此参数为 null 时，命令总是可执行的</param>
/// <param name="isDebounce">表示是否启用防抖</param>
public class MasAsyncRelayCommand(Func<object?, Task> executeAsync, Predicate<object?>? canExecute = null, bool isDebounce = true) : ICommand {
    private readonly ITipsNotificationService _tipsNotification = App.GetRequiredService<ITipsNotificationService>()!;

    private readonly Func<object?, Task> _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged;

    /// <inheritdoc/>
    public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;

    /// <inheritdoc/>
    public async void Execute(object? parameter) {
        if (isDebounce && !DebounceDispatcherHelper.Debounce()) {
            _tipsNotification.Warning("OperationTooFrequent");
            return;
        }

        try {
            await _executeAsync(parameter);
        } catch (Exception ex) {
            _tipsNotification.Error("UnexpectedErrorOccurred", ex.Message, InfoPosition.TopRight);
        }
    }

    /// <summary>
    /// 触发 CanExecuteChanged 事件，通知命令的可执行状态可能已更改
    /// </summary>
    public void RaiseCanExecuteChanged() {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
