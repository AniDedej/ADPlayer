using System;
using System.IO.Pipes;
using System.IO;
using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace ADPlayer.Desktop;

class Program
{
    private const string AppId = "ADPlayer_c2730a61-c9b8-4e28-b8c5-3487da95e7f3";
    private const string MutexName = AppId + "_Mutex";
    private const string PipeName = AppId + "_Pipe";

    [STAThread]
    public static void Main(string[] args)
    {
        bool isFirst;
        using var mutex = new Mutex(true, MutexName, out isFirst);

        if (isFirst)
        {
            StartPipeServer();
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        else
        {
            if (args.Length > 0 && File.Exists(args[0]))
            {
                try
                {
                    using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
                    client.Connect(timeout: 2000);
                    using var writer = new StreamWriter(client) { AutoFlush = true };
                    writer.Write(args[0]);
                }
                catch
                {
                }
            }
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
                     .UsePlatformDetect()
                     .WithInterFont()
                     .LogToTrace();

    private static void StartPipeServer()
    {
        ThreadPool.QueueUserWorkItem(_ =>
        {
            while (true)
            {
                using var server = new NamedPipeServerStream(
                    PipeName,
                    PipeDirection.In,
                    maxNumberOfServerInstances: 1,
                    PipeTransmissionMode.Message,
                    PipeOptions.None);

                try
                {
                    server.WaitForConnection();
                    using var reader = new StreamReader(server);
                    var path = reader.ReadToEnd().Trim();

                    if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                    {
                        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                        {
                            if (Application.Current.ApplicationLifetime
                                is IClassicDesktopStyleApplicationLifetime desktop &&
                                desktop.MainWindow?.DataContext
                                is ViewModels.MainViewModel vm)
                            {
                                desktop.MainWindow.Show();
                                desktop.MainWindow.Activate();
                                vm.LoadFile(path);
                            }
                        });
                    }
                }
                catch (IOException)
                {
                }
            }
        });
    }
}