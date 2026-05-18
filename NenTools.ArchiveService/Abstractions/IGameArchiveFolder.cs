// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

namespace NenTools.ArchiveService.Abstractions;

/// <summary>
/// Represents a folder within a game archive.
/// </summary>
public interface IGameArchiveFolder : IGameArchiveEntry
{
    /// <summary>
    /// Parent folder for this folder.
    /// </summary>
    IGameArchiveFolder? Parent { get; }

    /// <summary>
    /// Child entries for this folder.
    /// </summary>
    IReadOnlyDictionary<string, IGameArchiveEntry> Children { get; }

    /// <summary>
    /// Returns the number of files in this folder.
    /// </summary>
    /// <returns></returns>
    int GetFileCount();
}
