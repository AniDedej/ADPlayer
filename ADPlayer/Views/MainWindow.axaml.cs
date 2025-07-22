using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace ADPlayer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            this.TransparencyLevelHint = new[] { WindowTransparencyLevel.None };
            this.Background = Brushes.White;

            if (this.FindControl<Grid>("TitleBarGrid") is Grid titleBar)
            {
                titleBar.Background = new SolidColorBrush(Colors.White);
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
    }

    private void Header_PointerPressed(object? sender, PointerPressedEventArgs e)
    {  
        if (e.ClickCount == 2)
        {
            MaximizeRestore_Click(this, e);
        }
        else
        {
            BeginMoveDrag(e);
        }
    }

    private void Minimize_Click(object? sender, RoutedEventArgs e)
        => WindowState = WindowState.Minimized;

    private void MaximizeRestore_Click(object? sender, RoutedEventArgs e)
        => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void Close_Click(object? sender, RoutedEventArgs e)
        => Close();
}
