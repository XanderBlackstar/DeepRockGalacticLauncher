using System.Windows;

namespace DeepRockLauncher.WPF.Windows;

public partial class ChooseGameWindow : Window
{
    public ChooseGameWindow()
    {
        InitializeComponent();
    }

    private void OpenSettingsClick(object sender, RoutedEventArgs e)
    {
        var dialog = new SettingsWindow();
        dialog.ShowDialog();
    }

    private void XBoxClick(object sender, RoutedEventArgs e)
    {
        var dialog = new GameRunningWindow();
        dialog.ExecuteXBox();
    }

    private void SteamClick(object sender, RoutedEventArgs e)
    {
        var dialog = new GameRunningWindow();
        //dialog.ShowDialog();
        dialog.ExecuteSteam();
    }

    private void OpenVaultClick(object sender, RoutedEventArgs e)
    {
        var dialog = new VaultWindow();
        dialog.ShowDialog();
    }
}