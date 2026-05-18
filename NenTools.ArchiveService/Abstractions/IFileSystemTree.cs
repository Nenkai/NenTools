// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

namespace NenTools.ArchiveService.Abstractions;

/// <summary>
/// Represents a file system tree.
/// </summary>
public interface IFileSystemTree
{
    /// <summary>
    /// The root folder for this tree.
    /// </summary>
    IGameArchiveFolder Root { get; }

    /// <summary>
    /// Returns an archive entry using the specified tree path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns>Archive entry. null if not found.</returns>
    IGameArchiveEntry? GetByPath(string path);
}