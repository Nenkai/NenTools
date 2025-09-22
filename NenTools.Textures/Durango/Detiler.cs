using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using NenTools.Textures.Dxgi;

namespace NenTools.Textures.Durango;

public class DeTiler
{
    private unsafe static (XG_RESOURCE_LAYOUT Layout, byte[] Data)? DurangoDetile(byte[] tiledData, uint width, uint height, XG_FORMAT format, 
        uint arraySize_depth_numSlice, uint fullTextureNumMips, uint numMips, XG_TILE_MODE tileMode)
    {
        // For now we gotta use the xg lib for detiling, it sucks
        // It's pretty complex
        XG_TEXTURE2D_DESC desc;
        desc.Width = width;
        desc.Height = height;
        desc.Format = format;
        desc.Usage = XG_USAGE.XG_USAGE_DEFAULT; // Should be default
        desc.SampleDesc.Count = 1; // Should be 1
        desc.ArraySize = arraySize_depth_numSlice;
        desc.MipLevels = numMips;
        desc.BindFlags = (uint)XG_BIND_FLAG.XG_BIND_SHADER_RESOURCE;
        desc.MiscFlags = 0;
        desc.TileMode = tileMode;

        XGTextureAddressComputer* compWrapper;
        int result = XGImports.XGCreateTexture2DComputer(&desc, &compWrapper);
        if (result > 0)
        {
            Console.WriteLine($"ERROR: Failed to XGCreateTexture2DComputer (0x{result:X8})");
            return null;
        }

        XGTextureAddressComputer computer = *compWrapper;
        nint arrPtr = Marshal.AllocHGlobal(Marshal.SizeOf<XG_RESOURCE_LAYOUT>());

        result = computer.vt->GetResourceLayout(compWrapper, (XG_RESOURCE_LAYOUT*)arrPtr);
        if (result > 0)
        {
            Console.WriteLine($"ERROR: Failed to GetResourceLayout (0x{result:X8})");
            return null;
        }

        try
        {
            XG_RESOURCE_LAYOUT layout = Marshal.PtrToStructure<XG_RESOURCE_LAYOUT>(arrPtr);
            byte[] outputFile = new byte[layout.SizeBytes];

            for (uint nSlice = 0; nSlice < 1 /* depth/images */; nSlice++)
            {
                // Go through each mips
                for (uint nMip = 0; nMip < desc.MipLevels; nMip++)
                {
                    ulong mipSizeBytes = layout.Plane[0].MipLayout[nMip].SizeBytes;
                    ulong mipOffset = layout.Plane[0].MipLayout[nMip].OffsetBytes;

                    uint nDstSubResIdx = nMip + fullTextureNumMips * nSlice;
                    uint nRowPitch = layout.Plane[0].MipLayout[nMip].PitchBytes;

                    byte[] outputBytes = new byte[mipSizeBytes];

                    fixed (byte* outputPtr = outputBytes)
                    fixed (byte* inputPtr = tiledData)
                    {
                        // CopyFromSubresource will detile but not decode - just what we need
                        result = computer.vt->CopyFromSubresource(compWrapper, outputPtr, 0u, nDstSubResIdx, inputPtr, nRowPitch, 0);
                        if (result > 0)
                        {
                            Console.WriteLine($"ERROR: Failed to CopyFromSubresource (0x{result:X8})");
                            return null;
                        }

                        outputBytes.AsSpan().CopyTo(outputFile.AsSpan((int)mipOffset));
                    }
                }
            }

            return (layout, outputFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }
        finally
        {
            Marshal.FreeHGlobal(arrPtr);
        }

        return null;
    }

    private static byte[] DealignDurangoTextureData(uint width, uint height, int fullTextureNumMips, int numMips, XG_FORMAT format, (XG_RESOURCE_LAYOUT Layout, byte[] Data)? DetiledData)
    {
        uint totalSize = 0;
        uint w = width; uint h = height;
        for (int i = 0; i < fullTextureNumMips; i++)
        {
            DxgiUtils.ComputePitch((DXGI_FORMAT)format, w, h, out ulong rowPitch, out ulong slicePitch, out ulong alignedSlicePitch);
            totalSize += (uint)slicePitch;

            w >>= 1;
            h >>= 1;
        }

        byte[] outputData = new byte[totalSize];
        w = width; h = height;
        ulong offset = 0;
        for (int i = 0; i < numMips; i++)
        {
            DxgiUtils.ComputePitch((DXGI_FORMAT)format, w, h, out ulong rowPitch, out ulong slicePitch, out ulong alignedSlicePitch);

            XG_MIPLEVEL_LAYOUT mipLayout = DetiledData.Value.Layout.Plane[0].MipLayout[i];

            Span<byte> inputMip = DetiledData.Value.Data.AsSpan((int)mipLayout.OffsetBytes, (int)mipLayout.SizeBytes);
            Span<byte> outputMip = outputData.AsSpan((int)offset, (int)slicePitch);

            uint actualHeight = DxgiUtils.IsBCnFormat((DXGI_FORMAT)format) ? h / 4 : h;
            for (int y = 0; y < actualHeight; y++)
            {
                Span<byte> row = inputMip.Slice((y * (int)mipLayout.PitchBytes), (int)rowPitch);
                Span<byte> outputRow = outputMip.Slice((int)(y * (uint)rowPitch), (int)rowPitch);
                row.CopyTo(outputRow);
            }

            w >>= 1;
            h >>= 1;
            offset += slicePitch;
        }

        return outputData;
    }
}
