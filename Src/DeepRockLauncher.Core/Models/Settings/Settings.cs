
using System;
using System.Collections.Generic;
using System.IO;

namespace DeepRockLauncher.Core.Models.Settings;

public class Settings
{
    private string _settingsfolder = string.Empty;
    private string _settingsFile = string.Empty;
    
    public string SteamExe { get; set; }
    public string SteamSave { get; set; }
    public string XBoxExe { get; set; }
    public string XBoxSave { get; set; }
    public string VaultFolder { get; set; }
    public int XboxFlushDelay { get; set; }
    public string LastPlayedSave { get; set; }
    public bool IsDefaultValues { get; set; }

    public Settings(string settingsFolder)
    {
        CreateDefault();
        _settingsfolder = settingsFolder;
        _settingsFile = Path.Combine(_settingsfolder, "Vault\\Settings.dat");
        VaultFolder = Path.Combine(_settingsfolder, "Vault");

        /*
         Default File locations
        SteamExe    = C:\Program Files (x86)\Steam\steam.exe
        SteamSave   = C:\Program Files (x86)\Steam\steamapps\common\Deep Rock Galactic\FSD\Saved\SaveGames\76561198055151546_Player.sav
        XBoxExe     = C:\XboxGames\Deep Rock Galactic\Content\FSD\Binaries\WinGDK\FSD-WinGDK-Shipping.exe
        XBoxSave    = C:\Users\<profilename>\AppData\Local\Packages\CoffeeStainStudios.DeepRockGalactic_496a1srhmar9w\SystemAppData\wgs\0009000002F05CE3_882901006F2042808DB0569531F199CB\5B5CE989CF97455C8C9CCBCC879DB45D\546A40FB4CCB431981CB64FD452601F8
        VaultFolder = <InstallDir>\Vault
        */
    }

    public void Save()
    {
        List<string> lines = new List<string>();
        
        // Format
        // SteamExe
        lines.Add(SteamExe);
        // SteamSave
        lines.Add(SteamSave); 
        // XBoxExe
        lines.Add(XBoxExe);
        // XBoxSave
        lines.Add(XBoxSave);
        // VaultFolder
        lines.Add(VaultFolder);
        // XboxFlushDelay
        lines.Add(XboxFlushDelay.ToString());
        // LastPlayedSave
        lines.Add(LastPlayedSave);
        
        File.WriteAllLines(_settingsFile, lines);
    }

    public bool Load()
    {
        if (!File.Exists(_settingsFile))
        {
            // Create default Vault folder
            string vaultFolder = Path.Combine(_settingsfolder, "Vault"); 
            if (!Directory.Exists(vaultFolder))
            {
                Directory.CreateDirectory(vaultFolder);
            }
            VaultFolder = vaultFolder;
            // Return false because we didn't actually load the save file
            return false;
        }
        
        return LoadSettingsFile(_settingsFile);
    }

    private bool LoadSettingsFile(string filename)
    {
        var lines = File.ReadAllLines(filename);
        if (lines.Length < 7)
            return false;

        // Format
        // SteamExe
        SteamExe = lines[0];
        // SteamSave
        SteamSave = lines[1];
        // XBoxExe
        XBoxExe = lines[2];
        // XBoxSave
        XBoxSave = lines[3];
        // VaultFolder
        VaultFolder = lines[4];
        // XboxFlushDelay
        int delay = 30;
        Int32.TryParse(lines[5], out delay);
        XboxFlushDelay = delay;
        // LastPlayedSave
        LastPlayedSave = lines[6];
        IsDefaultValues = false;
        return true;
    }

    private void CreateDefault()
    {
        SteamExe = @"C:\Program Files (x86)\Steam\steam.exe";
        SteamSave = @"C:\Program Files (x86)\Steam\steamapps\common\Deep Rock Galactic\FSD\Saved\SaveGames";
        XBoxExe = string.Empty;
        XBoxSave = string.Empty;
        XboxFlushDelay = 45;
        LastPlayedSave = string.Empty;
        IsDefaultValues = true;
    }
}