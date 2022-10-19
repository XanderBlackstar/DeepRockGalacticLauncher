using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DeepRockLauncher.Core.Models;
using DeepRockLauncher.Core.Models.FolderSpy;
using DeepRockLauncher.Core.Models.ProcessHook;
using DeepRockLauncher.Core.Models.SavesManager;
using WpfUI.ColoringListBox;

namespace DeepRockLauncher.WPF.Windows;

public partial class GameRunningWindow : Window, INotifyPropertyChanged
{
    #region Fields & Properties

    private const string ContainerString = "container";
    
    private FolderSpy FolderSpy;
    private ProcessHook ProcessHook;
    private GameService LaunchedBy;

    //private ObservableCollection<string> _eventList = new ObservableCollection<string>();
    private ObservableCollection<ColorableListBoxItem> _eventList = new ObservableCollection<ColorableListBoxItem>();
        
    private List<string> _folderSpyFiles = new List<string>();
    
    public ObservableCollection<ColorableListBoxItem> EventList
    {
        get => _eventList;
        set
        {
            _eventList = value;
            OnPropertyChanged();
        }
    }
    #endregion

    #region Properties
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

    private bool _activeButtons;
    public bool ActiveButtons
    {
        get => _activeButtons;
        set
        {
            _activeButtons = value;
            CaptionLabel.Content = "Done";
            OnPropertyChanged();
        }
    }
    #endregion
    
    #region Constructors
    public GameRunningWindow()
    {
        InitializeComponent();

        CaptionLabel.Content = "Waiting for game to close ...";
    }
    #endregion

    #region Directory Monitoring
    private void StartFolderSpy(string folder)
    {
        AddEvent(string.Format("Starting FolderSpy [{0}]", folder), ColoringState.Success);
        FolderSpy = new FolderSpy(folder);
        FolderSpy.DirectoryChanged += FolderSpyOnDirectoryChanged;
        FolderSpy.Start();
    }

    private void StopFolderSpy()
    {
        FolderSpy.Stop();
        AddEvent("FolderSpy stopped!");
    }

    private void OnDirectoryChangedUIThread(FolderSpyEventArgs e)
    {
        var eventType = e.EventType.ToString();
        string Full = e.Filename;
        string Old = e.OldFilename;
        
        _folderSpyFiles.Add(e.Filename);
            
        AddEvent("FolderSpy Event [Type: "+ eventType + "]");
        AddEvent("   Filename: " + Full);
        if (e.EventType == FolderSpyEventType.Renamed)
            AddEvent("   Old Filename: " + Old);
    }
    
    private void FolderSpyOnDirectoryChanged(object? sender, FolderSpyEventArgs e)
    {
        // Invoke event on UI thread
        App.Current?.Dispatcher.Invoke(new Action(delegate
        {
            OnDirectoryChangedUIThread(e);
        }));
    }
    #endregion
    
    #region ProcessHook
    private void StartProcessHook(string filename, ProcessHookType hookType, string processName, string arguments)
    {
        ProcessHook = new ProcessHook()
        {
            ExeFilename = filename,
            HookType = hookType,
            RehookInterval = 1000,
            ProcessName = processName,
            Arguments = arguments
        };
        ProcessHook.StatusChanged += ProcessHookStatusChanged;
        ProcessHook.ProcessCompleted += ProcessHookCompleted;
        ProcessHook.Launch();
    }

    private void ProcessHookCompleted(object? sender, ProcessHookCompletedEventArgs e)
    {
        App.Current?.Dispatcher.Invoke(new Action(delegate
        {
            ProcessHookCompletedUIThread(e);
        }));
    }

    private async void ProcessHookCompletedUIThread(ProcessHookCompletedEventArgs e)
    {
        ColoringState colorState = e.ExitCode == 0 ? ColoringState.Success : ColoringState.Error;
        AddEvent(string.Format("Game concluded [ExitCode: {0}, ExitTime: {1}]", e.ExitCode, e.ExitTime), colorState);
        await CompletePlaySession();
    }

    private void ProcessHookStatusChanged(object? sender, ProcessHookStatusEventArgs e)
    {
        App.Current?.Dispatcher.Invoke(new Action(delegate
        {
            ProcessHookStatusChangedUIThread(e);
        }));
    }

    private void ProcessHookStatusChangedUIThread(ProcessHookStatusEventArgs e)
    {
        AddEvent(e.Status, MessageTypeToColoringState(e.MessageType));
    }
    #endregion
    
    #region Steam Logic
    public async void ExecuteSteam()
    {
        LaunchedBy = GameService.Steam;
        
        Show();

        string filename = App.Settings.SteamExe; 
        string folder = App.Settings.SteamSave;

        await DeploySave(LaunchedBy);
        
        StartFolderSpy(folder);
        StartProcessHook(filename, 
                         ProcessHookType.ReHookOnInitialCompletion, 
                         "FSD-Win64-Shipping", 
                         "steam://rungameid/548430");
    }
    #endregion

    #region XBox Logic
    public async void ExecuteXBox()
    {
        LaunchedBy = GameService.XBox;

        Show();

        string filename = App.Settings.XBoxExe;
        string folder = App.Settings.XBoxSave;
  
        await DeploySave(LaunchedBy);
        
        StartFolderSpy(folder);
        StartProcessHook(filename, 
                         ProcessHookType.ReHookOnInitialCompletion, 
                         "FSD-WinGDK-Shipping", 
                         "");
    }
    #endregion
    
    public void AddEvent(string message, ColoringState colorState = ColoringState.Default)
    {
        ColorableListBoxItem item = new ColorableListBoxItem(string.Format("{0} : {1}",
                                                             DateTime.Now.ToString("HH:mm:ss.FFF"),
                                                             message), colorState);
        EventList.Add(item);

        // Scroll last item into view
        EventListBox.SelectedIndex = EventListBox.Items.Count - 1;
        EventListBox.ScrollIntoView(EventListBox.SelectedItem);
    }

    private async Task CompletePlaySession()
    {
        CaptionLabel.Content = "Finishing up ...";
        
        if (LaunchedBy == GameService.XBox)
        {
            AddEvent(string.Format("Waiting {0} seconds for XBox to finish flushing to disk", App.Settings.XboxFlushDelay), ColoringState.Warning);
            await Delay(App.Settings.XboxFlushDelay);
        }
        StopFolderSpy();

        var saveInfo = new SaveFileInformation()
        {
            GameService = LaunchedBy,
            TimeStamp = DateTime.Now
        };

        string sourceFile = LaunchedBy == GameService.Steam ? App.Settings.SteamSave : App.Settings.XBoxSave;
        if (LaunchedBy == GameService.XBox)
        {
            sourceFile = DetectNewXBoxSaveFilename();
            if (sourceFile == string.Empty)
            {
                AddEvent("Savegame was not Saved to Vault", ColoringState.Error);
                ActiveButtons = true;
                return;
            }
        }
        
        AddEvent(string.Format("Copying savefile to vault [{0}]", sourceFile), ColoringState.Success);
        DumpLog();
        SavesManager savesManager = new SavesManager(App.Settings);
        try
        {
            var savedToFilename = await savesManager.CopyToVaultAsync(sourceFile, saveInfo);
            App.Settings.LastPlayedSave = savedToFilename;
            App.Settings.Save();
            AddEvent(string.Format("Saved to: {0} ", savedToFilename), ColoringState.Success);
            AddEvent("Game saved!", ColoringState.Success);
        }
        catch (Exception e)
        {
            AddEvent(string.Format("!!! Failed !!!   [{0}]", e.Message), ColoringState.Error);
        }
        ActiveButtons = true;
        
    }

    private string DetectNewXBoxSaveFilename()
    {
        // Find new filename
        AddEvent("Extracting new save filename", ColoringState.Warning);

        string ext = ".working";
        string filename = string.Empty;
        foreach (var item in _folderSpyFiles)
        {
            if (Path.GetExtension(item).ToUpperInvariant() == ext.ToUpperInvariant())
            {
                filename = Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item));
                break;
            }
        }

        if (filename != string.Empty)
        {
            AddEvent(string.Format("New Filename: {0}", Path.GetFileName(filename)), ColoringState.Success);
            App.Settings.XBoxSave = filename;
            App.Settings.Save();
            return filename;
        }

        AddEvent("!!! Could not extract filename from FolderSpy !!!", ColoringState.Error);
        AddEvent("Trying alternate method", ColoringState.Warning);

        // Grab the folder of the previous savefile
        string saveFolder = Path.GetDirectoryName(App.Settings.XBoxSave);
        AddEvent(string.Format("Previous save folder: {0}", saveFolder));

        // Get a filelist
        var files = Directory.GetFiles(saveFolder);

        List<string> validFiles = new List<string>();


        foreach (var item in files)
        {
            AddEvent(string.Format("Scanning: {0}", Path.GetFileName(item)));

            if (item.ToUpperInvariant().Contains(ContainerString.ToUpperInvariant()))
            {
                // ignore container.* files
                AddEvent("Ignore!", ColoringState.Warning);
                continue;
            }

            AddEvent("Including in valid candidates", ColoringState.Success);
            validFiles.Add(item);
        }

        if (validFiles.Count != 1)
        {
            AddEvent("FAILED! - Too many possible candidates left on list", ColoringState.Error);
            return string.Empty;
        }

        filename = validFiles[0];
        AddEvent(string.Format("New Filename: {0}", Path.GetFileName(filename)), ColoringState.Success);
        App.Settings.XBoxSave = filename;
        App.Settings.Save();
        return filename;
    }

    
    private async Task DeploySave(GameService service)
    {
        AddEvent(string.Format("Deploying latest save to service [{0}]", service.ToString()));

        if (App.Settings.LastPlayedSave == string.Empty)
        {
            AddEvent("No previous save found in vault!", ColoringState.Warning);
            return;
        }
        
        string source = "";
        string destination = "";

        switch (service)
        {
            case GameService.Steam:
                source = App.Settings.LastPlayedSave;
                destination = App.Settings.SteamSave;
                break;
            case GameService.XBox:
                source = App.Settings.LastPlayedSave;
                destination = App.Settings.XBoxSave;
                break;
            default:
                AddEvent(string.Format("!!! Unknown Game service [{0}] !!!", service.ToString()), ColoringState.Error);
                return;
        }

        string s = Path.GetFileName(source);
        if (!File.Exists(source))
        {
            AddEvent("Save file not found", ColoringState.Error);
            return;
        }

        // Printout whether there is a savefile where we expect to copy the vault save to.
        if (!File.Exists(destination))
        {
            AddEvent("Destination savefile does not exist", ColoringState.Error);
        }
        else
        {
            AddEvent("Destination file is present - Overwriting", ColoringState.Success);
        }
        
        string d = Path.GetFileName(destination);
        AddEvent(string.Format("Copying {0} => {1}", s, d), ColoringState.Success);
        SavesManager savesManager = new SavesManager(App.Settings);
        await savesManager.CopyFileAsync(source, destination, true);
    }
    
    private async Task Delay(int delayInSeconds)
    {
        for (int i = delayInSeconds; i > -1; i--)
        {
            AddEvent(i.ToString() + " ...");
            await Task.Run(() => Thread.Sleep(1000));
        }
    }
    

    private void DumpLogClick(object sender, RoutedEventArgs e)
    {
        DumpLog();
    }

    private void DumpLog()
    {
        List<string> lines = new List<string>();
        foreach (var item in EventList)
        {
            lines.Add(item.Text);
        }

        File.WriteAllLines(Path.Combine(App.Settings.VaultFolder, "log.txt"), lines);
    }

    private void CloseWindowClick(object sender, RoutedEventArgs e)
    {
        ActiveButtons = false;
        this.Close();
    }

    private void QuickApplicationClick(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private ColoringState MessageTypeToColoringState(MessageType mt)
    {
        switch (mt)
        {
            case MessageType.Error:
                return ColoringState.Error;
            case MessageType.Success:
                return ColoringState.Success;
            case MessageType.Warning:
                return ColoringState.Warning;
        }

        return ColoringState.Default;
    }
    
    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    // Create the OnPropertyChanged method to raise the event
    // The calling member's name will be used as the parameter.
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    #endregion
    
}