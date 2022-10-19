using System;

namespace DeepRockLauncher.Core.Models.FolderSpy;

public class FolderSpyEventArgs : EventArgs
{
    public FolderSpyEventType EventType { get; set; }
    public string Filename { get; set; }
    public string OldFilename { get; set; }

    public FolderSpyEventArgs()
    {
        EventType = FolderSpyEventType.Unknown;
        Filename = "";
        OldFilename = "";
    }
}