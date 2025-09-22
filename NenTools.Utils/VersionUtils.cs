using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NenTools.Utils;

public class VersionUtils
{
    public static Version? GetExecutableVersion() => Assembly.GetEntryAssembly()?.GetName().Version;
}
