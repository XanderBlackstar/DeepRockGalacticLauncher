using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace DeepRockLauncher.Core.Models.FolderSpy;

public class FolderSpy : IDisposable
{
    private FileSystemWatcher _watcher;
    private string _path;

    public event EventHandler<FolderSpyEventArgs> DirectoryChanged; 

    public FolderSpy(string filename)
    {
        string path = Path.GetDirectoryName(filename);
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException("Could not find " + Environment.NewLine + path);
        }

        Init(path);
    }

    public void Dispose()
    {
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
    }

    private void Init(string path)
    {
        _path = path;

        _watcher = new FileSystemWatcher(path);
        _watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;

        _watcher.Changed += OnChanged;
        _watcher.Created += OnCreated;
        _watcher.Deleted += OnDeleted;
        _watcher.Renamed += OnRenamed;
        _watcher.Error += OnError;

        _watcher.Filter = "*.*";
        _watcher.IncludeSubdirectories = true;
    }

    public void Start()
    {
        _watcher.EnableRaisingEvents = true;
    }

    public void Stop()
    {
        _watcher.EnableRaisingEvents = false;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
            return;

        TriggerDirectoryChanged(new FolderSpyEventArgs()
        {
            EventType = FolderSpyEventType.Changed, 
            Filename = e.FullPath
        });
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        TriggerDirectoryChanged(new FolderSpyEventArgs()
        {
            EventType = FolderSpyEventType.Created, 
            Filename = e.FullPath
        });
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        TriggerDirectoryChanged(new FolderSpyEventArgs()
        {
            EventType = FolderSpyEventType.Deleted, 
            Filename = e.FullPath
        });
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        TriggerDirectoryChanged(new FolderSpyEventArgs()
        {
            EventType = FolderSpyEventType.Renamed, 
            Filename = e.FullPath, 
            OldFilename = e.OldFullPath
        });
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        var ex = e.GetException();
        if (ex is null)
            return;

        string eventStr = "Exception: " + ex.Message;
        if (ex.InnerException != null)
            eventStr += ("(Inner: " + ex.InnerException.Message + ")");

        TriggerDirectoryChanged(new FolderSpyEventArgs()
        {
            EventType = FolderSpyEventType.Error, 
            Filename = eventStr
        });
    }

    private void TriggerDirectoryChanged(FolderSpyEventArgs eventArgs)
    {
        DirectoryChanged?.Invoke(this, eventArgs);
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    // Create the OnPropertyChanged method to raise the event
    // The calling member's name will be used as the parameter.
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}