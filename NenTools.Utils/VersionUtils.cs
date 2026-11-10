//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/NenTools
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using System.Reflection;

namespace NenTools.Utils;

public class VersionUtils
{
    public static Version? GetExecutableVersion() => Assembly.GetEntryAssembly()?.GetName().Version;
}
