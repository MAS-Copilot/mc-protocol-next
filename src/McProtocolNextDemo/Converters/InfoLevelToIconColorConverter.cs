// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using McProtocolNextDemo.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace McProtocolNextDemo.Converters;

/// <summary>
/// 用于将信息等级转换为对应的颜色
/// </summary>
public sealed class InfoLevelToIconColorConverter : IValueConverter {
    /// <inheritdoc/>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is InfoLevel level) {
            return level switch {
                InfoLevel.Info => new SolidColorBrush(Color.FromArgb(255, 79, 100, 238)),
                InfoLevel.Succeed => new SolidColorBrush(Color.FromArgb(255, 52, 228, 42)),
                InfoLevel.Warning => new SolidColorBrush(Color.FromArgb(255, 255, 205, 50)),
                InfoLevel.Error => new SolidColorBrush(Color.FromArgb(255, 254, 95, 30)),
                _ => Brushes.LightGray,
            };
        }

        return Brushes.LightGray;
    }

    /// <inheritdoc/>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotSupportedException();
    }
}
