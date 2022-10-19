using System;

namespace DeepRockLauncher.Core.Models.ProcessHook;

public class ProcessHookCompletedEventArgs : EventArgs
{
    public int ExitCode { get; set; }
    public DateTime ExitTime { get; set; }
}