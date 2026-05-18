// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

namespace NenTools.ArchiveService.Abstractions;

/// <summary>
/// Represents a file or directory within an archive.
/// </summary>
public interface IGameArchiveEntry
{
    /// <summary>
    /// Name of the entry.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Path (within the file system) for the entry in the archive.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Source archive for this file. <br/>
    /// May be null (for root folders of merged archives, for example).
    /// </summary>
    IGameArchive? SourceArchive { get; }
}
