// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System.Diagnostics.CodeAnalysis;

using NenTools.ArchiveService.Abstractions;

namespace NenTools.ArchiveService.Implementations;

public class GameArchiveFolder : IGameArchiveFolder
{
    public string Name { get; init; }
    public string Path { get; init; }
    public IGameArchive SourceArchive { get; init; }
    public IGameArchiveFolder? Parent { get; init; }

    public Dictionary<string, IGameArchiveEntry> Children { get; set; } = [];
    IReadOnlyDictionary<string, IGameArchiveEntry> IGameArchiveFolder.Children => Children;

    public GameArchiveFolder(string name, string path, IGameArchive sourceArchive, IGameArchiveFolder? parent = null)
    {
        Name = name;
        Path = path;
        SourceArchive = sourceArchive;
        Parent = parent;
    }

    public bool TryGetEntry(ReadOnlySpan<char> part, [NotNullWhen(true)] out IGameArchiveEntry? outNode)
    {
        outNode = null;

        Dictionary<string, IGameArchiveEntry>.AlternateLookup<ReadOnlySpan<char>> lookup = Children.GetAlternateLookup<ReadOnlySpan<char>>();
        if (lookup.TryGetValue(part, out IGameArchiveEntry? node))
        {
            outNode = node;
            return true;
        }

        return false;
    }

    public void SortChildren()
    {
        if (Children.Count == 0)
            return;

        Children = Children
            .OrderBy(e => e.Value is IGameArchiveFolder ? 0 : 1)
            .ThenBy(e => e.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary();

        foreach (var child in Children.Values)
        {
            if (child is GameArchiveFolder sub)
                sub.SortChildren();
        }
    }

    public int GetFileCount()
    {
        return Children.Count;
    }
}
