//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/NenTools
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

namespace NenTools.Reloaded.ScanManager;

public class PatternEntry
{
    public string Id { get; }
    public string Pattern { get; }
    public string Owner { get; }
    public nint? Address { get; private set; }
    public long Offset { get; set; }

    public PatternEntry(string id, string owner, string pattern, long offset = 0)
    {
        Id = id;
        Owner = owner;
        Pattern = pattern;
        Offset = offset;
    }

    public void SetAddress(nint address) 
        => Address = address;
}
