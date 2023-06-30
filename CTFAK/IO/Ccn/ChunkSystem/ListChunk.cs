using System.Collections;
using CTFAK.Memory;

namespace CTFAK.IO.Ccn.ChunkSystem;

public abstract class ListChunk : Chunk,ICollection<DataLoader>
{
    protected List<DataLoader> items = new List<DataLoader>();

    public IEnumerator<DataLoader> GetEnumerator()=>items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()=>GetEnumerator();
    public void Add(DataLoader item)=>items.Add(item);
    public void Clear()=>items.Clear();
    public bool Contains(DataLoader item)=>items.Contains(item);
    public void CopyTo(DataLoader[] array, int arrayIndex)=>items.CopyTo(array,arrayIndex);
    public bool Remove(DataLoader item)=>items.Remove(item);
    public int Count => items.Count;
    public bool IsReadOnly => false;
}
public abstract class ListChunk<T> : ListChunk,ICollection<T> where T : DataLoader, new()
{
    public override void Read(ByteReader reader)
    {
        var count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var item = new T();
            item.Read(reader);
            items.Add(item);
        }

    }
    public override void Write(ByteWriter writer)
    {
        writer.WriteInt32(items.Count);
        foreach (var item in items)
            item.Write(writer);
    }
    public IEnumerator<T> GetEnumerator()=>items.GetEnumerator() as IEnumerator<T>;
    IEnumerator IEnumerable.GetEnumerator()=>GetEnumerator();
    public void Add(T item)=>items.Add(item);
    public bool Contains(T item)=>items.Contains(item);
    public void CopyTo(T[] array, int arrayIndex)=>items.CopyTo(array,arrayIndex);
    public bool Remove(T item)=>items.Remove(item);

}