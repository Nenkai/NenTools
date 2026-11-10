//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/NenTools
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

namespace NenTools.IO.Streams.Interfaces;

public interface ISerializableStruct
{
    public void Read(SmartBinaryStream bs);
    public void Write(SmartBinaryStream bs);
    public uint GetSize();
}
