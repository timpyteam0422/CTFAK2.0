using CTFAK.Attributes;
using CTFAK.Memory;

namespace CTFAK.IO.CCN.Chunks;

[ChunkLoader(8773, "ExtendedHeader")]
public class ExtendedHeader : Chunk
{

    public BitDict CompressionFlags = new(new[]
    {
        "CompressionLevelMax",
        "CompressSounds",
        "IncludeExternalFiles",
        "NoAutoImageFilters",
        "NoAutoSoundFilters",
        "Unknown3",
        "Unknown4",
        "Unknown5",
        "DontDisplayBuildWarning",
        "OptimizeImageSize"
    });

    public BitDict Flags = new(new[]
    {
        "KeepScreenRatio",
        "Unknown1",
        "AntiAliasingWhenResizing",
        "Unknown2",
        "Unknown3",
        "RightToLeftReading",
        "Unknown4",
        "RightToLeftLayout",
        "Unknown5",
        "Unknown6",
        "Unknown7",
        "Unknown8",
        "Unknown9",
        "Unknown10",
        "Unknown11",
        "Unknown12",
        "Unknown13",
        "Unknown14",
        "Unknown15",
        "Unknown16",
        "Unknown17",
        "Unknown18",
        "DontOptimizeStrings",
        "Unknown19",
        "Unknown20",
        "Unknown21",
        "DontIgnoreDestroy",
        "DisableIME",
        "ReduceCPUUsage",
        "Unknown22",
        "PremultipliedAlpha",
        "OptimizePlaySample"
    });

    public BitDict NewFlags = new(new[]
    {
        "Unknown1"
    });

    public short ScreenAngle;
    public short ScreenRatio;

    public BitDict ViewFlags = new(new[]
    {
        "Unknown1"
    });

    public override void Read(ByteReader reader)
    {
        Flags.Flag = reader.ReadUInt32();
        reader.Skip(3); // idk
        Context.BuildType = (FusionBuildType)reader.ReadByte();
        CompressionFlags.Flag = reader.ReadUInt32();
        ScreenRatio = reader.ReadInt16();
        ScreenAngle = reader.ReadInt16();
        ViewFlags.Flag = reader.ReadUInt16();
        NewFlags.Flag = reader.ReadUInt16();
    }

    public override void Write(ByteWriter writer)
    {
        writer.WriteUInt32(Flags.Flag);
        writer.Skip(3);
        writer.WriteInt8((byte)Context.BuildType);
        writer.WriteUInt32(CompressionFlags.Flag);
        writer.WriteInt16(ScreenRatio);
        writer.WriteInt16(ScreenAngle);
        writer.WriteUInt16((ushort)ViewFlags.Flag);
        writer.WriteUInt16((ushort)NewFlags.Flag);
        
    }
}