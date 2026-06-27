using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;

namespace RetroAchievementBingoGenerator.Helpers;

public class Clipboard
{
    public static IClipboard Get()
    {
        //Desktop
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window })
        {
            return window.Clipboard!;

        }

        return null!;
    }
}
