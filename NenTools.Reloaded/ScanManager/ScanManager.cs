using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Drawing;

using Reloaded.Hooks.Definitions;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;

using PeanutButter.INI;

using NenTools.Reloaded.ScanManager.Interfaces;
using System.Globalization;

namespace NenTools.Reloaded.ScanManager;

public class ScanManager : IScanManager
{
    private readonly ILogger _logger;
    private readonly IStartupScanner _scanner;

    private Dictionary<string, PatternGroup> _groups = [];

    private readonly nint? BaseAddress = Process.GetCurrentProcess().MainModule?.BaseAddress;

    public ScanManager(IStartupScanner startupScanner, ILogger logger)
    {
        _scanner = startupScanner;
        _logger = logger;
    }

    public void InitializeScans(string folder, string owner, bool recursive = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folder, nameof(folder));
        if (!Directory.Exists(folder))
            throw new FileNotFoundException($"Signatures folder '{folder}' does not exist.");

        _logger.WriteLine($"[{nameof(ScanManager)}] Loading signatures from {folder} (owner: {owner})...");

        foreach (var file in Directory.GetFiles(folder, "*.ini", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
        {
            INIFile ini = new INIFile(file);

            string groupId = Path.GetFileNameWithoutExtension(file);
            if (!ini.HasSection("Scans"))
            {
                _logger.WriteLine($"File '{groupId}' does not have a [Scans] section!", Color.Red);
                continue;
            }

            IDictionary<string, string> values = ini.GetSection("Scans");

            if (!_groups.TryGetValue(groupId, out PatternGroup? group))
            {
                group = new PatternGroup();
                _groups.Add(groupId, group);
            }

            foreach (var value in values)
            {
                if (string.IsNullOrEmpty(value.Value))
                {
                    _logger.WriteLine($"[{nameof(ScanManager)}] Pattern '{value.Key}' in group '{groupId}' is null or empty!", Color.Yellow);
                    continue;
                }

                var pattern = new PatternEntry(value.Key, owner, value.Value);
                group.Patterns.Add(pattern.Id, pattern);
            }
        }
    }

    public void AddScan(string id, string? source, Action<nint> onSuccess, Action? onFail = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));

        if (BaseAddress is null)
            throw new Exception("Could not determine base address.");

        PatternEntry? patternEntry = null;
        if (!string.IsNullOrWhiteSpace(source))
        {
            if (!_groups.TryGetValue(source, out PatternGroup? group))
            {
                _logger.WriteLine("Ini not found");
                return;
            }

            if (!group.Patterns.TryGetValue(id, out patternEntry))
            {
                _logger.WriteLine($"[{nameof(ScanManager)}] Signature with name '{id}' not found in '{source}'", Color.Red);
                return;
            }

            if (patternEntry is null)
            {
                _logger.WriteLine($"[{nameof(ScanManager)}] Signature with name '{id}' in source '{source}' is invalid.", Color.Red);
                return;
            }
        }
        else
        {
            foreach (var (_, ini) in _groups)
            {
                if (ini.Patterns.TryGetValue(id, out patternEntry))
                    break;
            }

            if (patternEntry is null)
            {
                _logger.WriteLine($"[{nameof(ScanManager)}] Signature with name '{id}' not found in any signature file.", Color.Red);
                return;
            }
        }

        if (patternEntry.Address is not null)
        {
            onSuccess(patternEntry.Address.Value);
            return;
        }

        string patternString = patternEntry.Pattern.Trim();
        int idx = 0;
        string pattern = string.Empty;
        foreach (var spl in patternString.AsSpan().Split('|'))
        {
            switch (idx)
            {
                case 0:
                    pattern = spl.ToString();
                    if (string.IsNullOrWhiteSpace(pattern))
                    {
                        _logger.WriteLine($"[{nameof(ScanManager)}] Signature with name '{id}' in source '{source}' is invalid.", Color.Red);
                        return;
                    }
                    break;

                case 1:
                    {
                        ReadOnlySpan<char> range = patternString[spl];
                        if (range.Length > 0 && range[0] == '+')
                            range = range[1..];

                        NumberStyles numberStyles = NumberStyles.Number;
                        if (range.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                        {
                            range = range[2..];
                            numberStyles = NumberStyles.HexNumber;
                        }

                        if (!long.TryParse(range, numberStyles, CultureInfo.InvariantCulture, out long offset))
                        {
                            _logger.WriteLine($"[{nameof(ScanManager)}] Signature with name '{id}' in source '{source}' has an invalid offset.", Color.Red);
                            return;
                        }

                        patternEntry.Offset = offset;
                    }
                    break;
            }
        }

        _scanner.AddMainModuleScan(pattern, (result) =>
        {
            if (!result.Found)
            {
                OnPatternNotFound(patternEntry, onFail);
                return;
            }

            OnPatternFound(patternEntry, BaseAddress.Value + result.Offset + (nint)patternEntry.Offset, onSuccess);
        });
    }

    private void OnPatternNotFound(PatternEntry patternEntry, Action? onFail)
    {
        _logger.WriteLine($"[{nameof(ScanManager)}] Pattern {patternEntry.Id} not found!", Color.Red);
        onFail?.Invoke();
    }

    private void OnPatternFound(PatternEntry patternEntry, nint result, Action<nint> onSuccess)
    {
        _logger.WriteLine($"[{nameof(ScanManager)}] Found pattern {patternEntry.Id} @ 0x{result:X}", Color.LightPink);
        patternEntry.SetAddress(result);

        onSuccess(result);
    }
}
