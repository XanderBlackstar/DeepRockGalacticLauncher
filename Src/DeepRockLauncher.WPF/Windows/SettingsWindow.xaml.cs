using System;
using System.ComponentModel;
using System.IO;
using System.IO.Enumeration;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using DeepRockLauncher.Core.Models.Settings;
using Microsoft.Win32;

namespace DeepRockLauncher.WPF.Windows;

public partial class SettingsWindow : Window, INotifyPropertyChanged
{
    SolidColorBrush failBrush = new SolidColorBrush(Colors.Salmon);
    SolidColorBrush correctBrush = new SolidColorBrush(Colors.Aquamarine);
    
    
    public SettingsWindow()
    {
        InitializeComponent();
        
        LoadSettingsToWindow();
    }

    private void CloseClick(object sender, RoutedEventArgs e)
    {
        bool canContinue = true;

        if (!VerifySteamExe())
            canContinue = false;
        if (!VerifySteamSave())
            canContinue = false;
        if (!VerifyXboxExe())
            canContinue = false;
        if (!VerifyXboxSave())
            canContinue = false;
        if (!VerifyDiskFlushDelay())
            canContinue = false;

        if (!canContinue)
            return;
        
        SaveWindowToSettings();
        this.DialogResult = true;
    }

    private bool VerifySteamExe()
    {
        if (!File.Exists(TextBoxSteamExe.Text))
        {
            TextBoxSteamExe.Background = failBrush;
            return false;
        }
        TextBoxSteamExe.Background = correctBrush;
        return true;
    }
    private bool VerifySteamSave()
    {
        if (!File.Exists(TextBoxSteamSave.Text))
        {
            TextBoxSteamSave.Background = failBrush;
            return false;
        }
        TextBoxSteamSave.Background = correctBrush;
        return true;
    }
    private bool VerifyXboxExe()
    {
        if (!File.Exists(TextBoxXBoxExe.Text))
        {
            TextBoxXBoxExe.Background = failBrush;
            return false;
        }
        TextBoxXBoxExe.Background = correctBrush;
        return true;
    }
    private bool VerifyXboxSave()
    {
        if (!File.Exists(TextBoxXBoxSave.Text))
        {
            TextBoxXBoxSave.Background = failBrush;
            return false;
        }
        TextBoxXBoxSave.Background = correctBrush;
        return true;
    }

    private bool VerifyDiskFlushDelay()
    {
        int delay = 0;
        if (!Int32.TryParse(TextBoxDiskFlushDelay.Text, out delay))
        {
            TextBoxDiskFlushDelay.Background = failBrush;
            return false;
        }

        TextBoxDiskFlushDelay.Background = correctBrush;
        return true;
    }
    
    

    private void SaveWindowToSettings()
    {
        App.Settings.SteamExe = TextBoxSteamExe.Text;
        App.Settings.SteamSave = TextBoxSteamSave.Text;
        App.Settings.XBoxExe = TextBoxXBoxExe.Text;
        App.Settings.XBoxSave = TextBoxXBoxSave.Text;
        int delay = 30;
        Int32.TryParse(TextBoxDiskFlushDelay.Text, out delay);
        App.Settings.XboxFlushDelay = delay;
        App.Settings.Save();
    }

    private void LoadSettingsToWindow()
    {
        TextBoxSteamExe.Text = App.Settings.SteamExe;
        TextBoxSteamSave.Text = App.Settings.SteamSave;
        TextBoxXBoxExe.Text = App.Settings.XBoxExe;
        TextBoxXBoxSave.Text = App.Settings.XBoxSave;
        TextBoxDiskFlushDelay.Text = App.Settings.XboxFlushDelay.ToString();
    }

    private string PerformOpenFileDialog(string defaultFile, string defaultFolder, string filter = "")
    {
        if (filter == "")
            filter = "Executable files (*.exe)|*.exe";
        
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.FileName = defaultFile;
        openFileDialog.Filter = filter;
        openFileDialog.InitialDirectory = defaultFolder;
        if (openFileDialog.ShowDialog() == true)
        {
            return openFileDialog.FileName;
        }
        else
        {
            return string.Empty;
        }
    }

    private void BrowseSteamExeClick(object sender, RoutedEventArgs e)
    {
        string filename = PerformOpenFileDialog(TextBoxSteamExe.Text,
            Path.GetDirectoryName(TextBoxSteamExe.Text));

        if (filename != string.Empty)
        {
            TextBoxSteamExe.Text = filename;
        }
    }

    private void BrowseSteamSaveClick(object sender, RoutedEventArgs e)
    {
        string filename = PerformOpenFileDialog(TextBoxSteamSave.Text,
            Path.GetDirectoryName(TextBoxSteamSave.Text),
            "Steam Quicksave (*_Player.sav)|*_Player.sav");

        if (filename != string.Empty)
        {
            TextBoxSteamSave.Text = filename;
        }
    }

    private void BrowseXBoxExeClick(object sender, RoutedEventArgs e)
    {
        string filename = PerformOpenFileDialog(TextBoxXBoxExe.Text,
            Path.GetDirectoryName(TextBoxXBoxExe.Text));

        if (filename != string.Empty)
        {
            TextBoxXBoxExe.Text = filename;
        }
    }

    private void BrowseXBoxSaveClick(object sender, RoutedEventArgs e)
    {
        string filename = PerformOpenFileDialog(TextBoxXBoxSave.Text,
            Path.GetDirectoryName(TextBoxXBoxExe.Text),
            "All files (*.*)|*.*");
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    // Create the OnPropertyChanged method to raise the event
    // The calling member's name will be used as the parameter.
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void AutoLocateClick(object sender, RoutedEventArgs e)
    {
        var dialog = new FindPreReqsWindow();
        if (dialog.ShowDialog() == false)
            return;

        var settings = new Settings(App.LaunchFolder); // Constructor provides default values
        settings.SteamExe = dialog.SteamExe;
        settings.SteamSave = dialog.SteamSave;
        settings.XBoxExe = dialog.XboxExe;
        settings.XBoxSave = dialog.XboxSave;
        settings.VaultFolder = App.Settings.VaultFolder;
        int delay = 30;
        Int32.TryParse(TextBoxDiskFlushDelay.Text, out delay);
        settings.XboxFlushDelay = delay;
        App.Settings = settings;
        App.Settings.Save();
        LoadSettingsToWindow();
    }
}