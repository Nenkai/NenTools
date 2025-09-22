using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NenTools.Utils;

public class ByteUtils
{
    public static string BytesToString(ulong value)
    {
        string suffix;
        double readable;
        switch (value)
        {
            case >= 0x1000000000000000:
                suffix = "EiB";
                readable = value >> 50;
                break;
            case >= 0x4000000000000:
                suffix = "PiB";
                readable = value >> 40;
                break;
            case >= 0x10000000000:
                suffix = "TiB";
                readable = value >> 30;
                break;
            case >= 0x40000000:
                suffix = "GiB";
                readable = value >> 20;
                break;
            case >= 0x100000:
                suffix = "MiB";
                readable = value >> 10;
                break;
            case >= 0x400:
                suffix = "KiB";
                readable = value;
                break;
            default:
                return value.ToString("0 B");
        }

        return (readable / 1024).ToString("0.## ", CultureInfo.InvariantCulture) + suffix;
    }

    public static int MeasureBytesTakenByBits(double bitCount)
    => (int)Math.Round(bitCount / 8, MidpointRounding.AwayFromZero);

    public static ulong GetMaxValueForBitCount(int nBits)
        => (ulong)((1L << nBits) - 1);

    /// <summary>
    /// Gets the highest bit index for a value
    /// </summary>
    /// <param name="value">Value to get the highest bit for</param>
    /// <returns></returns>
    public static int GetHighestBitIndex(int value)
    {
#if NETCOREAPP3_0_OR_GREATER
        return (32 - BitOperations.LeadingZeroCount((uint)value));
#else
        return (32 - LeadingZerosSoftware(value));
#endif
    }

    /// <summary>
    /// Gets the max signed value for the specified bit count
    /// </summary>
    /// <param name="bits">Number of bits</param>
    /// <returns></returns>
    public static uint GetMaxSignedForBitCount(int bits)
    {
        if (bits == 0)
            return 0;

        return (uint)(GetMaxValueForBitCount(bits) / 2) - 1;
    }

    /// <summary>
    /// Packs a float into signed bits
    /// </summary>
    /// <param name="value">Value to pack</param>
    /// <param name="packedValue">Packed value bits</param>
    /// <param name="bits">Bits taken by value</param>
    public static void PackFloat(float value, out int packedValue, out int bits)
    {
        float rounded = (float)Math.Round(value);

        int absDistance = (int)Math.Abs(rounded);
        bits = GetHighestBitIndex(absDistance);

        if (absDistance != 0 && absDistance > GetMaxSignedForBitCount(bits))
            bits++;

        if (value < 0)
            packedValue = (int)(GetMaxValueForBitCount(bits) + rounded);
        else
            packedValue = absDistance;
    }

    /// <summary>
    /// Packs a float to a specific amount of bits
    /// </summary>
    /// <param name="value"></param>
    /// <param name="bits"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static int PackFloatToBitRange(float value, int bits)
    {
        float rounded = (float)Math.Round(value);

        int max = (int)GetMaxValueForBitCount(bits);
        if (rounded > max)
            throw new Exception($"Value too large to pack to {bits} bits");

        if (value < 0)
            return max + (int)rounded;
        else
            return (int)rounded;
    }
}
