// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

namespace NenTools.ArchiveService.Abstractions;

/// <summary>
/// Settings for a plugin.
/// </summary>
public interface IPluginSettings
{
    /// <summary>
    /// Archive settings descriptors (if any).
    /// </summary>
    IReadOnlyList<ArchiveSettingDescriptor> Descriptors { get; }

    /// <summary>
    /// Values.
    /// </summary>
    IReadOnlyDictionary<string, object?> Values { get; }

    /// <summary>
    /// Whether the settings have all required values.
    /// </summary>
    bool IsComplete { get; }

    /// <summary>
    /// Sets a configuration value.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="value">Value.</param>
    void SetValue(string key, object? value);

    bool TryGetValue(string key, out object? value);
    bool TryGetValue<T>(string key, out T? value);
}
