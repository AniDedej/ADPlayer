using System.IO;
using ADPlayer.Services;
using ADPlayer.ViewModels;
using ADPlayer.Views;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace ADPlayer;

public partial class App : Application
{
    private ServiceProvider? _services;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vm = _services.GetRequiredService<MainViewModel>();
            var mainWindow = new MainWindow { DataContext = vm };
            desktop.MainWindow = mainWindow;

            if (desktop.Args.Length > 0 && File.Exists(desktop.Args[0]))
                vm.LoadFile(desktop.Args[0]);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<IAudioService, AudioService>();
        services.AddSingleton<MainViewModel>();
    }
}
