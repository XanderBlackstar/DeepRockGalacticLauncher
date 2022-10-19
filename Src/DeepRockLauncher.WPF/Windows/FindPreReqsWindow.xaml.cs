using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Diagnostics;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using DeepRockLauncher.WPF.Dialogs;
using DeepRockLauncher.WPF.Model;
using Microsoft.Win32;

namespace DeepRockLauncher.WPF.Windows;

public partial class FindPreReqsWindow : Window, INotifyPropertyChanged
{
    #region Fields & Constants
    private bool _fastSearchRan = false;

    public string SteamExe = string.Empty;
    public string SteamSave = string.Empty;
    public string XboxExe = string.Empty;
    public string XboxSave = string.Empty;

    private const string SteamExeDefault = @"C:\Program Files (x86)\Steam\steam.exe";

    private const string SteamSaveDefault =
        @"C:\Program Files (x86)\Steam\steamapps\common\Deep Rock Galactic\FSD\Saved\SaveGames";

    private const string XboxExeDefault =
        @"C:\XboxGames\Deep Rock Galactic\Content\FSD\Binaries\WinGDK\FSD-WinGDK-Shipping.exe";

    private const string XboxSaveDefault =
        @"<AppDataLocal>\Packages\CoffeeStainStudios.DeepRockGalactic_496a1srhmar9w\SystemAppData\wgs";
    #endregion

    #region Properties
    private BoundColorString _steamExeStatus;
    public BoundColorString SteamExeStatus
    {
        get => _steamExeStatus;
        set
        {
            _steamExeStatus = value;
            OnPropertyChanged();
        }
    }

    private BoundColorString _steamSaveStatus;
    public BoundColorString SteamSaveStatus
    {
        get => _steamSaveStatus;
        set
        {
            _steamSaveStatus = value;
            OnPropertyChanged();
        }
    }

    private BoundColorString _xboxExeStatus;
    public BoundColorString XboxExeStatus
    {
        get => _xboxExeStatus;
        set
        {
            _xboxExeStatus = value;
            OnPropertyChanged();
        }
    }

    private BoundColorString _xboxSaveStatus;
    public BoundColorString XboxSaveStatus
    {
        get => _xboxSaveStatus;
        set
        {
            _xboxSaveStatus = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<string> _eventList = new();
    public ObservableCollection<string> EventList
    {
        get => _eventList;
        set
        {
            _eventList = value;
            OnPropertyChanged();
        }
    }

    private string _selectedItem;
    public string SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            OnPropertyChanged();
        }
    }
    
    public bool FoundAllNeeded
    {
        get
        {
            return (SteamExe != string.Empty &&
                    SteamSave != string.Empty &&
                    XboxExe != string.Empty &&
                    XboxSave != string.Empty);
        }
    }
    #endregion

    public FindPreReqsWindow()
    {
        InitializeComponent();

        string s = "Hold On; I'm looking ...";
        SteamExeStatus = new BoundColorString(s);
        SteamExeStatus.IDontHaveAnOpinion();

        SteamSaveStatus = new BoundColorString(s);
        SteamSaveStatus.IDontHaveAnOpinion();

        XboxExeStatus = new BoundColorString(s);
        XboxExeStatus.IDontHaveAnOpinion();

        XboxSaveStatus = new BoundColorString(s);
        XboxSaveStatus.IDontHaveAnOpinion();
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        if (_fastSearchRan)
            return;

        FastSearch();
    }



    private void FastSearch()
    {
        if (_fastSearchRan)
            return;

        _fastSearchRan = true;
        AddEvent("Checking where I expect to find what I'm looking for [Default locations]");

        string[] files;
        string[] directories;

        // Find steam exe
        if (File.Exists(SteamExeDefault))
        {
            SteamExe = SteamExeDefault;
            SteamExeStatus.Text = "All hail be to the Gabe";
            SteamExeStatus.WeGood();
            AddEvent("Found Steam Executable");
        }
        else
        {
            AddEvent("! Failed to find Steam Executable !");
        }

        // Find steam save
        if (Directory.Exists(SteamSaveDefault))
        {
            files = Directory.GetFiles(SteamSaveDefault, "*_player.sav");
            if (files.Length == 1)
            {
                SteamSave = files[0];
                SteamSaveStatus.Text = "Oh look what I found";
                SteamSaveStatus.WeGood();
                AddEvent("Found Steam Save");
            }
        }
        else
        {
            AddEvent("! Failed to find Steam Save file !");
        }

        // Find XBox DRG Exe
        if (File.Exists(XboxExeDefault))
        {
            XboxExe = XboxExeDefault;
            XboxExeStatus.Text = "Praise be to the William";
            XboxExeStatus.WeGood();
            AddEvent("Found XBox DRG executable");
        }
        else
        {
            AddEvent("! Failed to XBox DRG Executable !");
        }

        // Find Xbox save
        string searchDir = ReplaceFolderSpecifiers(XboxSaveDefault);
        if (Directory.Exists(searchDir))
        {
            string dontAccept = "T";
            dontAccept = dontAccept.ToUpperInvariant();
            var dirs = Directory.GetDirectories(searchDir, "*").ToList();
            if (dirs.Count == 2)
            {
                List<string> filesWithNoExt = new List<string>();
                foreach (var item in dirs)
                {
                    if (Path.GetFileName(item).ToUpperInvariant() == "T")
                        continue;

                    AddEvent("Searching " + item);
                    var secondDirs = Directory.GetDirectories(item, "*.*");

                    foreach (var secondLevelItem in secondDirs)
                    {
                        files = Directory.GetFiles(secondLevelItem, "*.*");
                        foreach (var file in files)
                        {
                            if (Path.GetExtension(file) == "")
                            {
                                XboxSave = file;
                                XboxSaveStatus.Text = "There you are you little f...";
                                XboxSaveStatus.WeGood();
                                AddEvent("Found Xbox Save file: " + file);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            AddEvent("! Failed to find XBox save !");
        }

        // Check if we found all the files we are looking for
        if (FoundAllNeeded)
        {
            AddEvent("Just what I expected");
            return;
        }

        AddEvent("Well that was not what I was expecting");
    }

    private void GuidedSearch()
    {
        if (SteamExe == string.Empty)
        {
            // Find steam exe
            QuickDialogs.Info("For the next Dialog I need you to locate the following" + Environment.NewLine + Environment.NewLine +
                              "THE STEAM CLIENT EXECUTABLE" + Environment.NewLine + Environment.NewLine +
                              "Hint: Open the start menu and type 'steam'" + Environment.NewLine +
                              "Right click on the item and select 'Open File Location'" + Environment.NewLine + 
                              "Copy the content of the address bar of the window that opened." + Environment.NewLine + Environment.NewLine +
                              "I'm kinda stupid, so I will still show you a dialog, but hopefully there will only be a 'Steam' file for you to select" + Environment.NewLine +
                              "Select it!");
            ManuallyLocateSteamExe();
        }

        if (SteamSave == string.Empty)
        {
            // Find Steam save
            QuickDialogs.Info("For the next Dialog I need you to locate" + Environment.NewLine + Environment.NewLine +
                              "THE STEAM SAVE FILE" + Environment.NewLine + Environment.NewLine +
                              "Hint: This is located in a subdirectory of where you installed DRG when you installed the game through steam." + Environment.NewLine +
                              "If you know where your steam install is, navigate to the following sub directory." + Environment.NewLine + Environment.NewLine +
                              @".\steamapps\common\Deep Rock Galactic\FSD\Saved\SaveGames" + Environment.NewLine + Environment.NewLine +
                              "If correct you should only be shown one file in the dialog. Select it!");
            ManuallyLocateSteamSave();
        }

        if (XboxExe == string.Empty)
        {
            // Find Xbox DRG executable
            QuickDialogs.Info("For the next Dialog I need you to locate" + Environment.NewLine + Environment.NewLine +
                              "THE MAIN EXECUTABLE FOR THE XBOX INSTALL OF DRG" + Environment.NewLine +
                              Environment.NewLine +
                              "Hint: This is the DRG program file in the Xbox install location." + Environment.NewLine +
                              "If you know what drive you installed the game to. You are looking for the following sub directory." +
                              Environment.NewLine +
                              @"\Deep Rock Galactic\Content\FSD\Binaries\WinGDK" + Environment.NewLine +
                              "If correct you will only be presented with one file. Select it!");
            ManuallyLocateXboxExe();
        }

        if (XboxSave == string.Empty)
        {
            // Find Xbox save game
            QuickDialogs.Info("For the next Dialog I need you to locate" + Environment.NewLine + Environment.NewLine +
                              "THE XBOX SAVE FILE" + Environment.NewLine + Environment.NewLine +
                              "Hint: I love Da Bill, but he does not make it easy. This save file was the hardest to track when coding this app." +
                              Environment.NewLine + Environment.NewLine +
                              "I'm sorry I cannot help you locate this file if it is not in the default location" +
                              Environment.NewLine + Environment.NewLine +
                              "My best advice is go online seeking help as to where to find the XBox save file for DRG." +
                              Environment.NewLine + Environment.NewLine +
                              "I'm sorry! - But I still might be able to locate it during the deep search.");
        }

        // Check if we found all the files we are looking for
        if (FoundAllNeeded)
        {
            AddEvent("Just what I expected");
            return;
        }

        DeepDriveSearch();
    }

    private void ManuallyLocateXboxExe()
    {
        AddEvent("Manually Locate Xbox DRG executable");
        var dialog = new OpenFileDialog();
        dialog.Filter = "Active Steam save (*.exe)|*.exe";
        if (dialog.ShowDialog() == false)
        {
            AddEvent("Steam Save not found");
            SteamSaveStatus.Text = "Pending deep search";
            SteamSaveStatus.HoldOn();
            return;
        }

        if (File.Exists(dialog.FileName))
        {
            SteamSave = dialog.FileName;
            SteamSaveStatus.Text = "Hallelujah";
            SteamSaveStatus.WeGood();
        }
    }

    private void ManuallyLocateSteamSave()
    {
        AddEvent("Manually Locate Steam save");
        var dialog = new OpenFileDialog();
        dialog.Filter = "Active Steam save (*_player.sav)|*_player.sav";
        if (dialog.ShowDialog() == false)
        {
            AddEvent("Steam Save not found");
            SteamSaveStatus.Text = "Pending deep search";
            SteamSaveStatus.HoldOn();
            return;
        }

        if (File.Exists(dialog.FileName))
        {
            SteamSave = dialog.FileName;
            SteamSaveStatus.Text = "Hallelujah";
            SteamSaveStatus.WeGood();
        }
    }

    private void ManuallyLocateSteamExe()
    {
        AddEvent("Manually locate steam");
        var dialog = new OpenFileDialog();
        dialog.Filter = "Steam executable (steam.exe)|steam.exe";
        if (dialog.ShowDialog() == false)
        {
            AddEvent("Somebody doesn't know Gabe");
            SteamExeStatus.Text = "Pending deep search";
            SteamExeStatus.HoldOn();
            return;
        }

        if (File.Exists(dialog.FileName))
        {
            SteamExe = dialog.FileName;
            SteamExeStatus.Text = "I've found Gabe!";
            SteamExeStatus.WeGood();
        }
    }

    private void DeepDriveSearch()
    {
        QuickDialogs.Info("Won't somebody think of the energy prices\n" +
                          "\n" +
                          "With apologies to Jesse Jackson, this feature is not implemented");

        // Approach

        // SteamExeDefault
        // Alert for "Steam.exe" and then verify the folder with another file (somewhat obscure file) that can only be found in that folder (I haven't looked for said file)

        // SteamSaveDefault
        // Alert for "*_player.sav" file. Maybe look into the file header for a DRG save file and verify that it is actually a DRG save file.

        // XBoxExeDefault
        // Alert for "FSD-WinGDK-Shipping.exe" file.

        // XBoxSaveDefault
        // Alert for the folder "CoffeeStainStudios.DeepRockGalactic_496a1srhmar9w", use the approach used in quick search and maybe combine with Save file validation as mentioned in SteamSaveDefault approach 
    }


    public void AddEvent(string message)
    {
        EventList.Add(string.Format("{0} : {1}",
            DateTime.Now.ToString("HH:mm:ss.FFF"),
            message));

        // Scroll last item into view
        EventListBox.SelectedIndex = EventListBox.Items.Count - 1;
        EventListBox.ScrollIntoView(EventListBox.SelectedItem);
    }

    private string ReplaceFolderSpecifiers(string filename)
    {
        // <AppDataLocal>
        if (filename.Contains("<AppDataLocal>"))
        {
            string appDataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            AddEvent(string.Format("AppDataLocal = {0}", appDataLocal));
            return filename.Replace("<AppDataLocal>", appDataLocal);
        }

        return filename;
    }


    #region UI Buttons
    private void DebugClick(object sender, RoutedEventArgs e)
    {
        FastSearch();
    }

    private void FullDriveScanClick(object sender, RoutedEventArgs e)
    {
        DeepDriveSearch();
    }

    private void CloseClick(object sender, RoutedEventArgs e)
    {
        this.DialogResult = true;
    }

    private void GuidedSearchClick(object sender, RoutedEventArgs e)
    {
        GuidedSearch();
    }
    #endregion
    
    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        if (name is null)
            return;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    #endregion
    
}