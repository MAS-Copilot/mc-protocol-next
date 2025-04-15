// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;

namespace McProtocolNext;

/// <summary>
/// 提供扩展方法，用于在 <see cref="IServiceCollection"/> 中注册与三菱通讯相关的服务
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// 在指定的 <see cref="IServiceCollection"/> 中注册与三菱通讯相关的服务
    /// </summary>
    /// <param name="services">
    /// 要添加服务的 <see cref="IServiceCollection"/> 实例
    /// </param>
    /// <remarks>
    /// <list>
    /// <item>
    /// <description><see cref="IMcProtocol"/>：与三菱 MC 协议的 PLC 设备进行通信</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <returns>已注册的服务集合</returns>
    public static IServiceCollection AddMcProtocolService(this IServiceCollection services) {
        _ = services.AddSingleton<IMcProtocol, McProtocol>();

        return services;
    }
}
