using System.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using CTFAK.GUI.PluginSystem;
using CTFAK.IO;
using CTFAK.IO.CCN;
using CTFAK.IO.CCN.Chunks;
using CTFAK.IO.CCN.Chunks.Frame;
using CTFAK.Utils;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Avalonia.Layout;
using Avalonia.Platform.Storage;
using CTFAK.IO.CCN.Chunks.Objects;
using CTFAK.IO.Ccn.ChunkSystem;
using CTFAK.IO.Common.Banks.SoundBank;
using CTFAK.IO.Exe;
using CTFAK.IO.EXE;

namespace CTFAK.GUI;

public partial class MainWindow : Window
{
    public static MainWindow Instance;
    public GameFile CurrentFile;
    public MainWindow()
    {
        Instance = this;
        InitializeComponent();
        PluginList.SelectionChanged += (o, e) =>
        {
            var plugin = (PluginList.SelectedItem as Control)?.Tag as IPlugin;
            PluginPanel.Children.Clear();
            PluginPanel.Children.Add(plugin as UserControl);
        };
        PackDataFiles.SelectionChanged += (o, e) =>
        {
            var packFile = (PackDataFiles.SelectedItem as Control)?.Tag as PackFile;
            PackDataFileDetails.Text = $"Name: {packFile.PackFilename}\nSize: {packFile.Data.Length.ToPrettySize()}";
        };
        

    }


    private void SelectFile_OnClick(object? sender, RoutedEventArgs e)
    {
        var fileSelector = new FileSelectorWindow();
        fileSelector.Show(this);
    }



    protected override void OnLoaded()
    {
        base.OnLoaded();
        if (!Design.IsDesignMode)
        {
            CTFAKCore.Init();

            VersionText.Text = $"CTFAK build hash: {CTFAKCore.GetVersion()}";
            Directory.CreateDirectory("Plugins");
            var files = Directory.GetFiles("Plugins", "*.dll");
            foreach (var file in files)
            {
                var asm = Assembly.Load(File.ReadAllBytes(file));
                var types = asm.GetTypes();
                foreach (var type in types)
                {
                    if (type.GetInterface(typeof(IPlugin).FullName) != null)
                    {
                        try
                        {
                            IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                            var item = new TextBlock();
                            item.Text = plugin.Name;
                            item.Tag = plugin;
                            PluginList.Items.Add(item);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("Error while loading plugins: " + ex);
                        }
                    }
                }
            }

        }


    }


    public TreeViewItem CreateTreeEntryForDataLoader(DataLoader loader,string name = "Unknown")
    {
        var treeViewItem = new TreeViewItem();
        treeViewItem.Header = name;
        treeViewItem.Tag = loader;
        if (loader is Chunk chk)
        {
            treeViewItem.Header = ChunkList.GetChunkName(chk.Id);
            if (chk is ListChunk list)
            {
                foreach (var item in list)
                {
                    treeViewItem.Items.Add(CreateTreeEntryForDataLoader(item));
                }
            }
            if (chk is BankChunk bank)
            {
                foreach (var item in bank.Chunks.Items)
                {
                    treeViewItem.Items.Add(CreateTreeEntryForDataLoader(item));
                }
            }

            if (chk.GetType().BaseType.IsGenericType)
            {
                if (chk.GetType().BaseType.GetGenericTypeDefinition() == typeof(DictChunk<,>))
                {
                    var dict = chk.GetType().GetField("Items").GetValue(chk) as IDictionary;
                    foreach (DictionaryEntry item in dict)
                    {
                        treeViewItem.Items.Add(CreateTreeEntryForDataLoader(item.Value as DataLoader));
                    }
                }
            }
            
            
            if (chk is Frame frmLoader)
            {
                treeViewItem.Header = $"Frame \"{frmLoader.Name}\"";
            }
            if (loader is ObjectInfo oi)
                treeViewItem.Header = oi.Name;
            else if (loader is ObjectProperties propChunk)
            {
                var props = propChunk.Properties;
                if (props is ObjectCommon common)
                {
                    if (common.Animations != null)
                        treeViewItem.Items.Add(CreateTreeEntryForDataLoader(common.Animations, "Animations"));
                    if (common.Movements != null)
                        treeViewItem.Items.Add(CreateTreeEntryForDataLoader(common.Movements, "Movements"));
                    if (common.Counters != null)
                        treeViewItem.Items.Add(CreateTreeEntryForDataLoader(common.Counters, "Counters"));
                    if (common.Counter != null)
                        treeViewItem.Items.Add(CreateTreeEntryForDataLoader(common.Counter, "Counter"));
                    if (common.Text != null)
                        treeViewItem.Items.Add(CreateTreeEntryForDataLoader(common.Text, "Text"));

                }
            }
        }
        else
        {
            if (loader is Extension ext)
                treeViewItem.Header = ext.Name;
            else if (loader is SoundItem snd)
                treeViewItem.Header = snd.Name;
        }
        
        return treeViewItem;
    }
    public void StartLoadingGame(string path)
    {

        SetStatus("Loading...", 0);
        var backgroundWorker = new BackgroundWorker();
        backgroundWorker.DoWork += (o, e) =>
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            CurrentFile = LoadHelper.LoadGameFromPath(path);
            stopwatch.Stop();
            Logger.Log($"Initial loading finished in {stopwatch.Elapsed.TotalSeconds} seconds");
        };
        backgroundWorker.RunWorkerCompleted += (o, e) =>
        {
            var game = CurrentFile.GameData;
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine($"Name: {game.Name}");
            strBuilder.AppendLine($"Author: {game.Author}");
            strBuilder.AppendLine($"Copyright: {game.Copyright}");
            strBuilder.AppendLine($"Number of frames: {game.Frames.Count}");
            strBuilder.AppendLine($"Number of objects: {game.FrameItems.Count}");
            strBuilder.AppendLine($"Number of images: {game.Images.Items.Count}");
            strBuilder.AppendLine($"Number of sounds: {game.Sounds.Count}");
            strBuilder.AppendLine($"Game build type: {CTFAKContext.Current.BuildType}");
            GameInfoText.Text = strBuilder.ToString();
            ChunkDetails.Items.Clear();
            ChunkTree.Items.Clear();
            var chunks = game.Chunks;
            foreach (var chk in chunks.Items)
            {
                ChunkTree.Items.Add(CreateTreeEntryForDataLoader(chk));
            }

            if (CurrentFile is ExeFile exe)
            {
                foreach (var packFile in exe.PackData.Items)
                {
                    PackDataFiles.Items.Add(new TextBlock() { Text = packFile.PackFilename, Tag = packFile });
                }

                PackDataFiles.SelectedIndex = 0;
            }


        };
        backgroundWorker.RunWorkerAsync();
    }

    public void DisplayDataLoader(DataLoader loader)
    {
        ChunkDetails.Items.Clear();

        if (loader is Chunk chk)
        {
            ChunkDetails.Items.Add(new TextBlock() { Text = $"Name: {ChunkList.GetChunkName(chk.Id)}" });
            ChunkDetails.Items.Add(new TextBlock() { Text = $"Loader: {chk?.GetType().Name ?? "None"}" });
            ChunkDetails.Items.Add(new TextBlock() { Text = $"Flag: {chk.Flag}" });
            ChunkDetails.Items.Add(new TextBlock() { Text = $"File offset: 0x{chk.FileOffset.ToString("X4")}" });
            ChunkDetails.Items.Add(new TextBlock() { Text = $"File size: {chk.FileSize.ToPrettySize()}" });
            ChunkDetails.Items.Add(new TextBlock() { Text = $"Unpacked size: {chk.UnpackedSize.ToPrettySize()}" });
            ChunkDetails.Items.Add(new TextBlock());
        }
        


        if (loader is StringChunk strChk)
        {
            ChunkDetails.Items.Add(new TextBlock() { Text = $"Contents: {strChk.Value}", TextWrapping = TextWrapping.WrapWithOverflow });
        }
        else if (loader is AppHeader hdrChk)
        {
            ChunkDetails.Items.Add(new TextBlock() { Text = $"Screen Resolution: {hdrChk.WindowWidth}x{hdrChk.WindowHeight}", TextWrapping = TextWrapping.WrapWithOverflow });
            ChunkDetails.Items.Add(new TextBlock() { Text = $"Initial Lives: {hdrChk.InitialLives}", TextWrapping = TextWrapping.WrapWithOverflow });
            ChunkDetails.Items.Add(new TextBlock() { Text = $"Initial Score: {hdrChk.InitialScore}", TextWrapping = TextWrapping.WrapWithOverflow });
            ChunkDetails.Items.Add(new TextBlock() { Text = $"Flags: {hdrChk.Flags}", TextWrapping = TextWrapping.WrapWithOverflow });
            ChunkDetails.Items.Add(new TextBlock() { Text = $"New flags: {hdrChk.NewFlags}", TextWrapping = TextWrapping.WrapWithOverflow });
            ChunkDetails.Items.Add(new TextBlock() { Text = $"Other flags: {hdrChk.OtherFlags}", TextWrapping = TextWrapping.WrapWithOverflow });
        }
        else if (loader is Frame frmChk)
        {
            ChunkDetails.Items.Add(new TextBlock() { Text = $"Frame size: {frmChk.Width}x{frmChk.Height}", TextWrapping = TextWrapping.WrapWithOverflow });
            ChunkDetails.Items.Add(new TextBlock() { Text = $"Flags: {frmChk.Flags}", TextWrapping = TextWrapping.WrapWithOverflow });
        }

        if (loader is Extension ext)
        {
            ChunkDetails.Items.Add(new TextBlock() { Text = $"Extension name: {ext.Name}" });
        }

        if (loader is Animations anims)
        {
            ChunkDetails.Items.Add(new TextBlock() { Text = $"Animations Count: {anims.AnimationDict.Where(a=>a.Value.DirectionDict.Any(b=>b.Value.Frames.Count>0)).Count()}" });
        }
        

        if (loader is IDumpable dumpable)
        {
            var button = new Button() { Content = "Dump", HorizontalAlignment = HorizontalAlignment.Stretch};
            button.Click += async (o, e) =>
            {
                var filters = new List<FilePickerFileType>();
                filters.Add(new FilePickerFileType(dumpable.TypeName) { Patterns = new string[] {dumpable.FileExtension } });
                var task = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions() {Title = "Select output folder", FileTypeChoices = filters, DefaultExtension = dumpable.FileExtension, SuggestedFileName = dumpable.OutputName});
                if (task == null)
                    return;
                var outStream = await task.OpenWriteAsync();
                dumpable.DumpToMemoryStream().CopyTo(outStream);
                outStream.Close();
                outStream.Dispose();
                // Not sure if I need to manually dispose those. Slidy, please check
            };
            ChunkDetails.Items.Add(new ListBoxItem(){Content = button});
        }

    }

    private void DumpImages_Click(object? sender, RoutedEventArgs e)
    {
        var worker = new BackgroundWorker();
        worker.DoWork += (o, e) =>
        {
            var game = CurrentFile.GameData;
            var imgs = game.Images.Items;
            var directory = Path.Join("Dumps", game.Name, "Images");
            Directory.CreateDirectory(directory);
            int i = 0;
            int count = imgs.Values.Count;
            foreach (var img in imgs)
            {
                //img.Value.bitmap.Save(Path.Join(directory, $"{img.Key}.png"));
                i++;
                SetStatus($"Dumping images: {i}/{count}", (int)((i / (float)count) * 100f));
            }
        };
        worker.RunWorkerCompleted += (o, e) =>
        {
            SetStatus("Idle", 0);
        };
        worker.RunWorkerAsync();

    }

    public void SetStatus(string status, int progress)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            StatusProgress.Value = progress;
            StatusText.Text = status;
        });
    }

    private void DumpSounds_Click(object? sender, RoutedEventArgs e)
    {
        var worker = new BackgroundWorker();
        worker.DoWork += (o, e) =>
        {
            var game = CurrentFile.GameData;
            var sounds = game.Sounds;
            var directory = Path.Join("Dumps", game.Name, "Sounds");
            Directory.CreateDirectory(directory);
            int i = 0;
            int count = sounds.Count;
            foreach (var snd in sounds)
            {
                File.WriteAllBytes(Path.Join(directory, Utils.Utils.ClearName(snd.Name) + ((snd.Data[0] == 0xff || snd.Data[0] == 0x49) ? ".mp3" : ".wav")), snd.Data);
                i++;
                SetStatus($"Dumping sounds: {snd.Name}. ({i}/{count})", (int)((i / (float)count) * 100f));
            }
        };
        worker.RunWorkerCompleted += (o, e) =>
        {
            SetStatus("Idle", 0);
        };
        worker.RunWorkerAsync();
    }

    private void DumpSortedImages_Click(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void DumpMfa_Click(object? sender, RoutedEventArgs e)
    {

        var worker = new BackgroundWorker();
        worker.DoWork += (o, e) =>
        {
            try
            {
                /*SetStatus("Dumping MFA",0);
                var game = CurrentFile.GameData;
                var mfa = Pame2Mfa.Convert(game, CurrentFile.GetIcons());
                var dir = Path.Join("Dumps", game.Name ?? "Unknown game");
                Directory.CreateDirectory(dir);
                mfa.Write(new ByteWriter(Path.Join(dir,Path.GetFileNameWithoutExtension(game.EditorFilename != null && string.IsNullOrEmpty(game.EditorFilename) ? game.Name : game.EditorFilename )+".mfa"),FileMode.Create));
*/
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while dumping MFA: " + ex);
            }
        };
        worker.RunWorkerCompleted += (o, e) =>
        {
            SetStatus("Idle", 0);
        };
        worker.RunWorkerAsync();
    }
    
    private void ChunkTree_OnTapped(object? sender, TappedEventArgs e)
    {
        if((ChunkTree?.SelectedItem as Control)?.Tag is DataLoader loader)
            DisplayDataLoader(loader);
    }

    private async void PackDataFileDump_OnClick(object? sender, RoutedEventArgs e)
    {
        var packFile = (PackDataFiles.SelectedItem as Control)?.Tag as PackFile;
        var filters = new List<FilePickerFileType>();
        filters.Add(new FilePickerFileType("Packed file") { Patterns = new string[] {".*"} });
        var task = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions() {Title = "Select output folder", FileTypeChoices = filters, SuggestedFileName = packFile.PackFilename});
        if (task == null)
            return;
        var outStream = await task.OpenWriteAsync();
        outStream.Write(packFile.Data);
        outStream.Close();
        outStream.Dispose();
    }
}
