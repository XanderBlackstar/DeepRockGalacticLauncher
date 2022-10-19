using System.Windows;

namespace DeepRockLauncher.WPF.Dialogs;

public static class QuickDialogs
{
    public static void Info(string message)
    {
        MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public static void Warning(string message)
    {
        MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    public static void Error(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public static bool WarningYesNo(string message)
    {
        return MessageBox.Show(message, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
               MessageBoxResult.Yes;
    }
}