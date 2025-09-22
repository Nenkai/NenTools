using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

using NenTools.IO.Streams;

namespace NenTools.Textures.Dxgi;

public class DdsHeader
{
    public DDSHeaderFlags Flags { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public int PitchOrLinearSize { get; set; }
    public int Depth { get; set; } = 0;
    public int LastMipmapLevel { get; set; }
    public DDSPixelFormatFlags FormatFlags { get; set; }
    public string FourCCName { get; set; }
    public int RGBBitCount { get; set; }
    public uint RBitMask { get; set; }
    public uint GBitMask { get; set; }
    public uint BBitMask { get; set; }
    public uint ABitMask { get; set; }
    public DDSCAPS DwCaps1 { get; set; } = DDSCAPS.DDSCAPS_TEXTURE;
    public DDSCAPS2 DwCaps2 { get; set; } = 0;

    public DXGI_FORMAT DxgiFormat { get; set; }
    public D3D10_RESOURCE_DIMENSION ResourceDimension { get; set; } = D3D10_RESOURCE_DIMENSION.DDS_DIMENSION_TEXTURE2D;

    public Memory<byte> ImageData { get; set; }

    public void Write(Stream outStream, Stream imageDataStream)
    {
        var bs = new SmartBinaryStream(outStream);

        bs.WriteString("DDS ", StringCoding.Raw);
        bs.WriteInt32(0x7C);    // dwSize (Struct Size)
        bs.WriteUInt32((uint)Flags); // dwFlags
        bs.WriteInt32(Height); // dwHeight
        bs.WriteInt32(Width);
        bs.WriteInt32(PitchOrLinearSize);
        bs.WriteInt32(Depth);    // Depth
        bs.WriteInt32(LastMipmapLevel);
        bs.WriteBytes(new byte[44]); // reserved
        bs.WriteInt32(32); // DDSPixelFormat Header starts here - Struct Size

        bs.WriteUInt32((uint)FormatFlags);           // Format Flags
        bs.WriteString(FourCCName, StringCoding.Raw); // FourCC
        bs.WriteInt32(RGBBitCount);         // RGBBitCount

        bs.WriteUInt32(RBitMask);  // RBitMask 
        bs.WriteUInt32(GBitMask);  // GBitMask
        bs.WriteUInt32(BBitMask);  // BBitMask
        bs.WriteUInt32(ABitMask);  // ABitMask

        bs.WriteInt32((int)DwCaps1); // dwCaps, 0x1000 = required
        bs.WriteInt32((int)DwCaps2);
        bs.WriteBytes(new byte[12]); // dwCaps3-4 + reserved

        if (FourCCName == "DX10")
        {
            // DDS_HEADER_DXT10
            bs.WriteInt32((int)DxgiFormat);
            bs.WriteInt32((int)ResourceDimension);  // DDS_DIMENSION_TEXTURE2D
            bs.WriteUInt32(0);  // miscFlag
            bs.WriteInt32(1); // arraySize
            bs.WriteInt32(0); // miscFlags2
        }

        imageDataStream.CopyTo(outStream);
    }
}

public enum D3D10_RESOURCE_DIMENSION
{
    DDS_DIMENSION_TEXTURE1D = 2,
    DDS_DIMENSION_TEXTURE2D = 3,
    DDS_DIMENSION_TEXTURE3D = 4,
}

[Flags]
public enum DDSCAPS
{
    DDSCAPS_COMPLEX = 0x08,
    DDSCAPS_MIPMAP = 0x400000,
    DDSCAPS_TEXTURE = 0x1000,
}

[Flags]
public enum DDSCAPS2
{
    DDSCAPS2_CUBEMAP = 0x200,
    DDSCAPS2_CUBEMAP_POSITIVEX = 0x400,
    DDSCAPS2_CUBEMAP_NEGATIVEX = 0x800,
    DDSCAPS2_CUBEMAP_POSITIVEY = 0x1000,
    DDSCAPS2_CUBEMAP_NEGATIVEY = 0x2000,
    DDSCAPS2_CUBEMAP_POSITIVEZ = 0x4000,
    DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x8000,
    DDSCAPS2_VOLUME = 0x200000,
}

[Flags]
public enum DDSPixelFormatFlags
{
    DDPF_ALPHAPIXELS = 0x01,
    DDPF_ALPHA = 0x02,
    DDPF_FOURCC = 0x04,
    DDPF_RGB = 0x40,
    DDPF_YUV = 0x200,
    DDPF_LUMINANCE = 0x20000
}

[Flags]
public enum DDSHeaderFlags : uint
{
    TEXTURE = 0x00001007,  // DDSDCAPS | DDSDHEIGHT | DDSDWIDTH | DDSDPIXELFORMAT 
    MIPMAP = 0x00020000,  // DDSDMIPMAPCOUNT
    DEPTH = 0x00800000,  // DDSDDEPTH
    PITCH = 0x00000008,  // DDSDPITCH
    LINEARSIZE = 0x00080000,  // DDSDLINEARSIZE
}

