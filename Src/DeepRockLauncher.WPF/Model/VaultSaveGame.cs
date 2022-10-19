using System.Windows.Media.Imaging;

namespace DeepRockLauncher.WPF.Model;

public class VaultSaveGame
{
    public string Filename { get; set; }
    public string Title { get; set; }
    public BitmapImage Image { get; set; }
}