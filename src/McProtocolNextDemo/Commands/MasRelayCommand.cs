// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using McProtocolNextDemo.Controls.Notifications;
using McProtocolNextDemo.Helpers;
using System.Windows.Input;

namespace McProtocolNextDemo.Commands;

/// <summary>
/// 根据特定逻辑执行和判断是否可以执行的命令
/// </summary>
/// <remarks>
/// 创建 <see cref="MasRelayCommand"/> 的新实例
/// </remarks>
/// <param name="execute">执行命令的委托，此参数不可为 null</param>
/// <param name="canExecute">定义命令是否可执行的逻辑。它接收一个可选的参数，当此参数为 null 时，命令总是可执行的</param>
/// <param name="isDebounce">表示是否启用防抖</param>
public class MasRelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null, bool isDebounce = true) : ICommand {
    private readonly ITipsNotificationService _tipsNotification = App.GetRequiredService<ITipsNotificationService>()!;

    private readonly Action<object?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    private readonly Predicate<object?>? _canExecute = canExecute;

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged;

    /// <inheritdoc/>
    public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

    /// <inheritdoc/>
    public void Execute(object? parameter) {
        if (isDebounce && !DebounceDispatcherHelper.Debounce()) {
            _tipsNotification.Warning("OperationTooFrequent");
            return;
        }

        try {
            _execute(parameter);
        } catch (Exception ex) {
            _tipsNotification.Error("UnexpectedErrorOccurred", ex.Message, InfoPosition.TopRight);
        }
    }

    /// <summary>
    /// 触发 CanExecuteChanged 事件, 通知命令的可执行状态可能已更改
    /// </summary>
    public void RaiseCanExecuteChanged() {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
