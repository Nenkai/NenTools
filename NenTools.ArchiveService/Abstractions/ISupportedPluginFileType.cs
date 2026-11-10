//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/NenTools
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

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
