// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using NenTools.ArchiveService.Abstractions;

namespace NenTools.ArchiveService.Implementations;

public class FileSystemTree : IFileSystemTree
{
    public GameArchiveFolder Root { get; }
    IGameArchiveFolder IFileSystemTree.Root => Root;

    private FileSystemTree(IGameArchive sourceArchive)
    {
        Root = new GameArchiveFolder("(root)", string.Empty, sourceArchive);
    }

    public FileSystemTree(GameArchiveFolder root)
    {
        Root = root;
    }

    public static IFileSystemTree Parse(IGameArchive source, IReadOnlyList<IGameArchiveFile> files, IReadOnlyList<IGameArchiveFolder>? folders = null)
    {
        var tree = new FileSystemTree(source);
        if (folders is not null)
            tree.InsertFolders(folders, source);

        tree.InsertFiles(files, source);

        tree.Root.SortChildren();
        return tree;
    }

    private void InsertFolders(IReadOnlyList<IGameArchiveFolder> folders, IGameArchive source)
    {
        foreach (var folder in folders)
        {
            if (string.IsNullOrWhiteSpace(folder.Path))
                continue;

            GameArchiveFolder parent = Root;
            ReadOnlySpan<char> pathSpan = folder.Path.AsSpan();

            foreach (var splitRange in pathSpan.Split('/'))
            {
                ReadOnlySpan<char> part = pathSpan[splitRange];
                if (part.IsEmpty)
                    continue;

                if (!parent.TryGetEntry(part, out IGameArchiveEntry? node))
                {
                    bool isLast = splitRange.End.GetOffset(folder.Path.Length) == folder.Path.Length;
                    if (isLast)
                        node = new GameArchiveFolder(part.ToString(), folder.Path, source, parent);
                    else
                        node = new GameArchiveFolder(part.ToString(), pathSpan.Slice(0, splitRange.End.Value).ToString(), source, parent);
                }

                parent.Children.TryAdd(node.Name, node);

                if (node is not GameArchiveFolder folderNode)
                    break;

                parent = folderNode;
            }
        }
    }

    private void InsertFiles(IReadOnlyList<IGameArchiveFile> files, IGameArchive source)
    {
        foreach (var file in files)
        {
            GameArchiveFolder? parent = Root;
            ReadOnlySpan<char> pathSpan = file.Path.AsSpan();

            foreach (var splitRange in pathSpan.Split('/'))
            {
                ReadOnlySpan<char> part = pathSpan[splitRange];
                if (!parent.TryGetEntry(part, out IGameArchiveEntry? node))
                {
                    bool isLast = splitRange.End.GetOffset(file.Path.Length) == file.Path.Length;
                    if (isLast)
                        node = file;
                    else
                        node = new GameArchiveFolder(part.ToString(), pathSpan.Slice(0, splitRange.End.Value).ToString(), source, parent);
                }

                parent?.Children.TryAdd(node.Name, node);
                parent = node as GameArchiveFolder;
            }
        }
    }

    public static IFileSystemTree Merge(IList<IFileSystemTree> trees)
    {
        var mergedRoot = new GameArchiveFolder("(root)", string.Empty, null!);
        var merged = new FileSystemTree(mergedRoot);

        foreach (var tree in trees)
        {
            if (tree.Root is GameArchiveFolder root)
                MergeFolders(merged.Root, root);
        }

        merged.Root.SortChildren();
        return merged;
    }

    private static void MergeFolders(GameArchiveFolder target, GameArchiveFolder source)
    {
        foreach (var (key, child) in source.Children)
        {
            if (child is GameArchiveFolder sourceFolder)
            {
                if (!target.Children.TryGetValue(key, out var existing) || existing is not GameArchiveFolder existingFolder)
                {
                    existingFolder = new GameArchiveFolder(sourceFolder.Name, sourceFolder.Path, sourceFolder.SourceArchive, target);
                    target.Children.TryAdd(key, existingFolder);
                }

                MergeFolders(existingFolder, sourceFolder);
            }
            else
            {
                // Same path across two archives? last wins
                target.Children.TryAdd(key, child);
            }
        }
    }

    public IGameArchiveEntry? GetByPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return Root;

        IGameArchiveEntry current = Root;
        ReadOnlySpan<char> pathSpan = path.AsSpan();

        foreach (var splitRange in pathSpan.Split('/'))
        {
            ReadOnlySpan<char> part = pathSpan[splitRange];
            if (part.IsEmpty)
                continue;

            if (current is not GameArchiveFolder folder)
                throw new InvalidOperationException($"Cannot traverse into a non-folder entry at '{current.Name}' for '{path}'.");

            if (!folder.TryGetEntry(part, out IGameArchiveEntry? next))
                return null;

            current = next;
        }

        return current;
    }
}
