// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using Microsoft.Extensions.Logging;

using NenTools.ArchiveService.Abstractions;

namespace NenTools.ArchiveService.Implementations;

/// <summary>
/// Base abstract archive plugin.
/// </summary>
public abstract class GameArchivePluginBase : IGameArchivePlugin
{
    public abstract string Name { get; }
    public abstract string Author { get; }
    public abstract string Version { get; }
    public abstract string? Website { get; }
    public abstract IReadOnlyCollection<ISupportedPluginFileType>? SupportedFileTypes { get; }

    public virtual IPluginCapabilities Capabilities { get; } = new PluginCapabilities();
    public virtual IPluginSettings Settings { get; } = new PluginSettingsBase();

    protected readonly Dictionary<string, IGameArchive> _loadedArchives = [];
    public virtual IReadOnlyDictionary<string, IGameArchive> LoadedArchives => _loadedArchives;

    public event OnArchiveOpenedDelegate? ArchiveOpened;
    public event OnArchiveClosedDelegate? ArchiveClosed;

    protected ILogger? Logger { get; private set; }

    protected GameArchivePluginBase()
    {
        
    }

    public virtual void SetLoggerFactory(ILoggerFactory loggerFactory)
        => Logger = loggerFactory.CreateLogger(GetType());

    /// <summary>
    /// Loads an archive and adds it to <see cref="LoadedArchives"/>.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public virtual IGameArchive? OpenArchive(string path, ArchiveLoadParameters? parameters = default)
    {
        if (!Settings.IsComplete && Settings.Descriptors.Any(e => e.IsRequired))
        {
            if (parameters?.OnSettingsRequired is null)
                throw new InvalidOperationException("Archive settings are required but no handler (parameters.OnSettingsRequired) was provided.");

            bool confirmed = parameters.OnSettingsRequired.Invoke(Settings);
            if (!confirmed)
                return null;

            foreach (var settingDescriptor in Settings.Descriptors)
            {
                if (settingDescriptor.IsRequired)
                {
                    if (!Settings.Values.TryGetValue(settingDescriptor.Name, out object? value) || value is null)
                        throw new InvalidOperationException($"Required archive setting '{settingDescriptor.Name}' was not provided or is null.");
                }
            }
        }

        var archive = OpenArchiveCore(path, parameters);
        if (archive is null)
            return null;

        if (archive is GameArchiveBase archiveBase)
            archiveBase.SourcePlugin = this;

        _loadedArchives[archive.Name] = archive;
        ArchiveOpened?.Invoke(archive);
        return archive;
    }

    /// <summary>
    /// Loads all supported files from the specified folder path.<br/>
    /// By default, files to load on that folder depends on <see cref="IsSupported(string)"/>.
    /// </summary>
    /// <param name="folderPath"></param>
    /// <param name="parameters"></param>
    /// <returns>List of all archives loaded.</returns>
    public virtual IReadOnlyList<IGameArchive>? LoadArchivesFromFolder(string folderPath, ArchiveLoadParameters? parameters = null)
    {
        var archives = new List<IGameArchive>();
        foreach (var path in Directory.EnumerateFiles(folderPath))
        {
            if (IsSupported(path))
            {
                var archive = OpenArchive(path, parameters);
                if (archive is null)
                    return null;

                archives.Add(archive);
            }
        }

        return archives;
    }

    /// <summary>
    /// Registers a new configuration value descriptor.
    /// </summary>
    /// <param name="descriptor"></param>
    protected void RegisterSetting(ArchiveSettingDescriptor descriptor)
    {
        if (Settings is PluginSettingsBase settingsBase)
            settingsBase.Register(descriptor);
    }

    /// <summary>
    /// Core logic for opening an archive.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    protected abstract IGameArchive? OpenArchiveCore(string path, ArchiveLoadParameters? parameters);

    /// <inheritdoc/>
    public virtual void CloseArchive(string name)
    {
        if (_loadedArchives.TryGetValue(name, out var archive))
            CloseArchive(archive);
    }

    /// <summary>
    /// Closes/disposes an archive.
    /// </summary>
    /// <param name="archive">Archive to dispose.</param>
    public virtual void CloseArchive(IGameArchive archive)
    {
        if (_loadedArchives.Remove(archive.Name))
        {
            archive.Dispose();
            ArchiveClosed?.Invoke(archive);
        }
    }

    /// <inheritdoc/>
    public virtual void UnloadAll()
    {
        foreach (var archive in _loadedArchives.Values)
            CloseArchive(archive);

        _loadedArchives.Clear();
    }

    /// <inheritdoc/>
    public virtual IFileSystemTree GetMergedTree()
    {
        List<IFileSystemTree> trees = [];
        foreach (var (name, archive) in _loadedArchives)
            trees.Add(archive.GetTree());

        return FileSystemTree.Merge(trees);
    }

    /// <inheritdoc/>
    public virtual void ExtractFile(IGameArchiveFile file, Stream outputStream)
    {
        if (file.SourceArchive is null)
            throw new InvalidOperationException($"File '{file.Name}' does not have a source archive.");

        file.SourceArchive.ExtractFile(file, outputStream);
    }

    /// <inheritdoc/>
    public virtual async Task ExtractFileAsync(IGameArchiveFile file, Stream outputStream, CancellationToken cancellationToken = default)
    {
        if (file.SourceArchive is null)
            throw new InvalidOperationException($"File '{file.Name}' does not have a source archive.");

        if (!file.SourceArchive.SupportsAsync)
            throw new NotSupportedException($"Archive '{file.SourceArchive.Name}' does not support async extraction.");

        await file.SourceArchive.ExtractFileAsync(file, outputStream, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual void Initialize() { }

    /// <inheritdoc/>
    public abstract bool IsSupported(string path);

    /// <inheritdoc/>
    public abstract bool IsSupported(Stream stream);

    /// <inheritdoc/>
    public abstract IReadOnlyDictionary<string, IAttributeMetadata<IGameArchiveFile>> GetFileAttributes();
}
