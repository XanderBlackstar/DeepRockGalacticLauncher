using System;

namespace DeepRockLauncher.Core.Models.SavesManager;

public class SaveFileInformation
{
    public GameService GameService { get; set; }
    public DateTime TimeStamp { get; set; }

    public string GetFilename()
    {
        string filename = string.Format("{0} - {1:O}.sav",
            GameService.ToString(), TimeStamp);

        filename = filename.Replace(":", ".");
        return filename;
    }
}