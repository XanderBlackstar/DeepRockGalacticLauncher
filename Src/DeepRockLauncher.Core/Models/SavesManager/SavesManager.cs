using System;
using System.IO;
using System.Threading.Tasks;

namespace DeepRockLauncher.Core.Models.SavesManager;

public class SavesManager
{
    private Settings.Settings _settings;
    
    public SavesManager(Settings.Settings settings)
    {
        if (settings is null)
            throw new NullReferenceException("settings cannot be null");
        _settings = settings;
    }

    public async Task<string> CopyToVaultAsync(string source, SaveFileInformation info)
    {
        string vaultFilename = EnsureTrailingSlash(_settings.VaultFolder) + info.GetFilename();
        await CopyFileAsync(source, vaultFilename, true);
        return vaultFilename;
    }
    
    
    public async Task CopyFileAsync(string source, string destination, bool overwrite = false)
    {
        await Task.Run(() => File.Copy(source, destination, overwrite));
    }

    private string EnsureTrailingSlash(string folder)
    {
        string sepChar = Path.DirectorySeparatorChar.ToString();
        string altChar = Path.AltDirectorySeparatorChar.ToString();

        if (!folder.EndsWith(sepChar) && !folder.EndsWith(altChar))
        {
            folder += sepChar;
        }

        return folder;
    }
}