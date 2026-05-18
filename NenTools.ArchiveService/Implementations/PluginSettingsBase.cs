// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using NenTools.ArchiveService.Abstractions;

namespace NenTools.ArchiveService.Implementations;

public class PluginSettingsBase : IPluginSettings
{
    protected readonly Dictionary<string, object?> _values = [];
    public IReadOnlyDictionary<string, object?> Values => _values;

    protected readonly List<ArchiveSettingDescriptor> _descriptors = [];
    public IReadOnlyList<ArchiveSettingDescriptor> Descriptors => _descriptors;

    public bool IsComplete => _descriptors
        .Where(d => d.IsRequired)
        .All(d => _values.ContainsKey(d.Name) && _values[d.Name] != null);

    internal void Register(ArchiveSettingDescriptor descriptor)
    {
        _descriptors.Add(descriptor);
    }

    public void SetValue(string key, object? value)
    {
        _values[key] = value;
    }

    public bool TryGetValue(string key, out object? value)
    {
        return Values.TryGetValue(key, out value);
    }

    public bool TryGetValue<T>(string key, out T? value)
    {
        if (Values.TryGetValue(key, out var obj) && obj is T t)
        {
            value = t;
            return true;
        }

        value = default;
        return false;
    }


}
