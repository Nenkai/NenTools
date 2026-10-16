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
