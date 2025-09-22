using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NenTools.IO.Streams.Interfaces;

public interface ISerializableStruct
{
    public void Read(SmartBinaryStream bs);
    public void Write(SmartBinaryStream bs);
    public uint GetSize();
}
