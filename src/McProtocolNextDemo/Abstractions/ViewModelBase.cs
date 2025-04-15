// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using CommunityToolkit.Mvvm.ComponentModel;
using McProtocolNextDemo.Models;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Input;

namespace McProtocolNextDemo.Abstractions;

/// <summary>
/// 视图模型基类
/// </summary>
public abstract class ViewModelBase : ObservableObject {
    /// <summary>
    /// 应用程序配置信息
    /// </summary>
    public static SettingsConfig Config => App.AppConfig;

    private bool _isBusy;
    /// <summary>
    /// 获取或设置是否忙碌
    /// </summary>
    [JsonIgnore]
    public virtual bool IsBusy {
        get => _isBusy;
        set => SetField(ref _isBusy, value, nameof(IsBusy));
    }

    /// <summary>
    /// 忙碌状态光标
    /// </summary>
    protected readonly Cursor BusyCursor =
        Application.Current.Resources["Busy"] as Cursor ?? Cursors.Wait;

    /// <summary>
    /// 正在工作/任务中光标
    /// </summary>
    protected readonly Cursor WorkingCursor =
        Application.Current.Resources["Working"] as Cursor ?? Cursors.AppStarting;

    /// <summary>
    /// 恢复默认光标状态
    /// </summary>
    public void ResetCursor() {
        Mouse.OverrideCursor = null;
        IsBusy = false;
    }

    /// <summary>
    /// 设置为忙碌状态光标
    /// </summary>
    public void SetBusyCursor() {
        Mouse.OverrideCursor = BusyCursor;
        IsBusy = true;
    }

    /// <summary>
    /// 设置为正在工作/任务中的光标
    /// </summary>
    public void SetWorkingCursor() {
        Mouse.OverrideCursor = WorkingCursor;
        IsBusy = true;
    }

    /// <summary>
    /// 辅助方法，用于设置属性值并触发属性更改通知
    /// </summary>
    /// <typeparam name="T">属性的类型</typeparam>
    /// <param name="field">引用后备字段。这个字段将被更新为新的值</param>
    /// <param name="value">要设置的新值。如果新值与旧值相同，则不会触发通知</param>
    /// <param name="propertyName">更改的属性的名称</param>
    /// <returns>如果属性值发生了变化返回true。反之false</returns>
    protected virtual bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "") {
        if (EqualityComparer<T>.Default.Equals(field, value)) {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
