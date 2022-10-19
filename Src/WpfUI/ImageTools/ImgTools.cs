using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfUI.Exceptions;

namespace WpfUI.ImageTools;

public static class ImgTools
{
    /// <summary>
    /// Fetch application wide Color resource
    /// </summary>
    /// <param name="xKey"></param>
    /// <returns></returns>
    public static SolidColorBrush GetResourceColor(string xKey)
    {
        object res;
        try
        {
            res = Application.Current.FindResource(xKey);
        }
        catch
        {
            try
            {
                res = Application.Current.FindResource("ColorNotFound");
            }
            catch (Exception e)
            {
                throw new ResourceBrushNotFoundException(
                    $"Could not find Resource Brush '{xKey}', nor the fallback value");
            }
        }

        return (SolidColorBrush)res;
    }
    
    /// <summary>
    /// Fetch application wide Image resource
    /// </summary>
    /// <param name="resourceName"></param>
    /// <returns></returns>
    public static BitmapImage GetResourceBitmap(string resourceName)
    {
        Uri uri = new Uri(resourceName, UriKind.RelativeOrAbsolute);
        return GetResourceBitmap(uri);
    }

    /// <summary>
    /// Fetch application wide Image resource
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static BitmapImage GetResourceBitmap(ImageSource source)
    {
        return GetResourceBitmap(source.ToString());
    }

    /// <summary>
    /// Fetch application wide Image resource
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static BitmapImage GetResourceBitmap(Uri uri)
    {
        BitmapImage? img = null;
        
        try
        {
            // This line doesn't trigger an exception if the resource isn't found ...
            img = new BitmapImage(uri);
            
            // ... Accessing PixelHeight will!
            var resourceValidityCheck = img.PixelHeight;
        }
        catch
        {
            img = new BitmapImage(new Uri("pack://application:,,,/WpfUI;component/Images/FileNotFound.png", UriKind.RelativeOrAbsolute));
        }

        return img;
    }
    

}