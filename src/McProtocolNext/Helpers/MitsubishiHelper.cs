// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace McProtocolNext;

/// <summary>
/// Helper class for Mitsubishi
/// </summary>
internal static class MitsubishiHelper {
    private static readonly Dictionary<Type, Func<byte[], object>> _typeConversionMap = new() {
        { typeof(double), (bytes) => BitConverter.ToDouble(bytes, 0) },
        { typeof(float), (bytes) => BitConverter.ToSingle(bytes, 0) },
        { typeof(short), (bytes) => BitConverter.ToInt16(bytes, 0) },
        { typeof(int), (bytes) => BitConverter.ToInt32(bytes, 0) },
        { typeof(long), (bytes) => BitConverter.ToInt64(bytes, 0) },
        { typeof(ushort), (bytes) => BitConverter.ToUInt16(bytes, 0) },
        { typeof(uint), (bytes) => BitConverter.ToUInt32(bytes, 0) },
        { typeof(ulong), (bytes) => BitConverter.ToUInt64(bytes, 0) },
        { typeof(byte), (bytes) => bytes[0] }
    };

    /// <summary>
    /// 将字符串解析为McFrame枚举
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public static McFrame ParseMcFrame(string frameType) {
        return frameType switch {
            "MC1E" => McFrame.MC1E,
            "MC3E" => McFrame.MC3E,
            "MC4E" => McFrame.MC4E,
            _ => throw new ArgumentException($"Unsupported frame type: {frameType}"),
        };
    }

    /// <summary>
    /// 将字符串解析为PlcDeviceType枚举
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public static PlcDeviceType ParsePlcDeviceType(string deviceType) {
        return deviceType.ToUpper() switch {
            "M" => PlcDeviceType.M,
            "SM" => PlcDeviceType.SM,
            "L" => PlcDeviceType.L,
            "F" => PlcDeviceType.F,
            "V" => PlcDeviceType.V,
            "S" => PlcDeviceType.S,
            "X" => PlcDeviceType.X,
            "Y" => PlcDeviceType.Y,
            "B" => PlcDeviceType.B,
            "SB" => PlcDeviceType.SB,
            "DX" => PlcDeviceType.DX,
            "DY" => PlcDeviceType.DY,
            "D" => PlcDeviceType.D,
            "SD" => PlcDeviceType.SD,
            "R" => PlcDeviceType.R,
            "ZR" => PlcDeviceType.ZR,
            "W" => PlcDeviceType.W,
            "SW" => PlcDeviceType.SW,
            "TC" => PlcDeviceType.TC,
            "TS" => PlcDeviceType.TS,
            "TN" => PlcDeviceType.TN,
            "CC" => PlcDeviceType.CC,
            "CS" => PlcDeviceType.CS,
            "CN" => PlcDeviceType.CN,
            "SC" => PlcDeviceType.SC,
            "SS" => PlcDeviceType.SS,
            "SN" => PlcDeviceType.SN,
            "Z" => PlcDeviceType.Z,
            "TT" => PlcDeviceType.TT,
            "TM" => PlcDeviceType.TM,
            "CT" => PlcDeviceType.CT,
            "CM" => PlcDeviceType.CM,
            "A" => PlcDeviceType.A,
            _ => throw new ArgumentException($"Unsupported device type: {deviceType}"),
        };
    }

    /// <summary>
    /// 获取指定类型的字节长度
    /// </summary>
    /// <param name="type">要获取字节长度的类型</param>
    /// <returns>类型对应的字节长度</returns>
    /// <exception cref="NotSupportedException"></exception>
    public static int GetTypeByteLength(Type type) {
        if (type == typeof(short)) {
            return 2;
        }

        if (type == typeof(int)) {
            return 4;
        }

        if (type == typeof(float)) {
            return 4;
        }

        if (type == typeof(double)) {
            return 8;
        }

        throw new NotSupportedException($"Type {type.Name} is not supported.");
    }

    /// <summary>
    /// 将字节数组转换为指定目标类型的值
    /// </summary>
    /// <param name="bytes">包含要转换的数据的字节数组</param>
    /// <param name="targetType">要转换的目标类型</param>
    /// <returns>从字节数组转换的指定目标类型的值</returns>
    /// <exception cref="NotSupportedException"></exception>
    public static object GetValueFromBytes(byte[] bytes, Type targetType) {
        if (_typeConversionMap.TryGetValue(targetType, out var convertFunc)) {
            return convertFunc(bytes);
        }

        throw new NotSupportedException($"不支持的类型: {targetType.Name}");
    }

    /// <summary>
    /// 将指定值转换为字节数组
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>转换后的字节数组</returns>
    /// <exception cref="NotSupportedException"></exception>
    public static byte[] GetBytes(object value) {
        if (value is short s) {
            return BitConverter.GetBytes(s);
        }

        if (value is int i) {
            return BitConverter.GetBytes(i);
        }

        if (value is float f) {
            return BitConverter.GetBytes(f);
        }

        if (value is double b) {
            return BitConverter.GetBytes(b);
        }

        throw new NotSupportedException($"Type {value.GetType().Name} is not supported.");
    }

    /// <summary>
    /// 获取结构体大小
    /// </summary>
    /// <param name="structType">结构体类型</param>
    /// <returns>计算得到的字节大小</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static int GetStructSize(Type structType) {
        double numBytes = 0.0;

        var fields = structType.GetFields();

        foreach (var file in fields) {
            if (file.FieldType.Name == "Boolean") {
                numBytes += 0.125;
            }
        }

        numBytes = Math.Ceiling(numBytes / 2) * 2;

        foreach (var field in fields) {
            switch (field.FieldType.Name) {
                case "Boolean":
                    break;
                case "Byte":
                    numBytes = Math.Ceiling(numBytes);
                    numBytes++;
                    break;
                case "Int16":
                case "UInt16":
                    numBytes = Math.Ceiling(numBytes);
                    numBytes += 2;
                    break;
                case "Int32":
                case "UInt32":
                    numBytes = Math.Ceiling(numBytes);
                    numBytes += 4;
                    break;
                case "Single":
                    numBytes = Math.Ceiling(numBytes);
                    numBytes += 4;
                    break;
                case "Double":
                    numBytes = Math.Ceiling(numBytes);
                    numBytes += 8;
                    break;
                case "String":
                    var attribute = field.GetCustomAttributes<MitsubishiStringAttribute>().SingleOrDefault()
                        ?? throw new InvalidOperationException($"{structType.FullName} 中的字符串字段 {field.Name} 缺少 MitsubishiStringAttribute 特性");
                    numBytes = Math.Ceiling(numBytes);
                    numBytes += attribute.Length;
                    break;
                default:
                    numBytes += GetStructSize(field.FieldType);
                    break;
            }
        }

        return (int)Math.Ceiling(numBytes);
    }

    /// <summary>
    /// 将字节数组转换为结构体对象
    /// </summary>
    /// <param name="bytes">表示结构体的字节数组</param>
    /// <param name="structType">要转换的结构体类型</param>
    /// <returns>结构体对象</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static object BytesToStruct(byte[] bytes, Type structType) {
        object structValue = Activator.CreateInstance(structType)!;
        var structFields = structType.GetFields();
        double bitPos = 0.0;

        foreach (var field in structFields) {
            if (field.FieldType.Name == "Boolean") {
                bitPos = HandleBooleanFromBytes(field, structValue, bytes, bitPos);
            }
        }

        if (bitPos / 2 != 0) {
            bitPos = Math.Ceiling(bitPos / 2) * 2;
        }

        foreach (var field in structFields) {
            if (field.FieldType.Name == "Boolean") {
                continue;
            }

            bitPos = HandleFieldFromBytes(field, structValue, bytes, bitPos);
        }

        return structValue;
    }

    /// <summary>
    /// 将字节数组转换为结构体对象
    /// </summary>
    /// <typeparam name="T">目标结构体类型</typeparam>
    /// <param name="bytes">包含结构体数据的字节数组</param>
    /// <returns>转换后的结构体对象</returns>
    public static T BytesToStruct<T>(byte[] bytes) where T : struct {
        object structValue = BytesToStruct(bytes, typeof(T));
        return (T)structValue;
    }

    /// <summary>
    /// 将结构体对象转换为字节数组
    /// </summary>
    /// <param name="structValue">要转换的结构体对象</param>
    /// <returns>表示结构体的字节数组</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static byte[] StructToBytes(object structValue) {
        var structFields = structValue.GetType().GetFields();
        int numBytes = GetStructSize(structValue.GetType());
        byte[] bytes = new byte[numBytes];
        double bitPos = 0.0;

        foreach (var field in structFields) {
            if (field.FieldType.Name == "Boolean") {
                HandleBooleanField(field, structValue, ref bytes, ref bitPos);
            }
        }

        if (bitPos / 2 != 0) {
            bitPos = Math.Ceiling(bitPos / 2) * 2;
        }

        foreach (FieldInfo field in structFields) {
            switch (field.FieldType.Name) {
                case "Boolean":
                    break;
                case "Byte":
                    HandleByteField(field, structValue, ref bytes, ref bitPos);
                    break;
                case "Int16":
                case "UInt16":
                    HandleShortField(field, structValue, ref bytes, ref bitPos);
                    break;
                case "Int32":
                case "UInt32":
                    HandleIntField(field, structValue, ref bytes, ref bitPos);
                    break;
                case "Single":
                    HandleFloatField(field, structValue, ref bytes, ref bitPos);
                    break;
                case "Double":
                    HandleDoubleField(field, structValue, ref bytes, ref bitPos);
                    break;
                case "String":
                    HandleStringField(field, structValue, ref bytes, ref bitPos);
                    break;
                default:
                    HandleNestedStructField(field, structValue, ref bytes, ref bitPos);
                    break;
            }
        }

        return bytes;
    }

    /// <summary>
    /// 将结构体字段的值复制到字节数组中
    /// </summary>
    /// <param name="structValue">包含字段值的结构体对象</param>
    /// <param name="bytes">目标字节数组</param>
    /// <param name="offset">在目标字节数组中的起始偏移量</param>
    /// <param name="size">要复制的字节数</param>
    public static void CopyStructToBytes(object? structValue, byte[] bytes, int offset, int size) {
        if (structValue == null) {
            Array.Clear(bytes, offset, size);
            return;
        }

        byte[] fieldBytes = new byte[size];
        GCHandle handle = GCHandle.Alloc(fieldBytes, GCHandleType.Pinned);
        try {
            Marshal.StructureToPtr(structValue, handle.AddrOfPinnedObject(), false);
        } finally {
            handle.Free();
        }

        Array.Copy(fieldBytes, 0, bytes, offset, size);
    }

    /// <summary>
    /// 检查响应数据是否错误（即不符合预期）
    /// </summary>
    /// <param name="response">响应数据</param>
    /// <param name="minLength">最小长度</param>
    /// <param name="frameType">协议类型</param>
    /// <returns>如果响应数据不正确，返回 true；否则返回 false</returns>
    public static bool IsIncorrectResponse(McFrame frameType, byte[] response, int minLength) {
        if (response.Length < minLength) {
            return false;
        }

        switch (frameType) {
            case McFrame.MC1E:
                return response.Length < minLength;
            case McFrame.MC3E:
            case McFrame.MC4E:
                var btCount = new[] {
                    response[minLength - 4], response[minLength - 3]
                };
                var btCode = new[] {
                    response[minLength - 2], response[minLength - 1]
                };
                var rsCount = BitConverter.ToUInt16(btCount, 0) - 2;
                var rsCode = BitConverter.ToUInt16(btCode, 0);
                return rsCode == 0 && rsCount != (response.Length - minLength);
            default:
                throw new Exception("Frame type not supported.");
        }
    }

    #region 私有方法

    public static bool IsHexDevice(PlcDeviceType type) {
        return type is PlcDeviceType.X or PlcDeviceType.Y or PlcDeviceType.B or PlcDeviceType.W;
    }

    private static double HandleFieldFromBytes(FieldInfo field, object structValue, byte[] bytes, double bitPos) {
        return field.FieldType.Name switch {
            "Byte" => HandleByteFromBytes(field, structValue, bytes, bitPos),
            "Int16" or "UInt16" => HandleShortFromBytes(field, structValue, bytes, bitPos),
            "Int32" or "UInt32" => HandleIntFromBytes(field, structValue, bytes, bitPos),
            "Single" => HandleFloatFromBytes(field, structValue, bytes, bitPos),
            "Double" => HandleDoubleFromBytes(field, structValue, bytes, bitPos),
            "String" => HandleStringFromBytes(field, structValue, bytes, bitPos),
            _ => HandleNestedStructFromBytes(field, structValue, bytes, bitPos),
        };
    }

    #region 结构体到字节数组

    private static void HandleBooleanField(FieldInfo field, object structValue, ref byte[] bytes, ref double bitPos) {
        int currentBytePos = (int)(bitPos);
        int bitOffset = (int)((bitPos - currentBytePos) * 8);

        bool hasValue = (bool)field.GetValue(structValue)!;

        if (hasValue) {
            bytes[currentBytePos] |= (byte)(1 << bitOffset);
        } else {
            bytes[currentBytePos] &= (byte)~(1 << bitOffset);
        }

        bitPos += 0.125;
    }

    private static void HandleByteField(FieldInfo field, object structValue, ref byte[] bytes, ref double bitPos) {
        int bytePos = (int)Math.Ceiling(bitPos);
        bytes[bytePos] = (byte)field.GetValue(structValue)!;
        bitPos += 1;
    }

    private static void HandleShortField(FieldInfo field, object structValue, ref byte[] bytes, ref double bitPos) {
        int bytePos = (int)Math.Ceiling(bitPos);
        byte[] shortBytes = BitConverter.GetBytes(Convert.ToInt16(field.GetValue(structValue)));

        Array.Copy(shortBytes, 0, bytes, bytePos, 2);
        bitPos += 2;
    }

    private static void HandleIntField(FieldInfo field, object structValue, ref byte[] bytes, ref double bitPos) {
        int bytePos = (int)Math.Ceiling(bitPos);
        byte[] intBytes = BitConverter.GetBytes(Convert.ToInt32(field.GetValue(structValue)));

        Array.Copy(intBytes, 0, bytes, bytePos, 4);
        bitPos += 4;
    }

    private static void HandleFloatField(FieldInfo field, object structValue, ref byte[] bytes, ref double bitPos) {
        int bytePos = (int)Math.Ceiling(bitPos);
        byte[] floatBytes = BitConverter.GetBytes(Convert.ToSingle(field.GetValue(structValue)));

        Array.Copy(floatBytes, 0, bytes, bytePos, 4);
        bitPos += 4;
    }

    private static void HandleDoubleField(FieldInfo field, object structValue, ref byte[] bytes, ref double bitPos) {
        int bytePos = (int)Math.Ceiling(bitPos);
        byte[] doubleBytes = BitConverter.GetBytes(Convert.ToDouble(field.GetValue(structValue)));

        Array.Copy(doubleBytes, 0, bytes, bytePos, 8);
        bitPos += 8;
    }

    private static void HandleStringField(FieldInfo field, object structValue, ref byte[] bytes, ref double bitPos) {
        var stringAttribute = field.GetCustomAttribute<MitsubishiStringAttribute>()
            ?? throw new InvalidOperationException($"字符串字段 {field.Name} 缺少 MitsubishiStringAttribute 特性");

        string fieldValue = field.GetValue(structValue) as string ?? string.Empty;
        byte[] stringBytes = Encoding.ASCII.GetBytes(fieldValue.PadRight(stringAttribute.Length, '\0'));
        int bytePos = (int)Math.Ceiling(bitPos);
        Array.Copy(stringBytes, 0, bytes, bytePos, stringBytes.Length);
        bitPos += stringBytes.Length;
    }

    private static void HandleNestedStructField(FieldInfo field, object structValue, ref byte[] bytes, ref double bitPos) {
        int bytePos = (int)Math.Ceiling(bitPos);
        byte[] nestedStructBytes = StructToBytes(field.GetValue(structValue)!);
        Array.Copy(nestedStructBytes, 0, bytes, bytePos, nestedStructBytes.Length);
        bitPos += nestedStructBytes.Length;
    }

    #endregion

    #region 字节数组到结构体

    private static double HandleBooleanFromBytes(FieldInfo field, object structValue, byte[] bytes, double bitPos) {
        int currentBytePos = (int)(bitPos);
        int bitOffset = (int)((bitPos - currentBytePos) * 8);

        bool hasValue = (bytes[currentBytePos] & (byte)(1 << bitOffset)) != 0;
        field.SetValue(structValue, hasValue);

        return bitPos + 0.125;
    }

    private static double HandleByteFromBytes(FieldInfo field, object structValue, byte[] bytes, double bitPos) {
        int bytePos = (int)Math.Ceiling(bitPos);
        field.SetValue(structValue, bytes[bytePos]);
        return bitPos + 1;
    }

    private static double HandleShortFromBytes(FieldInfo field, object structValue, byte[] bytes, double bitPos) {
        int bytePos = (int)Math.Ceiling(bitPos);
        short shortValue = BitConverter.ToInt16(bytes, bytePos);
        field.SetValue(structValue, Convert.ChangeType(shortValue, field.FieldType));
        return bitPos + 2;
    }

    private static double HandleIntFromBytes(FieldInfo field, object structValue, byte[] bytes, double bitPos) {
        int bytePos = (int)Math.Ceiling(bitPos);
        int intValue = BitConverter.ToInt32(bytes, bytePos);
        field.SetValue(structValue, Convert.ChangeType(intValue, field.FieldType));
        return bitPos + 4;
    }

    private static double HandleFloatFromBytes(FieldInfo field, object structValue, byte[] bytes, double bitPos) {
        int bytePos = (int)Math.Ceiling(bitPos);
        float floatValue = BitConverter.ToSingle(bytes, bytePos);
        field.SetValue(structValue, floatValue);
        return bitPos + 4;
    }

    private static double HandleDoubleFromBytes(FieldInfo field, object structValue, byte[] bytes, double bitPos) {
        int bytePos = (int)Math.Ceiling(bitPos);
        double doubleValue = BitConverter.ToDouble(bytes, bytePos);
        field.SetValue(structValue, doubleValue);
        return bitPos + 8;
    }

    private static double HandleStringFromBytes(FieldInfo field, object structValue, byte[] bytes, double bitPos) {
        var stringAttribute = field.GetCustomAttribute<MitsubishiStringAttribute>()
            ?? throw new InvalidOperationException($"字符串字段 {field.Name} 缺少 MitsubishiStringAttribute 特性");

        int bytePos = (int)Math.Ceiling(bitPos);
        string stringValue = Encoding.ASCII.GetString(bytes, bytePos, stringAttribute.Length).TrimEnd('\0');
        field.SetValue(structValue, stringValue);
        return bitPos + stringAttribute.Length;
    }

    private static double HandleNestedStructFromBytes(FieldInfo field, object structValue, byte[] bytes, double bitPos) {
        int bytePos = (int)Math.Ceiling(bitPos);
        int nestedStructSize = GetStructSize(field.FieldType);
        byte[] nestedStructBytes = new byte[nestedStructSize];
        Array.Copy(bytes, bytePos, nestedStructBytes, 0, nestedStructSize);
        object nestedStructValue = BytesToStruct(nestedStructBytes, field.FieldType);
        field.SetValue(structValue, nestedStructValue);
        return bitPos + nestedStructSize;
    }

    #endregion

    #endregion
}
