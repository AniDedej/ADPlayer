using ADPlayer.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace ADPlayer.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        BrowseButton.Click += OnBrowseClicked;
        PositionSlider.AddHandler(PointerPressedEvent, OnSliderPressed, RoutingStrategies.Tunnel);
        PositionSlider.AddHandler(PointerReleasedEvent, OnSliderReleased, RoutingStrategies.Tunnel);
    }

    private async void OnBrowseClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var options = new FilePickerOpenOptions
        {
            Title = "Select Audio File",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                        new FilePickerFileType("Audio Files")
                        {
                            Patterns = new[] {
                                "*.mp3",
                                "*.wav",
                                "*.flac",
                                "*.ogg",
                                "*.m4a",
                                "*.mp2",
                                "*.mp1",
                                "*.aac",
                                "*.m4b",
                                "*.m4r",
                                "*.ape",
                                "*.mpc",
                                "*.mpp",
                                "*.mp+",
                                "*.ofr",
                                "*.ofs",
                                "*.opus",
                                "*.spx",
                                "*.tta",
                                "*.wma",
                                "*.aif",
                                "*.aiff",
                                "*.aifc",
                                "*.wv",
                                "*.midi",
                                "*.mid",
                                "*.rmi",
                                "*.kar",
                                "*.mod",
                                "*.mdz",
                                "*.mo3",
                                "*.s3m",
                                "*.s3z",
                                "*.xm",
                                "*.xmz",
                                "*.it",
                                "*.itz",
                                "*.mtm",
                                "*.umx"
                            },
                            AppleUniformTypeIdentifiers = new[] { "public.audio" },
                            MimeTypes = new[] { "audio/*" },
                        }
                    }
        };
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(options);

        if (files.Count > 0 && files[0].TryGetLocalPath() is string path)
        {
            vm.LoadFile(path);
        }
    }
    private void OnSliderPressed(object? s, PointerPressedEventArgs e)
    {
        var slider = s as Slider;
        if (slider.Maximum <= 0.001f) { e.Handled = true; return; }
        (DataContext as MainViewModel)?.StartDragging();
    }
    private void OnSliderReleased(object? s, PointerReleasedEventArgs e)
    {
        var vm = DataContext as MainViewModel;
        vm?.EndDragging();
        vm?.SeekCommand.Execute(PositionSlider.Value);
    }
}
