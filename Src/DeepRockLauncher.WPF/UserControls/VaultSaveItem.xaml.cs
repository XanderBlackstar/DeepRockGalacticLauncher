using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DeepRockLauncher.WPF.UserControls;

public partial class VaultSaveItem : UserControl
{
    public VaultSaveItem()
    {
        InitializeComponent();
    }

    
    public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
        "Image", typeof(BitmapImage), typeof(VaultSaveItem), new PropertyMetadata(default(BitmapImage), ImagePropertyChangedCallback));
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        "Title", typeof(string), typeof(VaultSaveItem), new PropertyMetadata(default(string), TitlePropertyChangedCallback));


    public BitmapImage Image
    {
        get { return (BitmapImage) GetValue(ImageProperty); }
        set { SetValue(ImageProperty, value); }
    }
    public string Title
    {
        get { return (string) GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }


    private static void ImagePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // No need to do anything here
    }
    private static void TitlePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // No need to do anything here
    }

}