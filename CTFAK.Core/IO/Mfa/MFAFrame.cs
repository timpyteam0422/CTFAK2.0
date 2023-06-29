using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CTFAK.IO.CCN;
using CTFAK.Memory;

namespace CTFAK.IO.MFA;

public class MFAFrame : DataLoader
{
    public int Handle;
    public string Name = "ERROR";
    public int SizeX;
    public int SizeY;
    public Color Background;

    public BitDict Flags = new(new[]
    {
        "GrabDesktop",
        "KeepDisplay",
        "BackgroundCollisions",
        "DisplayFrameTitle",
        "ResizeToScreen",
        "ForceLoadOnCall",
        "NoDisplaySurface",
        "ScreenSaverSetup",
        "TimerBasedMovements",
        "MochiAds",
        "NoGlobalEvents"
    });

    public int MaxObjects;
    public byte[] Password = new byte[12];
    public int LastViewedX;
    public int LastViewedY;
    public List<Color> Palette = new Color[256].ToList();
    public int StampHandle;
    public int ActiveLayer;
    public List<MFALayer> Layers = new();
    public MFATransition FadeIn;
    public MFATransition FadeOut;
    public List<MFAObjectInfo> Items = new();
    public List<MFAItemFolder> Folders = new();
    public List<MFAObjectInstance> Instances = new();
    public MFAChunkList Chunks;
    public MFAEvents Events;

    public override void Write(ByteWriter writer)
    {
        writer.WriteInt32(Handle);
        writer.AutoWriteUnicode(Name);
        writer.WriteInt32(SizeX);
        writer.WriteInt32(SizeY);
        writer.WriteColor(Background);

        writer.WriteUInt32(Flags.Flag);
        writer.WriteInt32(MaxObjects);

        writer.Skip(3);
        writer.WriteInt8(128);
        writer.WriteInt32(Password.Length);
        writer.WriteBytes(Password);

        writer.WriteInt32(LastViewedX);
        writer.WriteInt32(LastViewedY);
        writer.WriteInt32(Palette.Count); //WTF HELP 

        foreach (var item in Palette) writer.WriteColor(item);

        writer.WriteInt32(StampHandle);
        writer.WriteInt32(ActiveLayer);
        writer.WriteInt32(Layers.Count);
        foreach (var layer in Layers) layer.Write(writer);

        if (FadeIn != null)
        {
            writer.WriteInt8(1);
            FadeIn.Write(writer);
        }
        else
        {
            writer.Skip(1);
        }

        if (FadeOut != null)
        {
            writer.WriteInt8(1);
            FadeOut.Write(writer);
        }
        else
        {
            writer.Skip(1);
        }


        writer.WriteInt32(Items.Count);
        foreach (var item in Items) item.Write(writer);

        writer.WriteInt32(Folders.Count);
        foreach (var item in Folders) item.Write(writer);

        writer.WriteInt32(Instances.Count);
        foreach (var item in Instances) item.Write(writer);

        Events.Write(writer);
        Chunks.Write(writer);
    }

    public override void Read(ByteReader reader)
    {
        Handle = reader.ReadInt32();
        Name = reader.AutoReadUnicode();
        SizeX = reader.ReadInt32();
        SizeY = reader.ReadInt32();
        Background = reader.ReadColor();
        Flags.Flag = reader.ReadUInt32();

        MaxObjects = reader.ReadInt32();

        reader.ReadInt32();
        Password = reader.ReadBytes(reader.ReadInt32());

        LastViewedX = reader.ReadInt32();
        LastViewedY = reader.ReadInt32();

        var paletteSize = reader.ReadInt32();
        Palette = new List<Color>();
        for (var i = 0; i < paletteSize; i++) Palette.Add(reader.ReadColor());

        StampHandle = reader.ReadInt32();
        ActiveLayer = reader.ReadInt32();
        var layersCount = reader.ReadInt32();
        for (var i = 0; i < layersCount; i++)
        {
            var layer = new MFALayer();
            layer.Read(reader);
            Layers.Add(layer);
        }

        if (reader.ReadByte() == 1)
        {
            FadeIn = new MFATransition();
            FadeIn.Read(reader);
        }

        if (reader.ReadByte() == 1)
        {
            FadeOut = new MFATransition();
            FadeOut.Read(reader);
        }

        var frameItemsCount = reader.ReadInt32();
        for (var i = 0; i < frameItemsCount; i++)
        {
            var frameitem = new MFAObjectInfo();
            frameitem.Read(reader);

            Items.Add(frameitem);
        }

        var folderCount = reader.ReadInt32();
        for (var i = 0; i < folderCount; i++)
        {
            var folder = new MFAItemFolder();
            folder.Read(reader);
            Folders.Add(folder);
        }

        var instancesCount = reader.ReadInt32();
        for (var i = 0; i < instancesCount; i++)
        {
            var inst = new MFAObjectInstance();
            inst.Read(reader);
            Instances.Add(inst);
        }

        // var unkCount = reader.ReadInt32();
        // for (int i = 0; i < unkCount; i++)
        // {
        // UnkBlocks.Add(reader.ReadBytes(32));
        // }

        Events = new MFAEvents();
        Events.Read(reader);

        Chunks = new MFAChunkList();
        // Chunks.Log = true;
        Chunks.Read(reader);
    }
}