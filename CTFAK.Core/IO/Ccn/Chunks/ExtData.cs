using System;
using CTFAK.Memory;
using CTFAK.Utils;

namespace CTFAK.IO.CCN.Chunks;

public class ExtData : Chunk
{
    public string Name;

    public byte[] Data;

    public override void Read(ByteReader reader)
    {
        Name = reader.ReadAscii();
        Data = reader.ReadBytes();
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}