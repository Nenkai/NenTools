//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/NenTools
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using System.IO.Hashing;
using System.Text;
using System.Runtime.CompilerServices;

namespace NenTools.IO.Hashing;

/// <summary>
/// Utility class for XXHash64 hashing.
/// </summary>
public class XXHash64
{
    /// <summary>
    /// XXHash64 a string using the specified seed.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="seed"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] HashString(string str, long seed)
    {
        return XxHash64.Hash(Encoding.ASCII.GetBytes(str), seed);
    }

    /// <summary>
    /// XXHash64 a buffer using the specified seed.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="seed"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Hash(Span<byte> data, long seed)
    {
        return XxHash64.HashToUInt64(data, seed);
    }
}