using System;

namespace DeepRockLauncher.Core.Models.ProcessHook;

public class ProcessHookStatusEventArgs : EventArgs
{
    public string Status { get; set; }
    public MessageType MessageType { get; set; } 
}