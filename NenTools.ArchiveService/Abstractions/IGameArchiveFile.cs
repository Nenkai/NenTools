//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/NenTools
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

namespace NenTools.ArchiveService.Abstractions;

public interface IGameArchiveFile : IGameArchiveEntry
{
    /// <summary>
    /// Size of the file, when extracted.
    /// </summary>
    ulong Size { get; }

    /// <summary>
    /// Additional properties for this file, keyed by attribute name.<br/>
    /// Values should correspond to the schema described by <see cref="IGameArchive.GetAttributes"/>.
    /// </summary>
    IReadOnlyDictionary<string, object?> AdditionalProperties { get; }
}
