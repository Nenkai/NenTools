using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NenTools.Reloaded.ScanManager;

public class PatternEntry
{
    public string Id { get; }
    public string Pattern { get; }
    public string Owner { get; }
    public nint? Address { get; private set; }

    public PatternEntry(string id, string owner, string pattern)
    {
        Id = id;
        Owner = owner;
        Pattern = pattern;
    }

    public void SetAddress(nint address) 
        => Address = address;
}
