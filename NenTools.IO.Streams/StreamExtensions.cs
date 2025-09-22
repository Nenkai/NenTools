using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.HighPerformance.Buffers;

namespace NenTools.IO.Streams;

public static class StreamExtensions
{
    public static void CopyStreamRange(this Stream inputStream, Stream outputStream, uint size, int bufferSize = 0x40000)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bufferSize, nameof(bufferSize));

        long remSize = size;
        using MemoryOwner<byte> outBuffer = MemoryOwner<byte>.Allocate(bufferSize);

        while (remSize > 0)
        {
            int chunkSize = (int)Math.Min(remSize, bufferSize);
            Span<byte> chunk = outBuffer.Span.Slice(0, chunkSize);

            inputStream.ReadExactly(chunk);
            outputStream.Write(chunk);

            remSize -= chunkSize;
        }
    }
}
