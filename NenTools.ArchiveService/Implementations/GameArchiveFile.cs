// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using NenTools.ArchiveService.Abstractions;

namespace NenTools.ArchiveService.Implementations;

public class GameArchiveFile : IGameArchiveFile
{
    public string Name { get; init; }
    public string Path { get; init; }
    public ulong Size { get; init; }
    public IGameArchive SourceArchive { get; init; }

    public Dictionary<string, object?> Properties { get; init; } = [];
    public IReadOnlyDictionary<string, object?> AdditionalProperties => Properties;

    public GameArchiveFile(string name, string path, IGameArchive sourceArchive)
    {
        Name = name;
        Path = path;
        SourceArchive = sourceArchive;
    }

    public override string ToString()
    {
        return Name ?? "<N/A>";
    }
}
