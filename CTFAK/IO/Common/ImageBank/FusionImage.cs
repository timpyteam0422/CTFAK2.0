using CTFAK.Memory;
using System.Drawing;

namespace CTFAK.IO.Common.Banks.ImageBank;

// ===GRAPHIC MODES===

// 0 - android, transparency, 4 bytes per pixel, 8 bits per channel
// 3 - android, no transparency, 3 bytes per pixel, 8 bits per channel

// 2 - android, transparency, 2 bytes per pixel, 5 bits per channel
// 1 - android, transparency, 2 bytes per pixel, 4 bits per channel
// 4 - android, no transparency, 2 bytes per pixel, 5 bits per channel

// 5 - android, no transparency, JPEG

// 4 - normal, 24 bits per color, 8 bit-deep alpha mask at the end
// 4 - mmf1.5, i don't like that
// 6 - normal, 15 bits per pixel, but it's actually 16 but retarded
// 7 - normal, 16 bits per pixel
// 8 - 2.5+, 32 bits per pixel, 8 bits per color

public abstract class FusionImage : DataLoader
{
    public int Handle;
    public int Checksum;
    public int References;
    public int Width;
    public int Height;
    public byte GraphicMode;
    public BitDict Flags = new(new[]
    {
        "RLE",
        "RLEW",
        "RLET",
        "LZX",
        "Alpha",
        "ACE",
        "Mac",
        "RGBA"
    });
    public short HotspotX;
    public short HotspotY;
    public short ActionX;
    public short ActionY;
    public Color Transparent;
    public byte[] ImageData;




    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}