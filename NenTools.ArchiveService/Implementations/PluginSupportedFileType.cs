// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using NenTools.ArchiveService.Abstractions;

namespace NenTools.ArchiveService.Implementations;

public class PluginSupportedFileType : ISupportedPluginFileType
{
    public required string Name { get; set; }
    public required string Extension { get; set; }
}
