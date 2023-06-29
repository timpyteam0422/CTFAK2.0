using CTFAK.Memory;
using System.Drawing;

namespace CTFAK.IO.Common.Events;

internal class Colour : ParameterCommon
{
    public Color Value;

    public override void Read(ByteReader reader)
    {
        var bytes = reader.ReadBytes(4);
        Value = Color.FromArgb(bytes[0], bytes[1], bytes[2]);
    }

    public override void Write(ByteWriter writer)
    {
        writer.WriteInt8(Value.R);
        writer.WriteInt8(Value.G);
        writer.WriteInt8(Value.B);
        writer.WriteInt8(255);
    }
}