namespace CTFAK.IO.Ccn.ChunkSystem;

public abstract class DictChunk<T, T2> : Chunk // TODO: Slidy please make a proper implementation for this one
{
    public Dictionary<T, T2> Items = new Dictionary<T, T2>();
}
