// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

namespace NenTools.ArchiveService.Abstractions;

/// <summary>
/// (Disposable) Represents a game archive (usually a packed/cooked file). 
/// </summary>
public interface IGameArchive : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Name/File name of the archive.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Path to the archive.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Additional description/caption for the archive.
    /// </summary>
    string? Description { get; set; }

    /// <summary>
    /// Whether this archive supports async operations.
    /// </summary>
    bool SupportsAsync { get; }

    /// <summary>
    /// Gets the file system tree for this archive.
    /// </summary>
    /// <returns></returns>
    IFileSystemTree GetTree();

    /// <summary>
    /// Plugin that loaded this archive.
    /// </summary>
    IGameArchivePlugin SourcePlugin { get; }

    /// <summary>
    /// Gets the file system tree for this archive.
    /// </summary>
    /// <returns></returns>
    Task<IFileSystemTree> GetTreeAsync(CancellationToken ct = default);

    /// <summary>
    /// Extracts a file from the archive to the specified output stream.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="outputStream"></param>
    void ExtractFile(IGameArchiveFile file, Stream outputStream);

    /// <summary>
    /// Extracts a file from the archive to the specified output stream.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="outputStream"></param>
    Task ExtractFileAsync(IGameArchiveFile file, Stream outputStream, CancellationToken ct = default);

    /// <summary>
    /// Gets a map describing all archive attributes.
    /// </summary>
    /// <returns></returns>
    IReadOnlyDictionary<string, IAttributeMetadata<IGameArchive>> GetAttributes();

    /// <summary>
    /// Additional properties for this archive.
    /// </summary>
    IReadOnlyDictionary<string, object?> AdditionalProperties { get; }
}
