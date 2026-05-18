// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using NenTools.ArchiveService.Abstractions;

namespace NenTools.ArchiveService.Implementations;

/// <summary>
/// Base game archive file.
/// </summary>
public abstract class GameArchiveBase : IGameArchive
{
    public required string Name { get; set; }
    public required string Path { get; set; }
    public string? Description { get; set; }
    public abstract bool SupportsAsync { get; }
    public IGameArchivePlugin SourcePlugin { get; internal set; } = null!;

    public Dictionary<string, object?> AdditionalProperties = [];
    IReadOnlyDictionary<string, object?> IGameArchive.AdditionalProperties => AdditionalProperties;

    public abstract void Dispose();
    public abstract ValueTask DisposeAsync();
    public abstract void ExtractFile(IGameArchiveFile file, Stream outputStream);
    public abstract Task ExtractFileAsync(IGameArchiveFile file, Stream outputStream, CancellationToken ct = default);
    public abstract IReadOnlyDictionary<string, IAttributeMetadata<IGameArchive>> GetAttributes();
    public abstract IFileSystemTree GetTree();
    public abstract Task<IFileSystemTree> GetTreeAsync(CancellationToken ct = default);
}
