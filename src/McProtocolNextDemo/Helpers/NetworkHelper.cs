// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;

namespace McProtocolNextDemo.Helpers;

/// <summary>
/// 承载网络相关的功能以及工具
/// </summary>>
public static class NetworkHelper {
    /// <summary>
    /// 表示网络接口类型
    /// </summary>
    public enum InterfaceType {
        /// <summary>
        /// 以太网
        /// </summary>
        Ethernet,
        /// <summary>
        /// 无线网
        /// </summary>
        Wireless,
        /// <summary>
        /// 其它
        /// </summary>
        Other
    }

    /// <summary>
    /// 配置代理并返回配置了代理的 HttpClient
    /// </summary>
    /// <param name="proxyAddress">代理服务器的地址</param>
    /// <param name="proxyPort">代理服务器的端口</param>
    /// <returns>配置了代理的 HttpClient 实例</returns>
    public static HttpClient CreateHttpClientWithProxy(string proxyAddress, int proxyPort) {
        var proxy = new WebProxy($"http://{proxyAddress}:{proxyPort}");
        var httpClientHandler = new HttpClientHandler {
            Proxy = proxy,
            UseProxy = true
        };

        return new HttpClient(httpClientHandler);
    }

    /// <summary>
    /// 获取所有活跃网络接口的本地IP地址
    /// </summary>
    /// <returns>字典，键为接口类型，值为该类型的所有活跃IPv4地址列表</returns>
    public static Dictionary<InterfaceType, List<string>> GetAllLocalIPAddresses() {
        var networkInterfaces = GetActiveNetworkInterfaces();

        return networkInterfaces
            .SelectMany(ni => GetInterfaceAndIPs(ni))
            .GroupBy(item => item.Interface)
            .ToDictionary(group => group.Key, group => group.SelectMany(item => item.IPAddresses).Distinct().ToList());
    }

    /// <summary>
    /// 获取指定网络接口的IPv4地址列表
    /// </summary>
    /// <param name="networkInterface">网络接口</param>
    /// <returns>IPv4地址列表</returns>
    public static IEnumerable<string> GetIPv4Addresses(NetworkInterface networkInterface) {
        return networkInterface.GetIPProperties().UnicastAddresses
            .Where(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .Select(ip => ip.Address.ToString());
    }

    /// <summary>
    /// 获取指定网络接口的IPv6地址列表
    /// </summary>
    /// <param name="networkInterface">网络接口</param>
    /// <returns>IPv6地址列表</returns>
    public static IEnumerable<string> GetIPv6Addresses(NetworkInterface networkInterface) {
        return networkInterface.GetIPProperties().UnicastAddresses
            .Where(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            .Select(ip => ip.Address.ToString());
    }

    /// <summary>
    /// 获取所有活跃的 TCP 连接
    /// </summary>
    /// <returns>一个包含本地端点和远程端点的元组列表</returns>
    public static IEnumerable<(string LocalEndPoint, string RemoteEndPoint, TcpState State)> GetActiveTcpConnections() {
        return IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections()
            .Select(conn => (conn.LocalEndPoint.ToString(), conn.RemoteEndPoint.ToString(), conn.State));
    }

    /// <summary>
    /// 检查是否可以连接到互联网，通过ping host
    /// </summary>
    /// <param name="host">要ping的网址</param>
    /// <param name="outtime">超时时间 单位：ms</param>
    /// <returns>成功返回true，否则false</returns>
    public static async Task<bool> CanPingAsync(string host, int outtime) {
        using Ping ping = new();
        PingReply reply = await ping.SendPingAsync(host, outtime);

        if (reply.Status == IPStatus.Success) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 异步获取公共IP地址
    /// </summary>
    /// <remarks>
    /// 使用 https://api.ipify.org 服务，此方法会返回当前设备在互联网上的公共IPv4地址
    /// </remarks>
    /// <returns>公共IP地址，即设备在外部网络中的IPv4地址字符串</returns>
    public static async Task<string> GetPublicIPAddressAsync() {
        using var httpClient = new HttpClient();
        string response = await httpClient.GetStringAsync("https://api.ipify.org");
        return response.Trim();
    }

    /// <summary>
    /// 异步检查指定主机的网络连接状态
    /// </summary>
    /// <param name="host">要检查的主机地址</param>
    /// <param name="timeout">超时时间</param>
    /// <returns>返回Ping的响应结果</returns>
    public static async Task<PingReply> CheckPingAsync(string host, int timeout) {
        using Ping ping = new();
        return await ping.SendPingAsync(host, timeout);
    }

    /// <summary>
    /// 异步测试到指定主机名或地址的网络延迟
    /// </summary>
    /// <param name="hostNameOrAddress">要ping的主机名或IP地址</param>
    /// <returns>来回通信的时间，以毫秒为单位</returns>
    public static async Task<long> PingHostAsync(string hostNameOrAddress) {
        using var ping = new Ping();
        var reply = await ping.SendPingAsync(hostNameOrAddress);
        return reply.RoundtripTime;
    }

    /// <summary>
    /// 获取指定网络接口的流量统计信息
    /// </summary>
    /// <param name="ni">要获取统计信息的网络接口</param>
    /// <returns>包含发送和接收的字节数的元组</returns>
    public static (long BytesSent, long BytesReceived) GetNetworkInterfaceStatistics(NetworkInterface ni) {
        var stats = ni.GetIPv4Statistics();
        return (stats.BytesSent, stats.BytesReceived);
    }

    /// <summary>
    /// 监控指定网络接口的流量统计信息，并定时更新
    /// </summary>
    /// <param name="ni">要监控的网络接口</param>
    /// <param name="onUpdate">每次更新时调用的回调函数，传递发送和接收的字节数</param>        
    public static async Task MonitorNetworkInterfaceAsync(NetworkInterface ni, Action<long, long> onUpdate) {
        while (true) {
            var stats = ni.GetIPv4Statistics();
            onUpdate(stats.BytesSent, stats.BytesReceived);
            await Task.Delay(1000);
        }
    }

    /// <summary>
    /// 获取当前设备的主机名
    /// </summary>
    /// <returns>设备的主机名字符串</returns>
    public static string GetHostName() {
        return Dns.GetHostName();
    }

    /// <summary>
    /// 获取所有活跃的网络接口
    /// </summary>
    /// <returns>活跃的网络接口列表</returns>
    public static IEnumerable<NetworkInterface> GetActiveNetworkInterfaces() {
        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(ni => ni.OperationalStatus == OperationalStatus.Up);
    }

    /// <summary>
    /// 异步测量解析指定域名所需的时间
    /// </summary>
    /// <param name="hostname">要解析的域名</param>
    /// <returns>DNS 解析所需的时间，以毫秒为单位</returns>
    public static async Task<long> MeasureDnsResolveTimeAsync(string hostname) {
        var stopwatch = Stopwatch.StartNew();
        _ = await Dns.GetHostAddressesAsync(hostname);
        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }

    /// <summary>
    /// 获取所有网络接口的名称和描述
    /// </summary>
    /// <returns>包含网络接口名称和描述的字典</returns>
    public static Dictionary<string, string> GetAllNetworkInterfacesNamesAndDescriptions() {
        return NetworkInterface.GetAllNetworkInterfaces()
            .ToDictionary(ni => ni.Name, ni => ni.Description);
    }

    /// <summary>
    /// 获取当前连接的无线网络的 SSID
    /// </summary>
    /// <returns>当前无线网络的 SSID，如果不是无线网络则返回空字符串</returns>
    public static string GetCurrentWifiSSID() {
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (var ni in networkInterfaces) {
            if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && ni.OperationalStatus == OperationalStatus.Up) {
                var ssid = ni.GetIPProperties().DnsSuffix;
                return !string.IsNullOrEmpty(ssid) ? ssid : "Unknown SSID";
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 获取指定网络接口的最大传输速率。
    /// </summary>
    /// <param name="ni">网络接口</param>
    /// <returns>网络接口的最大速率，以 Mbps 为单位</returns>
    public static long GetNetworkInterfaceSpeed(NetworkInterface ni) {
        return ni.Speed / 1000000;
    }

    /// <summary>
    /// 监听网络状态变化事件
    /// </summary>
    /// <param name="onNetworkChanged">当网络状态发生变化时调用的回调函数</param>
    public static void ListenToNetworkChanges(Action onNetworkChanged) {
        NetworkChange.NetworkAddressChanged += (sender, e) => {
            onNetworkChanged();
        };
    }

    /// <summary>
    /// 获取网络接口及其IP地址的元组序列
    /// </summary>
    /// <param name="ni">网络接口</param>
    /// <returns>网络接口及其IP地址的元组序列</returns>
    private static IEnumerable<(InterfaceType Interface, IEnumerable<string> IPAddresses)> GetInterfaceAndIPs(NetworkInterface ni) {
        var interfaceType = GetInterfaceType(ni.NetworkInterfaceType);
        var ipAddresses = GetIPv4Addresses(ni);
        return [(Interface: interfaceType, IPAddresses: ipAddresses)];
    }

    /// <summary>
    /// 根据网络接口类型获取对应的枚举值
    /// </summary>
    /// <param name="type">网络接口类型</param>
    /// <returns>网络接口枚举值</returns>
    private static InterfaceType GetInterfaceType(NetworkInterfaceType type) {
        return type switch {
            NetworkInterfaceType.Ethernet => InterfaceType.Ethernet,
            NetworkInterfaceType.Wireless80211 => InterfaceType.Wireless,
            _ => InterfaceType.Other
        };
    }

    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="recipient">接收人的电子邮件地址</param>
    /// <param name="body">邮件内容</param>
    /// <param name="subject">邮件主题</param>        
    public static void SendEmail(string recipient, string body, string? subject) {
        var mailto = new Uri($"mailto:{recipient}?subject={subject ?? ""}&body={body}");
        _ = Process.Start(new ProcessStartInfo(mailto.ToString()) { UseShellExecute = true });
    }

    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="recipient">接收人的电子邮件地址</param>
    public static void SendEmail(string recipient) {
        var mailto = new Uri($"mailto:{recipient}");
        _ = Process.Start(new ProcessStartInfo(mailto.ToString()) { UseShellExecute = true });
    }

    /// <summary>
    /// 检测谷歌地图是否可访问
    /// </summary>
    /// <returns>谷歌地图是否可访问</returns>
    public static async Task<bool> IsGoogleMapsAccessibleAsync() {
        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(5);
        try {
            var response = await httpClient.GetAsync("https://www.google.com/maps/@?api=1");
            return response.IsSuccessStatusCode;
        } catch {
            return false;
        }
    }

    /// <summary>
    /// 使用默认浏览器打开指定URL
    /// </summary>
    /// <param name="url">要打开的URL</param>
    public static void OpenUrl(string url) {
        _ = Process.Start(new ProcessStartInfo {
            FileName = url,
            UseShellExecute = true
        });
    }
}
