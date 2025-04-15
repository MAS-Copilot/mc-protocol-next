// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using System.Windows.Threading;

namespace McProtocolNextDemo.Helpers;

/// <summary>
/// 防抖机制的帮助类，使用 DispatcherTimer 来限制执行的频率
/// </summary>
public static class DebounceDispatcherHelper {
    private static readonly DispatcherTimer _timer = new();
    private static bool _isDebouncing;

    /// <summary>
    /// 静态构造函数用于设置默认的防抖时间
    /// </summary>
    static DebounceDispatcherHelper() {
        _timer.Interval = TimeSpan.FromMilliseconds(188);
        _timer.Tick += (s, e) => {
            _isDebouncing = false;
            _timer.Stop();
        };
    }

    /// <summary>
    /// 如果当前不在防抖状态，启动防抖定时器并返回 true，否则返回 false
    /// </summary>
    public static bool Debounce() {
        if (_isDebouncing) {
            return false;
        }

        _isDebouncing = true;
        _timer.Start();
        return true;
    }

    /// <summary>
    /// 调整防抖时间间隔
    /// </summary>
    /// <param name="intervalMilliseconds">新的防抖时间间隔（毫秒）</param>
    public static void SetInterval(int intervalMilliseconds) {
        _timer.Interval = TimeSpan.FromMilliseconds(intervalMilliseconds);
    }
}
