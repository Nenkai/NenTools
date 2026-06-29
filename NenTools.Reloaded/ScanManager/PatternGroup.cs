using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NenTools.Reloaded.ScanManager;

public class PatternGroup
{
    public Dictionary<string, PatternEntry> Patterns { get; } = [];
}
