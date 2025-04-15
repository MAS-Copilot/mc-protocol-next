// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

namespace McProtocolNext;

/// <summary>
/// 用于表示一个字符串字段在三菱 PLC 寄存器 D 中的映射
/// </summary>
/// <param name="length">指定字段映射的字符串长度（以字符为单位）</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class MitsubishiStringAttribute(int length) : Attribute {
    /// <summary>
    /// 获取该字段对应的字符串长度（以字符为单位）
    /// </summary>
    public int Length { get; } = length;
}
