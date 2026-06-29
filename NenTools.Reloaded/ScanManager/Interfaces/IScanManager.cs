namespace NenTools.Reloaded.ScanManager.Interfaces;

public interface IScanManager
{
    /// <summary>
    /// Initialize scan signatures from the specified folder.
    /// </summary>
    /// <param name="folder">Folder containing signature (.ini) files.</param>
    /// <param name="owner">Owner, usually your mod id.</param>
    /// <param name="recursive">Whether to search recursive folders too.</param>
    void InitializeScans(string folder, string owner, bool recursive = false);

    /// <summary>
    /// Adds a scan.
    /// </summary>
    /// <param name="id">Scan name/id.</param>
    /// <param name="groupSource">Scan group, normally matching signature file name without extension. If left null, searches all groups.</param>
    /// <param name="onSuccess">On success callback. May fire immediately if a previous scan for the same pattern was made previously.</param>
    /// <param name="onFail">On fail callback, if the pattern was not found.</param>
    void AddScan(string id, string? groupSource, Action<nint> onSuccess, Action? onFail = null);
}
