using System;
using System.Collections.Generic;
using CTFAK.Memory;

namespace CTFAK.IO.CCN.Chunks.Objects;

public class AlterableValues : DataLoader
{
    public List<int> Items = new();
    public int Flags;

    public override void Read(ByteReader reader)
    {
        // TODO: Figure out why there are try/catches here
        var count = reader.ReadInt16();
        for (var i = 0; i < count; i++)
            try
            {
                Items.Add(reader.ReadInt32());
            }
            catch
            {
                break;
            }

        try
        {
            Flags = reader.ReadInt32();
        }
        catch
        {
        }
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}

public class AlterableStrings : DataLoader
{
    public List<string> Items = new();

    public override void Read(ByteReader reader)
    {
        var count = reader.ReadInt16();
        for (var i = 0; i < count; i++)
            try
            {
                Items.Add(reader.ReadWideString());
            }
            catch
            {
                break;
            }
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}