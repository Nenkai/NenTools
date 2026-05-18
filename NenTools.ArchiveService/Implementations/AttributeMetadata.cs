// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using NenTools.ArchiveService.Abstractions;

using System.Diagnostics.CodeAnalysis;

namespace NenTools.ArchiveService.Implementations;

public class AttributeMetadata<T> : IAttributeMetadata<T>
{
    public string Name { get; private set; }
    public string DisplayName { get; private set; }
    public Type ValueType { get; private set; }
    public AttributeDisplayFormat DisplayFormat { get; private set; } = AttributeDisplayFormat.Default;
    public bool IsPrimary { get; private set; } = true;
    public Func<T, object?> Accessor { get; private set; }
    public Func<T, string?>? Formatter { get; private set; }
    public string? Description { get; }
    public Dictionary<string, IAttributeMetadata<T>>? ChildAttributes { get; private set; }
    IReadOnlyDictionary<string, IAttributeMetadata<T>>? IAttributeMetadata<T>.ChildAttributes => ChildAttributes;

    public AttributeMetadata(string name, string displayName, Type valueType, AttributeDisplayFormat format,
        Func<T, object?> accessor, bool isPrimary = true, Func<T, string?>? formatter = null)
    {
        Name = name;
        DisplayName = displayName;
        ValueType = valueType;
        DisplayFormat = format;
        IsPrimary = isPrimary;
        Accessor = accessor;
        Formatter = formatter;
    }

    public bool TryGetChildAttribute(string attributeName, [NotNullWhen(true)] out IAttributeMetadata<T>? attribute)
    {
        attribute = null;

        if (ChildAttributes is null)
            return false;

        return ChildAttributes.TryGetValue(attributeName, out attribute);
    }
}

public static class AttributeMetadata
{
    /// <summary>
    /// Creates attribute metadata.
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="name">Name of the attribute.</param>
    /// <param name="displayName">Header / Friendly name for the attribute.</param>
    /// <param name="format">Display format type.</param>
    /// <param name="accessor">Value accessor.</param>
    /// <param name="isPrimary">Whether this attribute is a primary attribute / important.</param>
    /// <param name="formatter">Used when <see cref="format"/> is <see cref="AttributeDisplayFormat.TextCustomFormatter"/>.</param>
    /// <returns></returns>
    public static AttributeMetadata<TType> Create<TType, TValue>(
        string name, string displayName, AttributeDisplayFormat format, Func<TType, TValue?> accessor,  bool isPrimary = true, Func<TType, string?>? formatter = null)
        => new(name, displayName, typeof(TValue), format, elem => accessor(elem), isPrimary, formatter);
}
