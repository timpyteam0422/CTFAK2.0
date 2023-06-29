using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
namespace CTFAK.GUI;

public partial class FileSelectorWindow : Window
{
    public FileSelectorWindow()
    {
        InitializeComponent();
        FileReaders = this.FindControl<ListBox>("FileReaders");
        FilePath = this.FindControl<TextBox>("FilePath");
        LoadFileButton = this.FindControl<Button>("LoadFileButton");
        AdvancedCheckbox = this.FindControl<CheckBox>("AdvancedCheckbox");
        AdvancedSettings = this.FindControl<Grid>("AdvancedSettings");
        RootGrid = this.FindControl<Grid>("RootGrid");
        if (!Design.IsDesignMode)
        {

        }


    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }


    private async void SelectPath_OnClick(object? sender, RoutedEventArgs e)
    {
        var filters = new List<FilePickerFileType>();
        filters.Add(new FilePickerFileType("Fusion Game") { Patterns = new string[] { "*.ccn", "*.exe", "*.dat" } });
        var task = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions() { AllowMultiple = false, Title = "Select Fusion game", FileTypeFilter = filters });

        FilePath.Text = task.Count > 0 ? task[0].Path.LocalPath : string.Empty;
    }

    private void LoadFile_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
        MainWindow.Instance.StartLoadingGame(FilePath.Text);
    }


    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        //refreshSize();

        AdvancedSettings.IsVisible = AdvancedCheckbox.IsChecked.Value;
        Console.WriteLine(AdvancedSettings.Height);
    }

    private void FilePath_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        LoadFileButton.IsEnabled = File.Exists(FilePath.Text);
    }
}