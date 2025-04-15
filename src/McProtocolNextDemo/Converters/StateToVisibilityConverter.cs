// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using McProtocolNextDemo.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace McProtocolNextDemo.Converters;

/// <summary>
/// 将 RunState 状态转换为 Visibility 值
/// </summary>
public sealed class StateToVisibilityConverter : IValueConverter {
    /// <inheritdoc/>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is not RunState state || parameter is not string targetStates) {
            return Visibility.Collapsed;
        }

        var states = targetStates.Split(',').Select(s => s.Trim());
        return states.Contains(state.ToString()) ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <inheritdoc/>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
