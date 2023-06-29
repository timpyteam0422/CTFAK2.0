using CTFAK.IO.CCN.Chunks;
using CTFAK.IO.CCN.Chunks.Frame;
using CTFAK.IO.CCN.Chunks.Objects;
using CTFAK.IO.Common.Banks;
using CTFAK.IO.Common.Banks.ImageBank;
using CTFAK.IO.Common.Banks.SoundBank;
using CTFAK.IO.EXE;
using CTFAK.Memory;
using CTFAK.Utils;

namespace CTFAK.IO.CCN;

public class GameData : DataLoader
{
    public AppHeader Header;
    public string Name;
    public string Author;
    public AppMenu Menu;
    public Dictionary<int, ObjectInfo> FrameItems = new();
    public FrameHandles FrameHandles;
    public ExtData ExtData;
    public string EditorFilename;
    public string TargetFilename;
    public GlobalValues GlobalValues;
    public GlobalStrings GlobalStrings;
    public Extensions Extensions;
    //public Bitmap Icon32X;
    public BinaryFiles BinaryFiles = new();
    public string AboutText;
    public string Copyright;
    public bool ExeOnly;
    public Shaders Shaders;
    public ExtendedHeader ExtHeader;
    public List<Frame> Frames = new();
    public ImageShapes ImageShapes;
    public ImageBank Images = new();
    public FontBank Fonts;
    public SoundBank Sounds;
    public MusicBank Music;



    public string Doc;



    public PackData PackData; //read-only. not actually stored in gamedata structure
    public int productBuild;
    public int productVersion;
    public short runtimeSubversion;


    public short runtimeVersion;

    public ChunkList Chunks;


    public static event SaveHandler OnChunkLoaded;
    public static event SaveHandler OnFrameLoaded;

    public override void Read(ByteReader reader)
    {

        Logger.Log("<color=green>Reading GameData</color>");
        var magic = reader.ReadAscii(4);

        // We don't really care about that stuff, because the reader will automatically figure out if the game uses unicode by checking some stuff in the AppName chunk
        if (magic == "PAMU")
            Context.Unicode = true;
        else if (magic == "PAME")
            Context.Unicode = false;
        //else if (magic == "CRUF")
        //     Settings.gameType |= Settings.GameType.F3;
        else Logger.LogWarning("Couldn't found any known headers: " + magic);



        runtimeVersion = reader.ReadInt16();
        runtimeSubversion = reader.ReadInt16();
        productVersion = reader.ReadInt32();
        productBuild = reader.ReadInt32();
        Context.BuildNumber = productBuild;

        Logger.Log("Fusion Build: " + productBuild);

        Chunks = new ChunkList();
        Chunks.OnChunkLoaded += (id, loader) =>
        {
            switch (id)
            {
                case 8739: //AppHeader
                    Header = loader as AppHeader;
                    break;
                case 8740: //AppName
                    Name = (loader as AppName)?.Value;
                    break;
                case 8741: //AppAuthor
                    Author = (loader as AppAuthor)?.Value;
                    break;
                case 8742: //AppMenu
                    Menu = loader as AppMenu;
                    break;
                case 8744: //Extensions
                    break;
                case 8745: //FrameItems
                    FrameItems = (loader as FrameItems)?.Items;
                    break;
                case 8746: //GlobalEvents
                    break;
                case 8747: //FrameHandler
                    FrameHandles = loader as FrameHandles;
                    break;
                case 8748: //ExtData
                    ExtData = loader as ExtData;
                    break;
                case 8749: //AdditionalExtension
                    break;
                case 8750: //AppEditorFilename
                    EditorFilename = (loader as EditorFilename)?.Value;
                    if (Context.BuildNumber > 284)
                        Decryption.MakeKey(Name, Copyright, EditorFilename);
                    else
                        Decryption.MakeKey(EditorFilename, Name, Copyright);
                    break;
                case 8751: //AppTargetFilename
                    TargetFilename = (loader as TargetFilename)?.Value;
                    break;
                case 8752: //AppDoc
                    break;
                case 8753: //OtherExts
                    break;
                case 8754: //GlobalValues
                    GlobalValues = loader as GlobalValues;
                    break;
                case 8755: //GlobalStrings
                    GlobalStrings = loader as GlobalStrings;
                    break;
                case 8756: //Extensions2
                    Extensions = loader as Extensions;
                    break;
                case 8757: //AppIcon
                    //Icon32X = (loader as AppIcon).Icon;
                    break;
                case 8758: //DemoVersion
                    break;
                case 8759: //SecNum
                    break;
                case 8760: //BinaryFiles
                    BinaryFiles = loader as BinaryFiles;
                    break;
                case 8761: //AppMenuImages:
                    break;
                case 8762: //AboutText
                    AboutText = (loader as AboutText)?.Value;
                    break;
                case 8763: //Copyright
                    Copyright = (loader as Copyright)?.Value;
                    break;
                case 8764: //GlobalValueNames
                    break;
                case 8765: //GlobalStringNames
                    break;
                case 8766: //MvtTexts
                    break;
                case 8767: //FrameItems2
                    FrameItems = (loader as FrameItems2)?.Items;
                    break;
                case 8768: //ExeOnly
                    //var exeOnly = loader as ExeOnly;
                    //ExeOnly = exeOnly.exeOnly;
                    break;
                case 8771:
                    Shaders = loader as Shaders;
                    break;
                case 8773: //ExtendedHeader
                    ExtHeader = loader as ExtendedHeader;
                    break;
                case 8792: //FontBank
                    Fonts = loader as FontBank;
                    break;
                case 8793: //FontBank
                    Fonts = loader as FontBank;
                    break;
                case 13107: //Frame
                    Frames.Add(loader as Frame);
                    break;
                case 17664: //ImageShapes
                    ImageShapes = loader as ImageShapes;
                    break;
                case 26214: //ImageBank
                    Images = loader as ImageBank;
                    break;
                case 26215: //FontBank
                    Fonts = loader as FontBank;
                    break;
                case 26216: //SoundBank
                    Sounds = loader as SoundBank;
                    /*if (CTFAKCore.Parameters.Contains("-nosounds")) break;
                    if (CTFAKContext.Current.Android)
                    {
                        Sounds = ApkFileReader.AndroidSoundBank;
                        var androidSounds = loader as AndroidSoundBank;
                        for (var i = 0; i < Sounds.Items.Count; i++)
                            Sounds.Items[i].Name = androidSounds.Items[Sounds.Items[i].Handle].Name;
                    }
                    else
                    {
                        
                    }*/

                    break;
                case 8790: //TwoFivePlusProperties
                    //FrameItems = TwoFilePlusContainer.Instance.ObjectsContainer;
                    break;
            }
        };
        Chunks.Read(reader);
        // reading again if we encounter an F3 game that uses a separate chunk list for images and sounds
        // it's safe to just read again
        //chunkList.Read(reader); // turns out it's not
        if (reader.Tell() < reader.Size()) Chunks.Read(reader); // turns out i actually gotta check some stuff
        if (CTFAKCore.Parameters.Contains("-debug"))
            Console.ReadLine();
    }

    public override void Write(ByteWriter writer)
    {

    }
}