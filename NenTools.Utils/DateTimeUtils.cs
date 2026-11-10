//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/NenTools
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using System.Globalization;

namespace NenTools.Utils;

public static class DateTimeUtils
{
    private static readonly string[] _formats = new string[] { "yyyy-MM-ddTHH:mm:ssK", "yyyy-MM-ddTHH:mm:ss.ffK", "yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-ddTHH:mm:ss.ffZ", "yyyy-MM-dd'T'HH:mm:ss.fffzzz" };

    public static string ToRfc3339String(this DateTime dt)
        => dt.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);

    public static DateTime FromRfc3339String(string dtString)
    {
        DateTime.TryParseExact(dtString, _formats,
            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime dt); // Will be defaulted if incorrect 
        return dt;
    }
}
