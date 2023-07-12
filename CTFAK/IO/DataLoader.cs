using CTFAK.Memory;
using CTFAK.Utils;
using EasyNetLog;

namespace CTFAK.IO;

public abstract class DataLoader
{
    protected CTFAKContext Context => CTFAKContext.Current;
    protected EasyNetLogger Logger => Context.Logger;
    public abstract void Read(ByteReader reader);
    public abstract void Write(ByteWriter writer);

}