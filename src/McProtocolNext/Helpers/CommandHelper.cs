// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

namespace McProtocolNext;

/// <summary>
/// 命令帮助类，用于构建不同类型的命令
/// </summary>
internal static class CommandHelper {
    /// <summary>
    /// 构建 MC3E 命令
    /// </summary>
    /// <param name="type">设备类型</param>
    /// <param name="address">设备地址</param>
    /// <param name="size">数据块大小</param>
    /// <param name="mcCommand">命令类</param>
    /// <param name="deviceData">可选的数据字节数组</param>
    /// <returns>构建好的命令字节数组和命令长度</returns>
    public static (byte[] command, int length) BuildMc3E(
        PlcDeviceType type,
        int address,
        int size,
        McCommand mcCommand,
        byte[]? deviceData = null) {
        List<byte> data = [
            (byte)address,
            (byte)(address >> 8),
            (byte)(address >> 16),
            (byte)type,
            (byte)size,
            (byte)(size >> 8)
        ];

        uint maincommd = 0x0401;
        if (deviceData != null) {
            data.AddRange(deviceData);
            maincommd = 0x1401;
        }

        byte[] command = mcCommand.SetCommandMC3E(maincommd, 0x0000, [.. data]);

        return (command, 11);
    }

    /// <summary>
    /// 构建 MC4E 命令
    /// </summary>
    /// <param name="type">设备类型</param>
    /// <param name="address">设备地址</param>
    /// <param name="size">数据块大小</param>
    /// <param name="mcCommand">命令类</param>
    /// <param name="deviceData">可选的数据字节数组</param>
    /// <returns>构建好的命令字节数组和命令长度</returns>
    public static (byte[] command, int length) BuildMc4E(
        PlcDeviceType type,
        int address,
        int size,
        McCommand mcCommand,
        byte[]? deviceData = null) {

        List<byte> data = [
            (byte)address,
            (byte)(address >> 8),
            (byte)(address >> 16),
            (byte)type,
            (byte)size,
            (byte)(size >> 8),
        ];

        uint maincommd = 0x0401;
        if (deviceData != null) {
            data.AddRange(deviceData);
            maincommd = 0x1401;
        }

        return (mcCommand.SetCommandMC4E(maincommd, 0x0000, [.. data]), 15);
    }

    /// <summary>
    /// 构建 MC1E 命令
    /// </summary>
    /// <param name="address">设备地址</param>
    /// <param name="size">数据块大小</param>
    /// <param name="mcCommand">命令类</param>
    /// <param name="deviceData">可选的数据字节数组</param>
    /// <returns>构建好的命令字节数组和命令长度</returns>
    public static (byte[] command, int length) BuildMc1E(
        int address,
        int size,
        McCommand mcCommand,
        byte[]? deviceData = null) {

        List<byte> data = [
            (byte)address,
            (byte)(address >> 8),
            (byte)(address >> 16),
            (byte)(address >> 24),
            0x20,
            0x44,
            (byte)size,
            0x00,
        ];

        if (deviceData != null) {
            data.AddRange(deviceData);
        }

        return (mcCommand.SetCommandMC1E(0x03, [.. data]), 2);
    }
}
