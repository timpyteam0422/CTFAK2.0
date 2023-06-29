using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CTFAK.Attributes;
using CTFAK.IO.CCN;
using CTFAK.Memory;
using CTFAK.Utils;

namespace CTFAK.IO.Common.Banks.ImageBank;

[ChunkLoader(26214, "ImageBank")]
public class ImageBank : Chunk
{
    public Dictionary<int, FusionImage> Items = new();

    public static List<Task> imageReadingTasks = new();

    public static List<Task> imageWritingTasks = new();
    
    public static event SaveHandler OnImageLoaded;


    public static FusionImage CreateImage()
    {
        if (CTFAKContext.Current.Android)
            return new AndroidImage();
        if (CTFAKContext.Current.TwoFivePlus)
            return new TwoFivePlusImage();
        if (CTFAKContext.Current.Old)
            return new MMFImage();
        return new NormalImage();
    }

    public override void Read(ByteReader reader)
    {
        // tysm LAK
        // Yuni asked my to add this back
        // This comment doesn't belong here, but I'm still keeping it
        if (CTFAKCore.Parameters.Contains("-noimg")) return;

        var count = 0;

        if (Context.Android)
        {
            var maxHandle = reader.ReadInt16();
            count = reader.ReadInt16();
        }
        else
        {
            count = reader.ReadInt32();
        }

        for (var i = 0; i < count; i++)
        {
            var newImg = CreateImage();
            newImg.Read(reader);
            OnImageLoaded?.Invoke(i, count);
            Items.Add(newImg.Handle, newImg);
        }
        
        foreach (var task in imageReadingTasks) task.Wait();
        imageReadingTasks.Clear();
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}