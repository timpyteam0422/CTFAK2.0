using CTFAK.Memory;

namespace CTFAK.IO.Common.Events;

internal class Int : ParameterCommon
{
    public int Value;

    public override void Read(ByteReader reader)
    {
        Value = reader.ReadInt32();
    }

    public override void Write(ByteWriter writer)
    {
        writer.WriteInt32(Value);
    }
}