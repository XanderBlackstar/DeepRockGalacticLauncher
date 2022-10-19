
using System;
using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;

namespace DeepRockLauncher.Core.Models.ProcessHook;

public class ProcessHook
{
    private ProcessHookType _hookType;
    private Timer ReHookTimer = new Timer(1000);
    private int ProcessCompletionCount;
    private int ReHookAttempts;
    private string _processName;
    private Process GameProcess { get; set; }
    
    public ProcessHook()
    {
        ReHookTimer.AutoReset = true;
        ReHookTimer.Elapsed += ReHookTimerOnElapsed;
        HookType = ProcessHookType.InitialProcess;
        _processName = string.Empty;
    }

    public ProcessHookType HookType
    {
        get => _hookType;
        set => _hookType = value;
    }

    public double RehookInterval
    {
        get => ReHookTimer.Interval;
        set => ReHookTimer.Interval = value;
    }

    public event EventHandler<ProcessHookStatusEventArgs> StatusChanged;
    public event EventHandler<ProcessHookCompletedEventArgs> ProcessCompleted;

    public string ProcessName
    {
        get => _processName;
        set
        {
            _processName = value;
            TriggerStatusUpdate("Process Hook '" + value + "'");
        }
    }
    public string ExeFilename { get; set; }
    public string Arguments { get; set; }

    public void Launch()
    {
        ProcessCompletionCount = 0;
        TriggerStatusUpdate("Configuring Process ...");
        GameProcess = new Process();
        GameProcess.StartInfo.FileName = ExeFilename;
        GameProcess.StartInfo.Arguments = Arguments;
        GameProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
        GameProcess.Exited += ProcessOnExited;
        GameProcess.EnableRaisingEvents = true;
        TriggerStatusUpdate("Launching ...", MessageType.Success);
        GameProcess.Start();
        if (ProcessName == string.Empty)
        {
            ProcessName = GameProcess.ProcessName;
        }
        TriggerStatusUpdate("Process Name = " + ProcessName, MessageType.Success);
    }

    private void ProcessOnExited(object? sender, EventArgs e)
    {
        ProcessCompletionCount++;
        switch (HookType)
        {
            case ProcessHookType.InitialProcess:
                CompleteProcess();
                break;
            case ProcessHookType.ReHookOnInitialCompletion:
                if (ProcessCompletionCount == 1)
                {
                    ReHookProcess();
                }
                else
                {
                    CompleteProcess();
                }
                break;
        }
    }

    private void ReHookProcess()
    {
        // Process has shut down
        // Rehook process as it wasn't final
        
        TriggerStatusUpdate("Attempting to rehook Process", MessageType.Warning);
        GameProcess = null;
        ReHookAttempts = 0;
        ReHookTimer.Enabled = true;
    }

    private void CompleteProcess()
    {
        // Process has completed as intended
        MessageType mt = GameProcess.ExitCode == 0 ? MessageType.Success : MessageType.Error;
        TriggerStatusUpdate(string.Format("Process exited [Exit Code: {0}]", GameProcess.ExitCode), mt);
        
        ReHookTimer.Enabled = false;
        TriggerCompleteProcess(GameProcess.ExitCode, GameProcess.ExitTime);
    }

    private void ReHookTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        if (GameProcess != null)
        {
            ReHookTimer.Enabled = false;
            return;
        }

        ReHookAttempts++;
        TriggerStatusUpdate(string.Format("Rehooking Process [Attempt #{0}] ...", ReHookAttempts), MessageType.Warning);
        Process[] games = Process.GetProcessesByName(ProcessName);

        if (games.Length == 1)
        {
            TriggerStatusUpdate("Found one process", MessageType.Success);
            ReHookTimer.Enabled = false;
            GameProcess = games[0];
            GameProcess.Exited += ProcessOnExited;
            GameProcess.EnableRaisingEvents = true;
            TriggerStatusUpdate("Process hooked!", MessageType.Success);
        }
        else
        {
            if (games.Length == 0)
            {
                TriggerStatusUpdate("Process not found", MessageType.Error);
            }
            else
            {
                TriggerStatusUpdate(string.Format("Found {0} processes", games.Length));
                TriggerStatusUpdate("Can't rehook from multiple instances - Exiting", MessageType.Error);
                CompleteProcess();
            }
        }
        
    }

    private void TriggerStatusUpdate(string status, MessageType mt = MessageType.Default)
    {
        StatusChanged?.Invoke(this, new ProcessHookStatusEventArgs()
        {
            Status = status,
            MessageType = mt
        });
    }

    private void TriggerCompleteProcess(int exitCode, DateTime exitTime)
    {
        ProcessCompleted?.Invoke(this, new ProcessHookCompletedEventArgs()
        {
            ExitCode = exitCode,
            ExitTime = exitTime
        });
    }
}