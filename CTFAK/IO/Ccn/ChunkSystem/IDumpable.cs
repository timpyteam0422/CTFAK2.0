namespace CTFAK.IO.Ccn.ChunkSystem;

public interface IDumpable
{
    public MemoryStream DumpToMemoryStream(); // TODO Might wanna pass the stream as a function argument
    public string OutputName { get; }
    public string TypeName { get; }
    public string FileExtension { get; }
}