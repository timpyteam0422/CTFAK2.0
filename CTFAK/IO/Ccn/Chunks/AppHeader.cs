using CTFAK.Attributes;
using CTFAK.Memory;
using System.Drawing;

namespace CTFAK.IO.CCN.Chunks;

[ChunkLoader(0x2223, "AppHeader")]
public class AppHeader : Chunk
{
    public BitDict Flags = new(new[]
    {
        "HeadingMaximized",
        "NoHeading",
        "FitInsideBars",
        "MachineIndependentSpeed",
        "ResizeDisplay",
        "MusicOn",
        "SoundOn",
        "DontDisplayMenu",
        "MenuBar",
        "MaximizedOnBoot",
        "MultiSamples",
        "ChangeResolutionMode",
        "SwitchToFromFullscreen",
        "Protected",
        "Copyright",
        "OneFile"
    });
    public BitDict NewFlags = new(new[]
    {
        "SamplesOverFrames",
        "RelocFiles",
        "RunFrame",
        "PlaySamplesWhenUnfocused",
        "NoMinimizeBox",
        "NoMaximizeBox",
        "NoThickFrame",
        "DoNotCenterFrame",
        "IgnoreInputOnScreensaver",
        "DisableClose",
        "HiddenAtStart",
        "VisualThemes",
        "VSync",
        "RunWhenMinimized",
        "MDI",
        "RunWhileResizing"
    });
    public short GraphicsMode;
    public BitDict OtherFlags = new(new[]
    {
        "DebuggerShortcuts",
        "Unknown1",
        "Unknown2",
        "DontShareSubData",
        "Unknown3",
        "Unknown4",
        "Unknown5",
        "ShowDebugger",
        "Unknown6",
        "Unknown7",
        "Unknown8",
        "Unknown9",
        "Unknown10",
        "Unknown11",
        "Direct3D9or11",
        "Direct3D8or11"
    });

    public short WindowWidth;
    public short WindowHeight;
    public int InitialScore;
    public int InitialLives;
    public Controls Controls;
    public Color BorderColor;
    public int NumberOfFrames;
    public int FrameRate;
    public int WindowsMenuIndex;


    public override void Read(ByteReader reader)
    {
        if (!Context.Old)
        {
            var size = reader.ReadInt32();
        }
        Flags.Flag = reader.ReadUInt16();
        NewFlags.Flag = reader.ReadUInt16();
        GraphicsMode = reader.ReadInt16();
        OtherFlags.Flag = reader.ReadUInt16();
        WindowWidth = reader.ReadInt16();
        WindowHeight = reader.ReadInt16();
        InitialScore = (int)(reader.ReadUInt32() ^ 0xffffffff);
        InitialLives = (int)(reader.ReadUInt32() ^ 0xffffffff);
        Controls = new Controls();
        if (Context.Old) reader.Skip(56);
        else Controls.Read(reader);
        BorderColor = reader.ReadColor();
        NumberOfFrames = reader.ReadInt32();
        if (Context.Old) return;
        FrameRate = reader.ReadInt32();
        WindowsMenuIndex = reader.ReadInt32();
    }

    public override void Write(ByteWriter writer)
    {
        var dataWriter = new ByteWriter(new MemoryStream());
        dataWriter.WriteUInt16((ushort)Flags.Flag);
        dataWriter.WriteUInt16((ushort)NewFlags.Flag);
        dataWriter.WriteInt16(GraphicsMode);
        dataWriter.WriteInt16((short)OtherFlags.Flag);
        dataWriter.WriteInt16(WindowWidth);
        dataWriter.WriteInt16(WindowHeight);
        dataWriter.WriteInt32((int)(InitialScore ^ 0xffffffff));
        dataWriter.WriteInt32((int)(InitialLives ^ 0xffffffff));
        Controls.Write(dataWriter);
        dataWriter.WriteColor(BorderColor);
        dataWriter.WriteInt32(NumberOfFrames);
        dataWriter.WriteInt32(WindowsMenuIndex);
        writer.WriteInt32((int)dataWriter.Tell());
        writer.WriteWriter(dataWriter);
    }
}

public class Controls : DataLoader
{
    public List<PlayerControl> Items;

    public override void Read(ByteReader reader)
    {
        Items = new List<PlayerControl>();
        for (var i = 0; i < 4; i++)
        {
            var item = new PlayerControl(reader);
            Items.Add(item);
            item.Read();
        }
    }

    public override void Write(ByteWriter writer)
    {
        foreach (var control in Items) control.Write(writer);
    }
}

public class PlayerControl
{
    private readonly ByteReader _reader;
    private int _controlType;
    private Keys _keys;

    public PlayerControl(ByteReader reader)
    {
        _reader = reader;
    }

    public void Read()
    {
        _keys = new Keys(_reader);
        _controlType = _reader.ReadInt16();
        _keys.Read();
    }

    public void Write(ByteWriter writer)
    {
        writer.WriteInt16((short)_controlType);
        _keys.Write(writer);
    }
}

public class Keys
{
    private readonly ByteReader _reader;
    private short _button1;
    private short _button2;
    private short _button3;
    private short _button4;
    private short _down;
    private short _left;
    private short _right;
    private short _up;

    public Keys(ByteReader reader)
    {
        _reader = reader;
    }

    public void Read()
    {
        _up = _reader.ReadInt16();
        _down = _reader.ReadInt16();
        _left = _reader.ReadInt16();
        _right = _reader.ReadInt16();
        _button1 = _reader.ReadInt16();
        _button2 = _reader.ReadInt16();
        //if (Settings.GameType == GameType.OnePointFive) return;
        _button3 = _reader.ReadInt16();
        _button4 = _reader.ReadInt16();
    }

    public void Write(ByteWriter writer)
    {
        writer.WriteInt16(_up);
        writer.WriteInt16(_down);
        writer.WriteInt16(_left);
        writer.WriteInt16(_right);
        writer.WriteInt16(_button1);
        writer.WriteInt16(_button2);
        writer.WriteInt16(_button3);
        writer.WriteInt16(_button4);
    }
}