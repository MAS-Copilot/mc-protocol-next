// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using MaterialDesignThemes.Wpf;
using McProtocolNextDemo.Models;
using System.Globalization;
using System.Windows.Data;

namespace McProtocolNextDemo.Converters;

/// <summary>
/// 用于将信息等级转换为对应的Material Design图标
/// </summary>
public sealed class InfoLevelToIconKindConverter : IValueConverter {
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is InfoLevel level) {
            return level switch {
                InfoLevel.Info => PackIconKind.Information,
                InfoLevel.Succeed => PackIconKind.CheckCircle,
                InfoLevel.Warning => PackIconKind.Alert,
                InfoLevel.Error => PackIconKind.CloseCircle,
                _ => (object)PackIconKind.Information,
            };
        }

        return PackIconKind.Information;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotSupportedException();
    }
}
