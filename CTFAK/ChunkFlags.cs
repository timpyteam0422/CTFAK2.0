namespace CTFAK;

public enum ChunkFlags
{
    NotCompressed = 0, // MODE0
    Compressed = 1, // MODE1
    Encrypted = 2, // MODE2
    CompressedAndEncrypted = 3 // MODE3
}