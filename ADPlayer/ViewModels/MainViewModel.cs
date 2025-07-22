using ADPlayer.Services;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManagedBass;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ADPlayer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IAudioService _audio;
    private readonly DispatcherTimer _timer;
    private bool _isDragging;
    private string? _lastPath;

    [ObservableProperty] private string _fileName = "No file loaded";
    [ObservableProperty] private double _positionSeconds;
    [ObservableProperty] private double _durationSeconds = 0.001f;
    [ObservableProperty] private bool _isPlaying;
    [ObservableProperty] private float _volume = 1.0f;
    public ObservableCollection<string> OutputDevices { get; } = new();
    [ObservableProperty] private int _selectedDeviceIndex;
    [ObservableProperty] private Bitmap? _albumArt;
    [ObservableProperty] private bool _isVolumePopupOpen;
    [ObservableProperty] private bool _isSpeakerPopupOpen;

    public MainViewModel(IAudioService audio)
    {
        _audio = audio;
        if (!_audio.Initialize())
            throw new InvalidOperationException("BASS init failed");
        PopulateDevices();
        _audio.SetVolume(Volume);

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        _timer.Tick += (_, __) => {
            if (!_isDragging && _lastPath != null)
            {
                PositionSeconds = _audio.GetPosition();
                if (PositionSeconds >= DurationSeconds - 0.001f)
                {
                    Stop();
                }
            }
        };
    }

    partial void OnSelectedDeviceIndexChanged(int oldValue, int newValue)
    {
        if (newValue < 0 || newValue >= Bass.DeviceCount)
            return;
        IsSpeakerPopupOpen = false;
        _audio.Dispose();

        if (!Bass.Init(newValue, 44100, DeviceInitFlags.Default, IntPtr.Zero))
        {
            Bass.Init(-1, 44100, DeviceInitFlags.Default, IntPtr.Zero);
            SelectedDeviceIndex = Array.IndexOf(OutputDevices.ToArray(), "Default");
            return;
        }

        if (!string.IsNullOrEmpty(_lastPath))
        {
            _audio.Load(_lastPath);
            if (IsPlaying) _audio.Play();
        }
    }

    partial void OnVolumeChanged(float _, float v) => _audio.SetVolume(v);

    private void PopulateDevices()
    {
        for (int i = 0; i < Bass.DeviceCount; i++)
        {
            var info = Bass.GetDeviceInfo(i);
            OutputDevices.Add(info.Name + (info.IsDefault ? " (Default)" : ""));
            if (info.IsDefault) SelectedDeviceIndex = i;
        }
    }

    private void UpdatePosition(object? sender, EventArgs e)
    {
        if (!_isDragging)
            PositionSeconds = _audio.GetPosition();
    }

    [RelayCommand]
    private void TogglePlayPause()
    {
        if (_lastPath == null) return;
        if (IsPlaying) { _audio.Pause(); _timer.Stop(); }
        else { _audio.Play(); _timer.Start(); }
        IsPlaying = !IsPlaying;
    }

    [RelayCommand]
    private void Stop()
    {
        _audio.Stop();
        _timer.Stop();
        IsPlaying = false;
        Seek(0);
    }

    [RelayCommand]
    private void Seek(double sec)
    {
        _audio.Seek(sec);
        PositionSeconds = sec;
    }

    public void StartDragging() => _isDragging = true;
    public void EndDragging() => _isDragging = false;

    public void LoadFile(string path)
    {
        _lastPath = path;
        Stop();
        _audio.Dispose();
        _audio.Initialize();
        _audio.Load(path);
        FileName = Path.GetFileName(path);
        DurationSeconds = _audio.GetDuration();
        PositionSeconds = 0;
        LoadAlbumArt(path);
        _audio.Play();
        _timer.Start();
        IsPlaying = true;
    }

    private void LoadAlbumArt(string f)
    {
        try
        {
            var file = TagLib.File.Create(f);
            var pic = file.Tag.Pictures.FirstOrDefault();
            if (pic != null)
            {
                using var ms = new MemoryStream(pic.Data.Data);
                AlbumArt = Bitmap.DecodeToWidth(ms, 100);
            }
            else AlbumArt = null;
        }
        catch { AlbumArt = null; }
    }
}
