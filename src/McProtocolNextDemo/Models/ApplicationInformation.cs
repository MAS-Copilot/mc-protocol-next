// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace McProtocolNextDemo.Models;

/// <summary>
/// 应用程序信息
/// </summary>
public struct ApplicationInformation {
    /// <summary>
    /// 标题栏
    /// </summary>
    public const string THE_TITLE_BAR = "McProtocolNextDemo";

    /// <summary>
    /// 版权信息
    /// </summary>
    public const string COPYRIGHT_INFORMATION = "Copyright (c) MAS (厦门威光) Corporation. All rights reserved. - ";

    /// <summary>
    /// 项目创建时间
    /// </summary>
    public static readonly string CreatedAt = "2025-04-14";

    /// <summary>
    /// 版本信息
    /// </summary>
    public static readonly string VersionInformation = GetVersionInformation();

    /// <summary>
    /// 文件版本信息
    /// </summary>
    public static readonly string FileVersionInformation = GetFileVersionInformation();

    /// <summary>
    /// 程序集版本信息
    /// </summary>
    public static readonly string AssemblyVersionInformation = GetAssemblyVersionInformation();

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public static readonly string LastUpdated = GetLastUpdated();

    /// <summary>
    /// 应用名称
    /// </summary>
    public const string APPLICATION_NAME = "McProtocolNextDemo";

    /// <summary>
    /// 应用程序图标
    /// </summary>
    public const string APPLICATION_ICON = "images/icons/logo.ico";

    /// <summary>
    /// 配置文件路径
    /// </summary>
    public const string CONFIG_FILE_PATH = "appsettings.json";

    #region 私有方法

    private static string GetVersionInformation() {
        var assembly = Assembly.GetExecutingAssembly();

        var informationalVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        string semanticVersion = informationalVersionAttribute!.InformationalVersion;
        if (semanticVersion.Contains('+')) {
            semanticVersion = semanticVersion.Split('+')[0];
        }

        return $"v{semanticVersion}";
    }

    private static string GetFileVersionInformation() {
        string assemblyPath = Assembly.GetExecutingAssembly().Location;
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(assemblyPath);
        string fileVersion = fileVersionInfo!.FileVersion!;
        return $"v{fileVersion}";
    }

    private static string GetAssemblyVersionInformation() {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;

        return $"v{version}";
    }

    private static string GetLastUpdated() {
        string assemblyPath = Assembly.GetExecutingAssembly().Location;
        DateTime lastWriteTime = File.GetLastWriteTime(assemblyPath);
        return lastWriteTime.ToString("yyyy-MM-dd");
    }

    #endregion
}
