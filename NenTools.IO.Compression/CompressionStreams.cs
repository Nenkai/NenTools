using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpromptuNinjas.ZStd;

namespace NenTools.IO.Compression;

public class CompressionStreams
{
    public static Stream New(Stream stream, CompressionType type)
    {
        return type switch
        {
            CompressionType.None => stream,
            CompressionType.ZStd => new ZStdDecompressStream(stream, leaveOpen: true),
            _ => throw new NotSupportedException($"Compression type {type} is not supported."),
        };
    }

    public enum CompressionType
    {
        None,
        ZStd,
    }
}
