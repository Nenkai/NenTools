//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/NenTools
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using NenTools.ArchiveService.Abstractions;

namespace NenTools.ArchiveService.Implementations;

public class PluginSupportedFileType : ISupportedPluginFileType
{
    public required string Name { get; set; }
    public required string Extension { get; set; }
}
