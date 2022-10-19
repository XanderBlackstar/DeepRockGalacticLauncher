using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DeepRockLauncher.WPF.Model;

public class BoundColorString : INotifyPropertyChanged
{
    private string _text;
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            OnPropertyChanged();
        }
    }

    private SolidColorBrush _foreground;
    public SolidColorBrush Foreground
    {
        get => _foreground;
        set
        {
            _foreground = value;
            OnPropertyChanged();
        }
    }

    private SolidColorBrush _background;

    public SolidColorBrush Background
    {
        get => _background;
        set
        {
            _background = value;
            OnPropertyChanged();
        }
    }

    public BoundColorString(string text)
    {
        Text = text;
    }

    public void WeGood()
    {
        Foreground = new SolidColorBrush(Colors.LawnGreen);
    }

    public void HoldOn()
    {
        Foreground = new SolidColorBrush(Colors.Yellow);
    }

    public void NoWayJose()
    {
        Foreground = new SolidColorBrush(Colors.Salmon);
    }

    public void IDontHaveAnOpinion()
    {
        Foreground = new SolidColorBrush(Colors.DarkGray);
    }
    
    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    #endregion
    
}