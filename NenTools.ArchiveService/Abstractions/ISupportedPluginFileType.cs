// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

namespace NenTools.ArchiveService.Abstractions;

public interface ISupportedPluginFileType
{
    /// <summary>
    /// File type name. Example: "My awesome .pack file"
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// ".pack", etc.
    /// </summary>
    public string Extension { get; }
}
