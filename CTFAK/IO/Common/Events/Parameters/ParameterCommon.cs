using CTFAK.Memory;

namespace CTFAK.IO.Common.Events;

public class ParameterCommon : DataLoader
{
    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException("Unknown parameter: " + GetType().Name);
    }

    public override void Read(ByteReader reader)
    {
    }
}