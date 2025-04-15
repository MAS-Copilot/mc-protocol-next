// Copyright (c) MAS (厦门威光) Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for details.

using Newtonsoft.Json;
using System.IO;

namespace McProtocolNextDemo.Helpers;

internal class FileHelper {
    public static T DeserializeObjectFromFile<T>(string filePath, JsonSerializerSettings? settings = null) {
        if (string.IsNullOrEmpty(filePath)) {
            throw new ArgumentNullException(nameof(filePath), "File path cannot be null");
        }

        if (!File.Exists(filePath)) {
            throw new FileNotFoundException("The specified file does not exist", filePath);
        }

        settings ??= new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };

        var fileContent = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<T>(fileContent, settings)
            ?? throw new InvalidOperationException("Deserialization returned null");
    }
}
