using CTFAK.Attributes;
using CTFAK.Memory;

namespace CTFAK.IO.Common.Banks;

[ChunkLoader(26215, "FontBank")]
public class FontBank : ListChunk<FontItem>
{
}

public class FontItem : DataLoader
{
    public int Checksum;
    public bool Compressed;
    public uint Handle;
    public int References;
    public LogFont Value;

    public override void Read(ByteReader reader)
    {

    }

    public override void Write(ByteWriter writer)
    {

    }
}

public class LogFont : DataLoader
{
    private byte _charSet;
    private byte _clipPrecision;
    private int _escapement;
    private string _faceName;
    private int _height;
    private byte _italic;
    private int _orientation;
    private byte _outPrecision;
    private byte _pitchAndFamily;
    private byte _quality;
    private byte _strikeOut;
    private byte _underline;
    private int _weight;
    private int _width;

    public override void Read(ByteReader reader)
    {
        _height = reader.ReadInt32();
        _width = reader.ReadInt32();
        _escapement = reader.ReadInt32();
        _orientation = reader.ReadInt32();
        _weight = reader.ReadInt32();
        _italic = reader.ReadByte();
        _underline = reader.ReadByte();
        _strikeOut = reader.ReadByte();
        _charSet = reader.ReadByte();
        _outPrecision = reader.ReadByte();
        _clipPrecision = reader.ReadByte();
        _quality = reader.ReadByte();
        _pitchAndFamily = reader.ReadByte();
        _faceName = reader.ReadWideString(32);
    }

    public override void Write(ByteWriter writer)
    {
        writer.WriteInt32(_height);
        writer.WriteInt32(_width);
        writer.WriteInt32(_escapement);
        writer.WriteInt32(_orientation);
        writer.WriteInt32(_weight);
        writer.WriteInt8(_italic);
        writer.WriteInt8(_underline);
        writer.WriteInt8(_strikeOut);
        writer.WriteInt8(_charSet);
        writer.WriteInt8(_outPrecision);
        writer.WriteInt8(_clipPrecision);
        writer.WriteInt8(_quality);
        writer.WriteInt8(_pitchAndFamily);
        writer.WriteUnicode(_faceName);
    }
}

