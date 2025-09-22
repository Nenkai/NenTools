using System.Reflection;

namespace NenTools.Utils;

public class PathUtils
{
    public static string GetCurrentExecutingPath()
    {
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        if (string.IsNullOrEmpty(assemblyLocation)) // This may be empty if we compiled the executable as single-file.
            assemblyLocation = Environment.GetCommandLineArgs()[0]!;

        return assemblyLocation;
    }
}
