using CTFAK.Memory;

namespace CTFAK.IO.Ccn.ChunkSystem;

public abstract class BankChunk : Chunk
{
    public ChunkList Chunks;
    public override void Read(ByteReader reader)
    {
        Chunks = new ChunkList(this);
        Chunks.OnChunkLoaded += OnChunkLoaded;
        Chunks.Read(reader);
    }

    public override void Write(ByteWriter writer)
    {
        Chunks.Write(writer);
    }

    protected abstract void OnChunkLoaded(int chunkId, Chunk loader);

}