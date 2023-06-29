using CTFAK.Memory;

namespace CTFAK.IO;

public abstract class DataLoader
{
    protected CTFAKContext Context=>CTFAKContext.Current;
    public abstract void Read(ByteReader reader);
    public abstract void Write(ByteWriter writer);

}