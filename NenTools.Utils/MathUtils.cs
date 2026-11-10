//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/NenTools
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

namespace NenTools.Utils;

public class MathUtils
{
    public static int ClampWrap(int value, int min, int max)
    {
        if (value > max)
            return min;
        else if (value < min)
            return max;

        return value;
    }

    public static uint AlignValue(uint x, uint alignment)
    {
        uint mask = ~(alignment - 1);
        return (x + (alignment - 1)) & mask;
    }

    public static float Lerp(float value1, float value2, float amount)
    {
        return (value1 * (1f - amount)) + (value2 * amount);
    }
}
