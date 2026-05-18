// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using Microsoft.Extensions.Logging;

namespace NenTools.ArchiveService.Abstractions;

/// <summary>
/// Game archive plugin, used to implement functionality for a custom, proprietary game archive.
/// </summary>
public interface IGameArchivePlugin
{
    /// <summary>
    /// Plugin name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Author.
    /// </summary>
    string Author { get; }

    /// <summary>
    /// Version.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Website.
    /// </summary>
    string? Website { get; }

    /// <summary>
    /// Supported files/extensions. This is generally only used for filtering, or for display.<br/>
    /// Use <see cref="IsSupported(string)"/> to check if a file can be loaded.
    /// </summary>
    IReadOnlyCollection<ISupportedPluginFileType>? SupportedFileTypes { get; }

    /// <summary>
    /// Plugin capabilities.
    /// </summary>
    IPluginCapabilities Capabilities { get; }

    /// <summary>
    /// All loaded archives (name, archive).
    /// </summary>
    IReadOnlyDictionary<string, IGameArchive> LoadedArchives { get; }

    /// <summary>
    /// Opens an archive from the specified local path.<br/>
    /// May be null if the user canceled load (i.e: if required settings were not provided).
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parameters"></param>
    /// <returns>Archive handle.</returns>
    IGameArchive? OpenArchive(string path, ArchiveLoadParameters? parameters = default);

    /// <summary>
    /// Settings.
    /// </summary>
    IPluginSettings Settings { get; }

    /// <summary>
    /// Initializes the plugin (plugin-specific initialization logic, archive setting descriptors, etc..)
    /// </summary>
    void Initialize();

    /// <summary>
    /// Sets the logger factory for this plugin.
    /// </summary>
    /// <param name="loggerFactory"></param>
    void SetLoggerFactory(ILoggerFactory loggerFactory) { }

    /// <summary>
    /// Closes an archive using the specified name.
    /// </summary>
    /// <param name="name"></param>
    void CloseArchive(string name);

    /// <summary>
    /// Closes an archive using the specified archive handle.
    /// </summary>
    /// <param name="archive"></param>
    void CloseArchive(IGameArchive archive);

    /// <summary>
    /// Returns whether this plugin supports the specified file (without inspecting its contents).
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    bool IsSupported(string path);

    /// <summary>
    /// Returns whether this plugin supports the specified file (by inspecting its contents).
    /// </summary>
    bool IsSupported(Stream stream);

    /// <summary>
    /// Loads all archives within the specified folder path.<br/>
    /// May be null if the user canceled load (i.e: if required settings were not provided).
    /// </summary>
    /// <param name="folderPath"></param>
    /// <param name="parameters"></param>
    /// <returns>List of all archives loaded.</returns>
    IReadOnlyList<IGameArchive>? LoadArchivesFromFolder(string folderPath, ArchiveLoadParameters? parameters = null);

    /// <summary>
    /// Unloads all archives.
    /// </summary>
    void UnloadAll();

    /// <summary>
    /// Gets a map describing all file attributes for <see cref="IGameArchiveFile.AdditionalProperties"/>.
    /// </summary>
    /// <returns></returns>
    IReadOnlyDictionary<string, IAttributeMetadata<IGameArchiveFile>> GetFileAttributes();

    /// <summary>
    /// Returns a merged file system tree of all the currently loaded archives for this plugin.
    /// </summary>
    /// <returns></returns>
    IFileSystemTree GetMergedTree();

    /// <summary>
    /// Extracts a file handle to the specified output stream.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="outputStream"></param>
    void ExtractFile(IGameArchiveFile file, Stream outputStream);

    /// <summary>
    /// Extracts a file handle to the specified output stream.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="outputStream"></param>
    Task ExtractFileAsync(IGameArchiveFile file, Stream outputStream, CancellationToken ct = default);

    /// <summary>
    /// Fired on archive load.
    /// </summary>
    event OnArchiveOpenedDelegate ArchiveOpened;

    /// <summary>
    /// Fired on archive close/disposal.
    /// </summary>
    event OnArchiveClosedDelegate ArchiveClosed;
}

public class ArchiveSettingDescriptor
{
    /// <summary>
    /// Name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Display name.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Setting description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Type of the value.
    /// </summary>
    public required Type ValueType { get; init; }

    /// <summary>
    /// Default value.
    /// </summary>
    public object? DefaultValue { get; init; }

    /// <summary>
    /// This setting is required to open an archive.
    /// </summary>
    public bool IsRequired { get; init; }

    /// <summary>
    /// Child descriptors, if this is a group.
    /// </summary>
    public IReadOnlyList<ArchiveSettingDescriptor>? Children { get; init; }

    /// <summary>
    /// Whether this descriptor is a group rather than a value.
    /// </summary>
    public bool IsGroup => Children is { Count: > 0 };
}

public delegate void OnArchiveOpenedDelegate(IGameArchive archive);
public delegate void OnArchiveClosedDelegate(IGameArchive archive);

public class ArchiveLoadParameters
{
    /// <summary>
    /// Callback event for required settings before opening the archive. <br/>
    /// Returns a bool as to whether it was canceled.
    /// </summary>
    public Func<IPluginSettings, bool>? OnSettingsRequired { get; set; }
}
