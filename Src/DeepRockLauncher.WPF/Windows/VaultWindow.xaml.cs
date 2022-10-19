using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using DeepRockLauncher.Core.Models;
using DeepRockLauncher.WPF.Dialogs;
using DeepRockLauncher.WPF.Model;

namespace DeepRockLauncher.WPF.Windows;

public partial class VaultWindow : Window, INotifyPropertyChanged
{
    private ObservableCollection<VaultSaveGame> _saves = new ObservableCollection<VaultSaveGame>();
    private VaultSaveGame _selectedSave;
    private VaultSaveGame _activeVaultDeployment;

    public ObservableCollection<VaultSaveGame> Saves
    {
        get => _saves;
        set
        {
            _saves = value;
            OnPropertyChanged();
        }
    }

    public VaultSaveGame SelectedSave
    {
        get => _selectedSave;
        set
        {
            _selectedSave = value;
            OnPropertyChanged();
        }
    }

    public VaultSaveGame ActiveVaultDeployment
    {
        get => _activeVaultDeployment;
        set
        {
            _activeVaultDeployment = value;
            OnPropertyChanged();
        }
    }

    public VaultWindow()
    {
        InitializeComponent();

        Load();
    }

    private void Load()
    {
        LoadActiveDeploymentSave();
        
        Saves.Clear();
        string folder = App.Settings.VaultFolder;

        var files = Directory.GetFiles(folder, "*.sav").OrderByDescending(d => new FileInfo(d).LastAccessTimeUtc);        
        if (!files.Any())
            return;

        foreach (var file in files)
        {
            DateTime lastAccessTimeStamp  = File.GetLastAccessTime(file);
            var segments = Path.GetFileNameWithoutExtension(file).Split('-');
            GameService service = segments[0].Trim().ToUpperInvariant() == "XBOX" ? GameService.XBox : GameService.Steam;

            BitmapImage image;
            if (service == GameService.Steam)
            {
                image = GetResourceBitmap("/WpfUI;Component/Images/Steam.png");
            }
            else
            {
                image = GetResourceBitmap("/WpfUI;Component/Images/XBox.png");
                image = GetResourceBitmap("/WpfUI;Component/Images/XBox.png");
            }
            Saves.Add(new VaultSaveGame()
            {
                Filename = file,
                Image = image,
                Title = lastAccessTimeStamp.ToString("dddd, dd MMMM yyyy HH:mm:ss")
            });
        }
    }


    private void LoadActiveDeploymentSave()
    {
        string filename = App.Settings.LastPlayedSave;
        if (filename == "")
            return;
        
        VaultSaveGame activeSaveGame = new VaultSaveGame();
        
        activeSaveGame.Filename = filename;
        DateTime activeCreationTimeStamp = File.GetLastAccessTime(filename);
        activeSaveGame.Title = activeCreationTimeStamp.ToString("F");
        var list = Path.GetFileNameWithoutExtension(filename).Split('-');
        GameService activeService = list[0].Trim().ToUpperInvariant() == "XBOX" ? GameService.XBox : GameService.Steam;
        if (activeService == GameService.Steam)
        {
            activeSaveGame.Image = GetResourceBitmap("/WpfUI;Component/Images/Steam.png");
        }
        else
        {
            activeSaveGame.Image = GetResourceBitmap("/WpfUI;Component/Images/XBox.png");
        }

        ActiveVaultDeployment = activeSaveGame;
    }
    
    public static BitmapImage GetResourceBitmap(string resourceName)
    {
        Uri uri = new Uri(resourceName, UriKind.RelativeOrAbsolute);
        return GetResourceBitmap(uri);
    }

    public static BitmapImage GetResourceBitmap(Uri uri)
    {
        BitmapImage? img = null;
        
        try
        {
            // This line doesn't trigger an exception if the resource isn't found
            img = new BitmapImage(uri);
            
            // Accessing PixelHeight will!
            var resourceValidityCheck = img.PixelHeight;
        }
        catch
        {
            img = new BitmapImage(new Uri("/WpfUI;Component/Images/FileNotFound.png", UriKind.RelativeOrAbsolute));
        }

        return img;
    }

    private void CloseClick(object sender, RoutedEventArgs e)
    {
        this.DialogResult = true;
    }

    private async void DeploySaveClick(object sender, RoutedEventArgs e)
    {
        if (!QuickDialogs.WarningYesNo("Are you sure you want to deploy this save the next time you start the game?"))
            return;

        App.Settings.LastPlayedSave = SelectedSave.Filename;
        LoadActiveDeploymentSave();
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    // Create the OnPropertyChanged method to raise the event
    // The calling member's name will be used as the parameter.
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}