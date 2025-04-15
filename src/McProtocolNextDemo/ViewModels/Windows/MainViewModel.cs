// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using CommunityToolkit.Mvvm.ComponentModel;
using McProtocolNext;
using McProtocolNextDemo.Abstractions;
using McProtocolNextDemo.Commands;
using McProtocolNextDemo.Common;
using McProtocolNextDemo.Controls.Notifications;
using McProtocolNextDemo.Helpers;
using McProtocolNextDemo.Models;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Threading;

namespace McProtocolNextDemo.ViewModels.Windows;

/// <summary>
/// 主窗口视图模型
/// </summary>
public partial class MainViewModel : ViewModelBase, IDisposable {
    private readonly IMcProtocol _mcProtocol;
    private readonly ITipsNotificationService _tipsNotification;

    #region 私有字段

    private bool _disposed;
    private readonly ConcurrentQueue<InstantMessageModel> _messageQueue = new();
    private DispatcherTimer? _connectTimer;
    private DateTime _startTime;
    private readonly DispatcherTimer _messageTimer;
    private const int MESSAGE_PROCESSING_INTERVAL = 1000;
    private const int SHOW_MAX_COUNT = 1024;
    private Timer? _heartbeatTimer;
    private readonly Stopwatch _stopwatch = new();
    private bool _heartbeatSignal;

    #endregion

    #region 属性

    [ObservableProperty]
    private bool _isConnect;

    [ObservableProperty]
    private string _versionInformation = ApplicationInformation.VersionInformation;

    [ObservableProperty]
    private string _copyrightInformation = ApplicationInformation.COPYRIGHT_INFORMATION;

    [ObservableProperty]
    private RunState _currentState = RunState.Stopped;

    [ObservableProperty]
    private string _taskRuntimeDuration = "Unconnected";

    [ObservableProperty]
    private string _writeDelay = "Unknown";

    [ObservableProperty]
    private ObservableCollection<InstantMessageModel> _instantMessage = [];

    #endregion

    #region 命令

    /// <summary>
    /// 获取关闭窗口命令
    /// </summary>
    public MasRelayCommand CloseWindowCommand { get; }

    /// <summary>
    /// 获取切换状态命令
    /// </summary>
    public MasAsyncRelayCommand StateSwitchCommand { get; }

    /// <summary>
    /// 获取清空内容命令
    /// </summary>
    public MasRelayCommand ClearContentCommand { get; }

    /// <summary>
    /// 获取创建新窗口命令
    /// </summary>
    public MasRelayCommand CreateNewWindowCommand { get; }

    #endregion

    #region 事件

    /// <summary>
    /// 即时信息更新更新事件
    /// </summary>
    public event EventHandler? OnMessageUpdated;

    #endregion

    /// <summary>
    /// 构造函数，初始化 <see cref="MainViewModel"/> 新实例
    /// </summary>
    public MainViewModel(
        IMcProtocol mcProtocol,
        ITipsNotificationService tipsNotification) {
        _mcProtocol = mcProtocol;
        _tipsNotification = tipsNotification;

        _connectTimer = new DispatcherTimer {
            Interval = TimeSpan.FromMinutes(1)
        };
        _connectTimer.Tick += ConnectTimer_Tick;
        _messageTimer = new DispatcherTimer {
            Interval = TimeSpan.FromMilliseconds(MESSAGE_PROCESSING_INTERVAL)
        };
        _messageTimer.Tick += ProcessMessageQueueCallback;
        _messageTimer.Start();

        CloseWindowCommand = new MasRelayCommand(ExecuteCloseWindow);
        StateSwitchCommand = new MasAsyncRelayCommand(ExecuteStateSwitchAsync);
        ClearContentCommand = new MasRelayCommand(_ => InstantMessage.Clear());
        CreateNewWindowCommand = new MasRelayCommand(ExecuteCreateNewWindow);
    }

    #region 私有方法

    private void ExecuteCreateNewWindow(object? parameter) {
        try {
            _ = Process.Start(Environment.ProcessPath!);
        } catch (System.ComponentModel.Win32Exception ex) {
            _tipsNotification.Error(
                title: "Win32 error code",
                message: ex.Message
            );
        } catch (System.Security.SecurityException ex) {
            _tipsNotification.Error(
                title: "Permission Denied",
                message: ex.Message
            );
        } catch (Exception ex) {
            _tipsNotification.Error(
                title: "Unexpected Error",
                message: ex.Message
            );
        }
    }

    private async Task ExecuteStateSwitchAsync(object? parameter) {
        switch (CurrentState) {
            case RunState.Starting:
                _tipsNotification.Warning("StartingPleaseWait");
                return;
            case RunState.Stopped:
                CurrentState = RunState.Starting;
                if (!await PreCommunicationCheckAsync()) {
                    CurrentState = RunState.Stopped;
                    return;
                }

                if (await _mcProtocol.TryReconnectToPlcAsync()) {
                    CurrentState = RunState.Running;
                    EnqueueInstantMessage(InstantMessageHelper.Create("ConnectionSuccessful"));
                } else {
                    CurrentState = RunState.Stopped;
                    EnqueueInstantMessage(InstantMessageHelper.Create("ConnectionFailed", InfoLevel.Warning));
                }

                break;
            case RunState.Running:
                CurrentState = RunState.Stopping;
                StopHeartbeatTimer();
                _mcProtocol.DisconnectFromPlc();
                CurrentState = RunState.Stopped;
                EnqueueInstantMessage(InstantMessageHelper.Create("DisconnectedFromPLC"));
                break;
            case RunState.Stopping:
                _tipsNotification.Warning("StoppingPleaseWait");
                return;
            default:
                _tipsNotification.Error($"Invalid communication status: {CurrentState}");
                break;
        }
    }

    private void ExecuteCloseWindow(object? parameter) {
        if (parameter is not Window windows) {
            return;
        }

        windows.Close();
    }

    private void ProcessMessageQueueCallback(object? sender, EventArgs e) {
        var newMessages = new List<InstantMessageModel>();
        while (_messageQueue.TryDequeue(out var message)) {
            newMessages.Add(message);
        }

        if (newMessages.Count <= 0) {
            return;
        }

        foreach (var message in newMessages) {
            InstantMessage.Add(message);
        }

        while (InstantMessage.Count > SHOW_MAX_COUNT) {
            InstantMessage.RemoveAt(0);
        }

        OnMessageUpdated?.Invoke(null, EventArgs.Empty);
    }

    private void ConnectTimer_Tick(object? sender, EventArgs e) {
        TimeSpan date = DateTime.Now - _startTime;
        TaskRuntimeDuration = $"{date.Hours}h{date.Minutes}m";
    }

    private void UpdateWriteDelay(long time) {
        switch (time) {
            case -1:
                WriteDelay = "已断线";
                return;
            case <= 0:
                WriteDelay = "<1ms";
                return;
            case >= 460:
                WriteDelay = ">460ms";
                return;
            default:
                WriteDelay = $"{time}ms";
                break;
        }
    }

    private void EnqueueInstantMessage(InstantMessageModel message) {
        _messageQueue.Enqueue(message);
    }

    private async Task<bool> PreCommunicationCheckAsync() {
        string targetIp = Config.McProtocols.Ip;
        EnqueueInstantMessage(InstantMessageHelper.Create(
            InfoLevel.Info,
            GlobalLanguageManager.GetString("TargetIP"),
            targetIp,
            ", ",
            GlobalLanguageManager.GetString("GettingLocalIPs")));

        bool isPingSuccess = await PingHostAsync(targetIp).ConfigureAwait(false);
        if (isPingSuccess) {
            return true;
        }

        EnqueueInstantMessage(InstantMessageHelper.Create(
            GlobalLanguageManager.GetString("CheckNetwork"),
            InfoLevel.Warning));
        return false;
    }

    private async Task<bool> PingHostAsync(string host) {
        try {
            var allIPs = NetworkHelper.GetAllLocalIPAddresses();
            string message = "";
            foreach (var entry in allIPs) {
                message += GlobalLanguageManager.GetString("InterfaceType") +
                    entry.Key +
                    "，" +
                    GlobalLanguageManager.GetString("IPAddress") +
                    string.Join(", ", entry.Value);
            }

            EnqueueInstantMessage(InstantMessageHelper.Create(message));
            var message1 = GlobalLanguageManager.GetString("Pinging") +
                host +
                "，" +
                GlobalLanguageManager.GetString("CheckingNetworkReachability");
            EnqueueInstantMessage(InstantMessageHelper.Create(message1));

            var reply = await NetworkHelper.CheckPingAsync(host, 5000).ConfigureAwait(false);
            if (reply.Status == IPStatus.Success) {
                return true;
            }

            var message2 = $"{GlobalLanguageManager.GetString("PingFailed")} {host}，{GlobalLanguageManager.GetString("StatusCode")} {reply.Status}";
            EnqueueInstantMessage(InstantMessageHelper.Create(message2, InfoLevel.Error));
            return false;
        } catch (Exception ex) {
            var message3 = GlobalLanguageManager.GetString("PingException") + ex.Message;
            EnqueueInstantMessage(InstantMessageHelper.Create(message3, InfoLevel.Error));
            return false;
        }
    }

    private void StartupHeartbeatTimer() {
        int heartbeatIntervalInSeconds = Config.McProtocols.HeartbeatInterval;
        int heartbeatIntervalInMilliseconds = heartbeatIntervalInSeconds * 1000;

        _heartbeatTimer = new Timer(
            HeartbeatCallback,
            state: null,
            dueTime: 0,
            period: heartbeatIntervalInMilliseconds);
    }

    private void StopHeartbeatTimer() {
        _ = (_heartbeatTimer?.Change(Timeout.Infinite, 0));
        _heartbeatTimer?.Dispose();
        _heartbeatTimer = null;
    }

    private async void HeartbeatCallback(object? state) {
        bool[] bools;
        try {
            _stopwatch.Restart();
            _heartbeatSignal = !_heartbeatSignal;
            bools = [_heartbeatSignal];
            if (!_mcProtocol.CheckPlcConnection()) {
                StopHeartbeatTimer();
                CurrentState = RunState.Stopped;
                return;
            }

            await _mcProtocol.WriteBitsAsync(
                "D",
                _mcProtocol.Configuration.WriteDataAdr,
                bools
            ).ConfigureAwait(false);
        } catch (TimeoutException ex) {
            EnqueueInstantMessage(InstantMessageHelper.Create(
                InfoLevel.Error,
                "HeartbeatTimeout",
                ex.Message));
        } catch (Exception ex) {
            EnqueueInstantMessage(InstantMessageHelper.Create(
                InfoLevel.Error,
                "HeartbeatCallbackError",
                ex.Message));
        } finally {
            _stopwatch.Stop();
            UpdateWriteDelay(_stopwatch.ElapsedMilliseconds);
        }
    }

    #endregion

    partial void OnIsConnectChanged(bool value) {
        if (value) {
            StartupHeartbeatTimer();
            _startTime = DateTime.Now;
            _connectTimer?.Start();
        } else {
            WriteDelay = "Unknown";
            _connectTimer?.Stop();
            StopHeartbeatTimer();
        }
    }

    partial void OnCurrentStateChanged(RunState value) {
        if(value == RunState.Running) {
            IsConnect = true;
        } else if (value == RunState.Stopped) {
            IsConnect = false;
        }
    }

    /// <inheritdoc/>
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 处理资源释放
    /// </summary>
    protected virtual void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }

        if (disposing) {
            if (_connectTimer != null) {
                _connectTimer.Stop();
                _connectTimer.Tick -= ConnectTimer_Tick;
                _connectTimer = null;
            }

            _messageTimer.Stop();
            _messageTimer.Tick -= ProcessMessageQueueCallback;
            InstantMessage.Clear();
            _mcProtocol.Dispose();
        }

        _disposed = true;
    }
}
