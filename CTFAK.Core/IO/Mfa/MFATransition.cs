using System.Drawing;
using CTFAK.IO.CCN;
using CTFAK.Memory;

namespace CTFAK.IO.MFA;

public class MFATransition : DataLoader
{
    public string Module;
    public string Name;
    public string Id;
    public string TransitionId;
    public int Duration;
    public int Flags;
    public Color Color;
    public byte[] ParameterData;

    public override void Read(ByteReader reader)
    {
        Module = reader.AutoReadUnicode();
        Name = reader.AutoReadUnicode();
        Id = reader.ReadAscii(4);
        TransitionId = reader.ReadAscii(4);
        Duration = reader.ReadInt32();
        Flags = reader.ReadInt32();
        Color = reader.ReadColor();
        ParameterData = reader.ReadBytes(reader.ReadInt32());
    }

    public override void Write(ByteWriter writer)
    {
        writer.AutoWriteUnicode(Module);
        writer.AutoWriteUnicode(Name);
        writer.WriteAscii(Id);
        writer.WriteAscii(TransitionId);
        writer.WriteInt32(Duration);
        writer.WriteInt32(Flags);
        writer.WriteColor(Color);
        writer.WriteInt32(ParameterData.Length);
        writer.WriteBytes(ParameterData);
    }
}