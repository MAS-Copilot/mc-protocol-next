// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace McProtocolNextDemo.Converters;

/// <summary>
/// 反转可见性状态的值转换器
/// </summary>
/// <remarks>
/// Converts Visibility.Visible to Visibility.Collapsed and vice versa
/// </remarks>
public sealed class InverseVisibilityConverter : IValueConverter {
    /// <inheritdoc/>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is Visibility visibility) {
            return visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    /// <inheritdoc/>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
