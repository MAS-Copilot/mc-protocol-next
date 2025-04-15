// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using System.ComponentModel;
using System.Globalization;

namespace McProtocolNextDemo.Common;

/// <summary>
/// 全局语言文化管理
/// </summary>
public class GlobalLanguageManager : INotifyPropertyChanged {
    private static GlobalLanguageManager? _instance;

    /// <summary>
    /// 获取 <see cref="GlobalLanguageManager"/> 类的单一实例
    /// </summary>
    public static GlobalLanguageManager Instance => _instance ??= new GlobalLanguageManager();

    /// <summary>
    /// 触发属性更改通知的事件
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 语言改变事件，在语言切换后触发
    /// </summary>
    public static event EventHandler? LanguageChanged;

    /// <summary>
    /// 触发属性更改通知
    /// </summary>
    /// <param name="propertyName">更改的属性名称</param>
    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// 通过资源键名获取对应的本地化字符串
    /// </summary>
    /// <param name="key">资源的键名，用于查找对应的本地化字符串</param>
    /// <returns>与提供的键名对应的本地化字符串</returns>
    public string this[string key] => GetString(key);

    /// <summary>
    /// 根据语言描述切换语言
    /// </summary>
    /// <param name="languageDescription">语言描述（例如：“简体中文”、“English”）</param>
    public void ChangeLanguageByDescription(string languageDescription) {
        string languageCode = ParseLanguage(languageDescription);
        ChangeLanguage(languageCode);
    }

    /// <summary>
    /// 得到字符串
    /// </summary>
    /// <param name="key">资源键名 Name</param>
    /// <returns>对应的 value，未找到则返回字符串 "{key} is Undefined"</returns>
    public static string GetString(string key) {
        var result = Resources.Resources.ResourceManager.GetString(key, Resources.Resources.Culture) ?? $"{key} is Undefined";
        return result;
    }

    /// <summary>
    /// 尝试得到字符串
    /// </summary>
    /// <param name="key">资源键名 Name</param>
    /// <returns>对应的 value，未找到则返回null</returns>
    public static string? TryGetString(string key) {
        return Resources.Resources.ResourceManager.GetString(key, Resources.Resources.Culture);
    }

    /// <summary>
    /// 切换语言
    /// </summary>
    /// <param name="languageCode">语言代码</param>
    private void ChangeLanguage(string languageCode) {
        CultureInfo newCulture = new(languageCode);
        Thread.CurrentThread.CurrentCulture = newCulture;
        Thread.CurrentThread.CurrentUICulture = newCulture;

        Resources.Resources.Culture = newCulture;

        OnPropertyChanged("Item[]");
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 将语言描述转换为对应的文化信息代码
    /// </summary>
    /// <param name="type">语言描述</param>
    /// <returns>对应的文化信息代码</returns>
    private static string ParseLanguage(string type) {
        return type switch {
            "简体中文" => "zh",
            "繁體中文" => "zh-TW",
            "English" => "en-US",
            _ => "en-US"
        };
    }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private GlobalLanguageManager() { }
}
