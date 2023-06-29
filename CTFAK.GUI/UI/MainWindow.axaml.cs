﻿using Avalonia.Controls;
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
using CTFAK.IO.Common.Banks.SoundBank;

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
            var plugin = (PluginList.SelectedItem as Control).Tag as IPlugin;
            PluginPanel.Children.Clear();
            PluginPanel.Children.Add(plugin as UserControl);
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


    public TreeViewItem CreateTreeEntryForDataLoader(DataLoader loader)
    {
        var treeViewItem = new TreeViewItem();
        treeViewItem.Header = "Unknown";
        treeViewItem.Tag = loader;
        if (loader is Chunk chk)
        {
            treeViewItem.Header = ChunkList.GetChunkName(chk.Id);
            if (chk is Frame frmLoader)
            {
                treeViewItem.Header = $"Frame \"{frmLoader.Name}\"";
            }
            
            if (chk is ListChunk list)
            {
                foreach (var item in list)
                {
                    treeViewItem.Items.Add(CreateTreeEntryForDataLoader(item));
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

        treeViewItem.PointerPressed += (o, e) =>
        {
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
            {
                var contextMenu = new ContextMenu();
                var saveRaw = new TextBlock();

                saveRaw.Text = "Save(Raw)";
                saveRaw.PointerPressed += (o, e) =>
                {
                    contextMenu.Close();

                };
                contextMenu.Items.Add(saveRaw);

                var saveUncompressed = new TextBlock();
                saveUncompressed.Text = "Save(Uncompressed)";
                saveUncompressed.PointerPressed += (o, e) =>
                {
                    contextMenu.Close();

                };
                contextMenu.Items.Add(saveUncompressed);
                contextMenu.Placement = PlacementMode.Pointer;
                contextMenu.PlacementTarget = (e.Source as Control);
                contextMenu.Open(this);
            }
        };

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
        DisplayDataLoader((ChunkTree.SelectedItem as Control).Tag as DataLoader);
    }
}
