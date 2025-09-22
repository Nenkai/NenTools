using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vortice.DirectStorage;
using SharpGen.Runtime;

namespace NenTools.IO.Compression;

public class GDeflate
{
    private static IDStorageCompressionCodec _codec;
    static GDeflate()
    {
        _codec = DirectStorage.DStorageCreateCompressionCodec(CompressionFormat.GDeflate, (uint)Environment.ProcessorCount);
    }

    public static void Decompress(Span<byte> compressedData, Span<byte> decompressedData)
    {
        unsafe
        {
            fixed (byte* buffer = compressedData)
            fixed (byte* buffer2 = decompressedData)
            {
                _codec.DecompressBuffer((nint)buffer, (uint)compressedData.Length, (nint)buffer2, (uint)decompressedData.Length, (uint)decompressedData.Length);
            }
        }
    }

    public static uint Compress(Span<byte> decompressedData, Span<byte> outputCompressedData)
    {
        unsafe
        {
            fixed (byte* inputDecompChunkPtr = decompressedData)
            fixed (byte* outputCompChunkPtr = outputCompressedData)
            {
                CompressBuffer(_codec, (nint)inputDecompChunkPtr, decompressedData.Length, Vortice.DirectStorage.Compression.BestRatio, (nint)outputCompChunkPtr, outputCompressedData.Length, out long compressedDataSize);
                return (uint)compressedDataSize;
            }
        }
    }

    public static long CompressionSize(ulong size)
    {
        return _codec.CompressBufferBound(size);
    }

    // This is a hack. CompressBuffer would offer no way to grab back the compressed size
    private static unsafe void CompressBuffer(IDStorageCompressionCodec codec,
        nint uncompressedData, PointerSize uncompressedDataSize, Vortice.DirectStorage.Compression compressionSetting, nint compressedBuffer, PointerSize compressedBufferSize, out long compressedDataSize)
    {
        long value;
        long** vtbl = (long**)codec.NativePointer;
        ((Result)((delegate* unmanaged[Stdcall]<nint, void*, void*, int, void*, void*, void*, int>)(*vtbl)[3])(
            codec.NativePointer, (void*)uncompressedData, uncompressedDataSize, (int)compressionSetting, (void*)compressedBuffer, compressedBufferSize, &value)).CheckError();
        compressedDataSize = value;
    }
}
