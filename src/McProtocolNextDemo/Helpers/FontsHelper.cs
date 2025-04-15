// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using McProtocolNextDemo.Models;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace McProtocolNextDemo.Helpers;

/// <summary>
/// 字体族辅助类
/// </summary>
internal class FontsHelper {
    /// <summary>
    /// 从字体资源目录获取所有TTF字体的 FontInfo 对象
    /// </summary>
    /// <returns>包含 FontInfo 对象的列表</returns>
    public static List<FontInfo> GetFontInfosFromDirectory() {
        List<FontInfo> fontInfos = [];
        string fontFolderPath = Path.Combine(Environment.CurrentDirectory, "Resources", "Fonts");

        if (!fontFolderPath.EndsWith(Path.DirectorySeparatorChar.ToString())) {
            fontFolderPath += Path.DirectorySeparatorChar;
        }

        var fontDirectoryUri = new Uri(fontFolderPath, UriKind.Absolute);
        try {
            var fontFamilies = Fonts.GetFontFamilies(fontDirectoryUri);
            foreach (var fontFamily in fontFamilies) {
                string name = fontFamily.Source[(fontFamily.Source.LastIndexOf('#') + 1)..];
                var fontInfo = new FontInfo(name, fontFamily);
                fontInfos.Add(fontInfo);
            }
        } catch (Exception ex) {
            throw new Exception("Error processing font files: " + ex.Message);
        }

        return fontInfos;
    }

    /// <summary>
    /// 修改字体家族，从嵌入的资源
    /// </summary>
    /// <param name="fontName">字体名称</param>
    public static void ModifyFontFromResource(string fontName) {
        var fontFamilies = Fonts.GetFontFamilies(new Uri("pack://application:,,,/Resources/Fonts/"), $"./{fontName}.ttf");
        var fontFamily = fontFamilies.FirstOrDefault();
        Application.Current.Resources["GlobalFontFamily"] = fontFamily;
    }

    /// <summary>
    /// 修改字体家族，从资源文件夹
    /// </summary>
    /// <param name="fontName">字体名称</param>
    public static void ModifyFontFromFile(string fontName) {
        var fontFolderPath = Path.Combine(Environment.CurrentDirectory, "Resources", "Fonts");
        if (!fontFolderPath.EndsWith(Path.DirectorySeparatorChar.ToString())) {
            fontFolderPath += Path.DirectorySeparatorChar;
        }

        var fontDirectoryUri = new Uri(fontFolderPath, UriKind.Absolute);

        var fontFamilies = Fonts.GetFontFamilies(fontDirectoryUri, $"{fontName}.ttf");
        var fontFamily = fontFamilies.FirstOrDefault();

        Application.Current.Resources["GlobalFontFamily"] = fontFamily;
    }

    /// <summary>
    /// 修改字体家族，从字体族名称
    /// </summary>
    /// <param name="familyName">字体族名称</param>
    public static void ModifyFontByFamilyName(string familyName) {
        var fontInfos = GetFontInfosFromDirectory();
        var fontInfo = fontInfos.FirstOrDefault(info => info.Name == familyName);
        if (fontInfo != null) {
            Application.Current.Resources["GlobalFontFamily"] = fontInfo.FontFamily;
        } else {
            throw new ArgumentException($"No font family found with the name: {familyName}", nameof(familyName));
        }
    }

    /// <summary>
    /// 修改字体家族
    /// </summary>
    /// <param name="fontFamily"><see cref="FontFamily"/> 字体对象</param>
    public static void ModifyFontFamily(FontFamily fontFamily) {
        Application.Current.Resources["GlobalFontFamily"] = fontFamily;
    }
}
