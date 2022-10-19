using System.Windows.Media;
using WpfUI.ImageTools;

namespace WpfUI.ColoringListBox;

public class ColorableListBoxItem
{
    public string Text { get; set; }
    public Brush Foreground { get; set; }
    public Brush Background { get; set; }

    public ColorableListBoxItem(string text, ColoringState color)
    {
        Text = text;
        Background = ImgTools.GetResourceColor("ColorBlack");

        switch (color)
        {
            case ColoringState.Success:
                Foreground = ImgTools.GetResourceColor("ColorTextGreen");
                break;
            case ColoringState.Warning:
                Foreground = ImgTools.GetResourceColor("ColorTextYellow");
                break;
            case ColoringState.Error:
                Foreground = ImgTools.GetResourceColor("ColorTextRed");
                break;
            default:
                Foreground = ImgTools.GetResourceColor("ColorTextGray");
                break;
        }
    }
}