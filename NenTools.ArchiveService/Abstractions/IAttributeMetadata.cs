// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System.Diagnostics.CodeAnalysis;

namespace NenTools.ArchiveService.Abstractions;

/// <summary>
/// Represents a custom file attribute.
/// </summary>
public interface IAttributeMetadata<T>
{
    /// <summary>
    /// Name of the attribute.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Attribute type.
    /// </summary>
    Type ValueType { get; }

    /// <summary>
    /// Value accessor.
    /// </summary>
    Func<T, object?>? Accessor { get; }

    /// <summary>
    /// Display format type.
    /// </summary>
    AttributeDisplayFormat DisplayFormat { get; }

    /// <summary>
    /// Used when <see cref="DisplayFormat"/> is <see cref="AttributeDisplayFormat.TextCustomFormatter"/>.
    /// </summary>
    Func<T, string?>? Formatter { get; }

    /// <summary>
    /// Whether this attribute is a primary attribute / important.
    /// </summary>
    bool IsPrimary { get; }

    /// <summary>
    /// Header / Friendly name for the attribute.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Description for this attribute.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Child attributes, if any.
    /// </summary>
    IReadOnlyDictionary<string, IAttributeMetadata<T>>? ChildAttributes { get; }

    /// <summary>
    /// Returns an attribute if it exists.
    /// </summary>
    /// <param name="attributeName"></param>
    /// <param name="attribute"></param>
    /// <returns></returns>
    bool TryGetChildAttribute(string attributeName, [NotNullWhen(true)] out IAttributeMetadata<T>? attribute);
}

/// <summary>
/// Attribute display format types.
/// </summary>
public enum AttributeDisplayFormat
{
    /// <summary>
    /// .ToString() will be called on the value.
    /// </summary>
    Default,

    /// <summary>
    /// Value is a file name that should be displayed as such.
    /// </summary>
    FileName,

    /// <summary>
    /// Value (if number) should be shown as a size.
    /// </summary>
    ByteSize,

    /// <summary>
    /// Value should be shown as a hex number.
    /// </summary>
    Hex,

    /// <summary>
    /// Value should be shown as a bool checkbox.
    /// </summary>
    CheckBox,

    /// <summary>
    /// Value should be custom formatted.
    /// </summary>
    TextCustomFormatter,
}