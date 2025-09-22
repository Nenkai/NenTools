using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NenTools.Textures.Durango
{
    public partial class XGImports
    {
        [LibraryImport("xg.dll", EntryPoint = "XGCreateTexture2DComputer")]
        public unsafe static partial int XGCreateTexture2DComputer(XG_TEXTURE2D_DESC* desc, XGTextureAddressComputer** computer);
    }
}