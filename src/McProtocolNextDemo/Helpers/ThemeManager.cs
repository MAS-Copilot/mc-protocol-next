// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using MaterialDesignThemes.Wpf;
using System.Diagnostics;
using System.Windows;
using Wpf.Ui.Appearance;

namespace McProtocolNextDemo.Helpers;

/// <summary>
/// 主题管理
/// </summary>
internal static class ThemeManager {
    /// <summary>
    /// 应用主题
    /// </summary>
    /// <param name="themeName">主题名称</param>
    public static void ApplyTheme(string themeName) {
        var theme = Enum.TryParse<ApplicationTheme>(themeName, true, out var result) ? result : ApplicationTheme.Light;
        ApplicationThemeManager.Apply(theme);
        ModifyMaterialDesignTheme(themeName);
        ModifyCustomTheme(themeName);
        CursorTheme(themeName);

#if DEBUG
        Debug.WriteLine("Current MergedDictionaries Count: " + Application.Current.Resources.MergedDictionaries.Count);
        foreach (var dict in Application.Current.Resources.MergedDictionaries) {
            Debug.WriteLine("Resource Dictionary Source: " + dict.Source);
        }
#endif
    }

    /// <summary>
    /// 修改 Material Design 主题
    /// </summary>
    /// <param name="themeName">主题名称</param>
    private static void ModifyMaterialDesignTheme(string themeName) {
        PaletteHelper paletteHelper = new();
        var theme = paletteHelper.GetTheme();

        theme.SetBaseTheme(themeName.Equals("Dark", StringComparison.OrdinalIgnoreCase)
            ? BaseTheme.Dark
            : BaseTheme.Light);

        paletteHelper.SetTheme(theme);
    }

    /// <summary>
    /// 鼠标主题
    /// </summary>
    /// <param name="themeName">主题名称</param>
    private static void CursorTheme(string themeName) {
        var newThemeUri = new Uri($"pack://application:,,,/McProtocolNextDemo;component/Resources/Cursors/{themeName}Cursors.xaml", UriKind.Absolute);

        ResourceDictionary newThemeDict = new() { Source = newThemeUri };

        var currentThemeDicts = Application.Current.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.StartsWith("pack://application:,,,/McProtocolNextDemo;component/Resources/Cursors/"));

        if (currentThemeDicts != null) {
            int index = Application.Current.Resources.MergedDictionaries.IndexOf(currentThemeDicts);
            Application.Current.Resources.MergedDictionaries[index] = newThemeDict;
        } else {
            Application.Current.Resources.MergedDictionaries.Add(newThemeDict);
        }
    }

    /// <summary>
    /// 修改自定义主题
    /// </summary>
    /// <param name="themeName">主题名称</param>
    private static void ModifyCustomTheme(string themeName) {
        string themePath = themeName == "Dark"
            ? "pack://application:,,,/McProtocolNextDemo;component/Resources/Themes/MAS.Dark.xaml"
            : "pack://application:,,,/McProtocolNextDemo;component/Resources/Themes/MAS.Light.xaml";

        var themeUri = new Uri(themePath, UriKind.Absolute);

        ResourceDictionary newThemeDict = new() { Source = themeUri };

        var oldThemeDicts = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && (d.Source.OriginalString.EndsWith("MAS.Dark.xaml") ||
                                                          d.Source.OriginalString.EndsWith("MAS.Light.xaml")));
        if (oldThemeDicts != null) {
            int index = Application.Current.Resources.MergedDictionaries.IndexOf(oldThemeDicts);
            Application.Current.Resources.MergedDictionaries[index] = newThemeDict;
        } else {
            Application.Current.Resources.MergedDictionaries.Add(newThemeDict);
        }
    }
}
