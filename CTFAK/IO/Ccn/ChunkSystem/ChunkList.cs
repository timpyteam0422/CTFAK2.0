//#define SLIM

using CTFAK.Attributes;
using CTFAK.Memory;
using CTFAK.Utils;
using System.Reflection;

namespace CTFAK.IO.Ccn.ChunkSystem;



public class ChunkLoaderData
{
    public short ChunkId;
    public string ChunkName;
    public Type LoaderType;
}

public class ChunkList
{
#if !SLIM
    public static readonly Dictionary<int, ChunkLoaderData> KnownLoaders = new();
#endif

    public List<Chunk> Items = new();
    public DataLoader Parent;

    public delegate void OnChunkLoadedEvent(int chunkId, Chunk loader);

    public delegate void HandleChunkEvent(int chunkId, Chunk loader);


    public event OnChunkLoadedEvent OnChunkLoaded;

    static ChunkList()
    {
        Init();

    }
    public ChunkList(DataLoader parent)
    {
        Parent = parent;
    }

    public ChunkList()
    {

    }
    public static Chunk CreateChunk(int id)
    {
#if SLIM
        switch (id)
        {
            case 8739:
                return new AppHeader();
            case 8740:
                return new AppMenu();
            case 8741:
                return new AppAuthor();
            case 8742:
                return new AppMenu();
            case 8743:
                return new ExtPath();
            case 8744:
                return new Extensions();
            case 8745:
                return new FrameItems();
            case 8746:
                return null; // GlobalEvents. Probably used for temporary CCN files when running in editor
            case 8747:
                return new FrameHandles();
            case 8748:
                return new ExtData();
            case 8749:
                return new Extension();
            case 8750:
                return new EditorFilename();
            case 8751:
                return new TargetFilename();
            case 8752:
                return new AppDoc();
            case 8753:
                return null; // OtherExts
            case 8754:
                return new GlobalValues();
            case 8766:
                return new GlobalStrings();
            case 8756:
                return new Extensions();
            case 8758:
                return null; // DemoVersion
            case 8759:
                return new SecNum();
            
            default:
                return null;
            
        }
#else
        ChunkLoaderData loaderData;
        if (KnownLoaders.TryGetValue(id, out loaderData))
            return Activator.CreateInstance(loaderData.LoaderType) as Chunk;
        else return new UnknownChunk();
#endif
    }

    public static string GetChunkName(int id)
    {
#if SLIM
return $"Unknown-{id}";
#else
        ChunkLoaderData loaderData;
        if (KnownLoaders.TryGetValue(id, out loaderData))
            return loaderData.ChunkName;
        else return $"Unknown-{id}";
#endif
    }


    public static void Init()
    {
#if !SLIM
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var asm in assemblies)
            try
            {
                foreach (var type in asm.GetTypes())
                    if (type.GetCustomAttributes().Any(a => a.GetType() == typeof(ChunkLoaderAttribute)))
                    {
                        var attribute =
                            type.GetCustomAttributes().First(a => a.GetType() == typeof(ChunkLoaderAttribute)) as
                                ChunkLoaderAttribute;
                        var newChunkLoaderData = new ChunkLoaderData();
                        newChunkLoaderData.LoaderType = type;
                        newChunkLoaderData.ChunkId = attribute.ChunkId;
                        newChunkLoaderData.ChunkName = attribute.ChunkName;


                        Logger.Log(
                            $"Found chunk loader handler for chunk id <color=lightblue>{newChunkLoaderData.ChunkId}</color> with name \"<color=lightblue>{newChunkLoaderData.ChunkName}</color>\"");
                        if (!KnownLoaders.ContainsKey(newChunkLoaderData.ChunkId))
                            KnownLoaders.Add(newChunkLoaderData.ChunkId, newChunkLoaderData);
                        else
                            Logger.LogWarning("Multiple loaders are getting registered for chunk: " +
                                              newChunkLoaderData.ChunkId);
                    }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error white loading chunk loaders: " + ex);
            }
#endif
    }

    public void Read(ByteReader reader)
    {
        while (true)
        {
            if (reader.Tell() >= reader.Size()) break;
            var chunkId = reader.PeekUInt16();
            var newChunk = CreateChunk(chunkId);


            if (newChunk is ChildChunk child)
            {
                child.parent = Parent;
            }

            if (newChunk is UnknownChunk)
            {
                Logger.Log($"No loader found for chunk <color=yellow>{chunkId}</color>");
            }
            else
            {
                Logger.Log($"Loading chunk <color=lightblue>{GetChunkName(chunkId)}</color> ({chunkId})");
            }


            newChunk.ReadAndLoad(reader);


            OnChunkLoaded?.Invoke(newChunk.Id, newChunk);

            if (newChunk.Id == 32639) break;
            if (newChunk.Id == 8787) CTFAKContext.Current.TwoFivePlus = true;


            Items.Add(newChunk);

        }
    }

    public void Write(ByteWriter writer)
    {
        foreach (var chk in Items)
        {
            chk.CompressAndWrite(writer);
        }
    }


    public static readonly Dictionary<int, string> ChunkNames = new()
    {
        { 4386, "Preview" },
        { 8738, "AppMiniHeader" },
        { 8739, "AppHeader" },
        { 8740, "AppName" },
        { 8741, "AppAuthor" },
        { 8742, "AppMenu" },
        { 8743, "ExtPath" },
        { 8744, "Extensions" },
        { 8745, "FrameItems" },
        { 8746, "GlobalEvents" },
        { 8747, "FrameHandles" },
        { 8748, "ExtData" },
        { 8749, "AdditionalExtension" },
        { 8750, "AppEditorFilename" },
        { 8751, "AppTargetFilename" },
        { 8752, "AppDoc" },
        { 8753, "OtherExts" },
        { 8754, "GlobalValues" },
        { 8755, "GlobalStrings" },
        { 8756, "Extensions2" },
        { 8757, "AppIcon" },
        { 8758, "DemoVersion" },
        { 8759, "SecNum" },
        { 8760, "BinaryFiles" },
        { 8761, "AppMenuImages" },
        { 8762, "AboutText" },
        { 8763, "Copyright" },
        { 8764, "GlobalValueNames" },
        { 8765, "GlobalStringNames" },
        { 8766, "MvtExts" },
        { 8767, "FrameItems2" },
        { 8768, "ExeOnly" },
        { 8770, "Protection" },
        { 8771, "Shaders" },
        { 8773, "AppHeader2" },
        { 8792, "TTFFonts" },
        { 13107, "Frame" },
        { 13108, "FrameHeader" },
        { 13109, "FrameName" },
        { 13110, "FramePassword" },
        { 13111, "FramePalette" },
        { 13112, "FrameItemInstances" },
        { 13113, "FrameFadeInFrame" },
        { 13114, "FrameFadeOutFrame" },
        { 13115, "FrameFadeIn" },
        { 13116, "FrameFadeOut" },
        { 13117, "FrameEvents" },
        { 13118, "FramePlayHeader" },
        { 13119, "Additional_FrameItem" },
        { 13120, "Additional_FrameItemInstance" },
        { 13121, "FrameLayers" },
        { 13122, "FrameVirtualLayers" },
        { 13123, "DemoFilePath" },
        { 13124, "RandomSeed" },
        { 13125, "FrameLayerEffects" },
        { 13126, "BlurayFrameOptions" },
        { 13127, "MVTTimerBase" },
        { 13128, "MosaicImageTable" },
        { 13129, "FrameEffects" },
        { 13130, "FrameIPhoneOptions" },
        { 17476, "OIHeader" },
        { 17477, "OIName" },
        { 17478, "OIProperties" },
        { 17479, "OIUnknown" },
        { 17480, "OIEffects" },
        { 21845, "ImageOffsets" },
        { 21846, "FontOffsets" },
        { 21847, "SoundOffsets" },
        { 21848, "MusicOffsets" },
        { 26214, "Images" },
        { 26215, "Fonts" },
        { 26216, "Sounds" },
        { 26217, "Musics" },
        { 32639, "Last" }
    };
}
