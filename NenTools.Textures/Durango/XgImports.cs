//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/NenTools
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace NenTools.Textures.Durango
{
    public partial class XGImports
    {
        [LibraryImport("xg.dll", EntryPoint = "XGCreateTexture2DComputer")]
        public unsafe static partial int XGCreateTexture2DComputer(XG_TEXTURE2D_DESC* desc, XGTextureAddressComputer** computer);
    }
}