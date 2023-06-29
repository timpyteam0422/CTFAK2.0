using CTFAK.Memory;

namespace CTFAK.IO.Common.Events;

public class Filename : StringParam
{
    public override void Write(ByteWriter writer)
    {
        writer.WriteUnicode(Value);
    }
}