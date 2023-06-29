using CTFAK.Memory;
using System.Runtime.CompilerServices;

namespace CTFAK.IO.Common.Banks.ImageBank;

public class NormalImage : FusionImage
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override void Read(ByteReader reader)
    {
        Handle = reader.ReadInt32();
        if (Context.BuildNumber >= 284)
            Handle--;

        var decompressedSize = reader.ReadInt32();
        var compSize = reader.ReadInt32();
        var compressedBuffer = reader.ReadBytes(compSize);
        var task = new Task(() =>
        {

            using (var decompressedReader = new ByteReader(Decompressor.DecompressBlock(compressedBuffer)))
            {
                Checksum = decompressedReader.ReadInt32();
                References = decompressedReader.ReadInt32();
                var dataSize = decompressedReader.ReadInt32();
                Width = decompressedReader.ReadInt16();
                Height = decompressedReader.ReadInt16();
                GraphicMode = decompressedReader.ReadByte();
                Flags.Flag = decompressedReader.ReadByte();
                decompressedReader.ReadInt16();
                HotspotX = decompressedReader.ReadInt16();
                HotspotY = decompressedReader.ReadInt16();
                ActionX = decompressedReader.ReadInt16();
                ActionY = decompressedReader.ReadInt16();
                Transparent = decompressedReader.ReadColor();
                if (Flags["LZX"])
                {
                    var decompSize = decompressedReader.ReadInt32();
                    ImageData = Decompressor.DecompressBlock(decompressedReader,
                        (int)(decompressedReader.Size() - decompressedReader.Tell()));
                }
                else
                {
                    ImageData = decompressedReader.ReadBytes(dataSize);
                }
            }


        });
        task.Start();
        //task.RunSynchronously();
        ImageBank.imageReadingTasks.Add(task);
    }
}