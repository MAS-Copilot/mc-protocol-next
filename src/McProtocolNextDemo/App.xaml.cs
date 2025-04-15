// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using McProtocolNext;
using McProtocolNextDemo.Controls.Notifications;
using McProtocolNextDemo.Helpers;
using McProtocolNextDemo.Models;
using McProtocolNextDemo.ViewModels.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;

namespace McProtocolNextDemo;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    private static IServiceProvider? Services { get; set; }
    private readonly ServiceProvider _serviceProvider;

    /// <summary>
    /// 获取应用程序配置对象
    /// </summary>
    public static SettingsConfig AppConfig { get; private set; } = new();

    /// <summary>
    /// 程序主要入口
    /// </summary>
    public App() {
        ServiceCollection services = [];
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
        Services = _serviceProvider;
    }

    /// <summary>
    /// 获取已注册的服务
    /// </summary>
    /// <typeparam name="T">要获取的服务类型</typeparam>
    /// <returns>服务的实例或 <see langword="null"/></returns>
    public static T? GetRequiredService<T>()
        where T : class {
        return Services?.GetRequiredService<T>();
    }

    /// <inheritdoc/>
    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);

        Exit += App_Exit;
        AppConfig = LoadConfiguration();
        ThemeManager.ApplyTheme(AppConfig.AppTheme);
        FontsHelper.ModifyFontByFamilyName(AppConfig.Fonts);

        MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }

    #region 私有方法

    private static void ConfigureServices(ServiceCollection services) {
        _ = services.AddSingleton<MainWindow>();
        _ = services.AddSingleton<MainViewModel>();

        // config
        _ = services.AddSingleton<IMcCommunicationConfig>(_ => AppConfig.McProtocols);

        // services
        _ = services.AddMcProtocolService();
        _ = services.AddSingleton<ITipsNotificationService, TipsNotificationService>();
        _ = services.AddSingleton<INotificationHistoryService, NotificationHistoryService>();
    }

    private static SettingsConfig LoadConfiguration() {
        try {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (!File.Exists(configPath)) {
                Environment.Exit(1);
            }

            return FileHelper.DeserializeObjectFromFile<SettingsConfig>(configPath);
        } catch (Exception) {
            Environment.Exit(1);
            return null;
        }
    }

    private void App_Exit(object sender, ExitEventArgs e) {
        _serviceProvider.Dispose();
    }

    #endregion
}

//                            _ooOoo_
//                           o8888888o
//                           88" . "88
//                           (| -_- |)
//                            O\ = /O
//                        ____/`---'\____
//                      .   ' \\| |// `.
//                       / \\||| : |||// \
//                     / _||||| -:- |||||- \
//                       | | \\\ - /// | |
//                     | \_| ''\---/'' | |
//                      \ .-\__ `-` ___/-. /
//                   ___`. .' /--.--\ `. . __
//                ."" '< `.___\_<|>_/___.' >'"".
//               | | : `- \`.;`\ _ /`;.`/ - ` : | |
//                 \ \ `-. \_ __\ /__ _/ .-` / /
//         ======`-.____`-.___\_____/___.-`____.-'======
//                            `=---='
//
//         .............................................
//                  佛祖镇楼                  BUG辟易
