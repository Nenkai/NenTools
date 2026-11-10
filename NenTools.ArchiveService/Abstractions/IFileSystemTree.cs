//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/NenTools
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

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