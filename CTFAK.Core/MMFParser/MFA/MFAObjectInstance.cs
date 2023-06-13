using CTFAK.Memory;
using CTFAK.MMFParser.CCN;

namespace CTFAK.MMFParser.MFA;

public class MFAObjectInstance : ChunkLoader
{
    public int X;
    public int Y;
    public int Layer;
    public int Handle;
    public uint Flags;
    public int ParentType;
    public int ItemHandle;
    public int ParentHandle;

    public override void Read(ByteReader reader)
    {
        X = reader.ReadInt32();
        Y = reader.ReadInt32();
        Layer = reader.ReadInt32();
        Handle = reader.ReadInt32();
        Flags = reader.ReadUInt32();
        ParentType = reader.ReadInt32();
        ItemHandle = reader.ReadInt32();
        ParentHandle = reader.ReadInt32();
    }

    public override void Write(ByteWriter writer)
    {
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
        writer.WriteInt32(Layer);
        writer.WriteInt32(Handle);
        writer.WriteUInt32(Flags);
        writer.WriteInt32(ParentType);
        writer.WriteInt32(ItemHandle);
        writer.WriteInt32(ParentHandle);
    }
}