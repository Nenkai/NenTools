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
            // So we can dispose it without closing the inner stream, as other stream types may have associated
            // compression contexts that need to be disposed of.
            CompressionType.None => new NonDisposingStream(stream), 

            // Actual compression streams
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

internal sealed class NonDisposingStream : Stream
{
    private readonly Stream _inner;

    public NonDisposingStream(Stream inner) 
        => _inner = inner ?? throw new ArgumentNullException(nameof(inner));

    public override bool CanRead => _inner.CanRead;
    public override bool CanSeek => _inner.CanSeek;
    public override bool CanWrite => _inner.CanWrite;
    public override long Length => _inner.Length;
    public override long Position { get => _inner.Position; set => _inner.Position = value; }

    public override void Flush() => _inner.Flush();
    public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
    public override void Write(byte[] buffer, int offset, int count) => _inner.Write(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);
    public override void SetLength(long value) => _inner.SetLength(value);

    protected override void Dispose(bool disposing) { }
    public override ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
