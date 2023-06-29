using CTFAK.Memory;
using CTFAK.Utils;

namespace CTFAK.IO.Common.Banks.ImageBank;

public class AndroidImage : FusionImage
{
    public override void Read(ByteReader reader)
    {
        Handle = reader.ReadInt16();
        if (Context.BuildNumber >= 284)
            Handle--;
        GraphicMode = (byte)reader.ReadInt32();
        Width = reader.ReadInt16();
        Height = reader.ReadInt16();
        HotspotX = reader.ReadInt16();
        HotspotY = reader.ReadInt16();
        ActionX = reader.ReadInt16();
        ActionY = reader.ReadInt16();
        var dataSize = reader.ReadInt32();

        // TODO: This is definitely not the correct way to check for compression. Fix it
        if (reader.PeekByte() == 255)
            ImageData = reader.ReadBytes(dataSize);
        else
            ImageData = Decompressor.DecompressBlock(reader, dataSize);

    }
}