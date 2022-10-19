using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI.ColoringListBox;

public partial class ColorListBox : UserControl, INotifyPropertyChanged
{
    public ColorListBox()
    {
        InitializeComponent();
    }
    
    /*
    private ColorableListBoxItem _selectedItem;
    public ColorableListBoxItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            OnPropertyChanged();
        }
    }
    */
    
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        "Items", 
        typeof(ObservableCollection<ColorableListBoxItem>), 
        typeof(ColorListBox), 
        new PropertyMetadata(default(ObservableCollection<ColorableListBoxItem>)));

    public ObservableCollection<ColorableListBoxItem> Items
    {
        get { return (ObservableCollection<ColorableListBoxItem>)GetValue(ItemsProperty); }
        set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        "SelectedItem", typeof(ColorableListBoxItem), typeof(ColorListBox), new PropertyMetadata(default(ColorableListBoxItem)));

    public ColorableListBoxItem SelectedItem
    {
        get { return (ColorableListBoxItem) GetValue(SelectedItemProperty); }
        set { SetValue(SelectedItemProperty, value); }
    }
    
    public int SelectedIndex
    {
        get => TheListBox.SelectedIndex;
        set
        {
            TheListBox.SelectedIndex = value;
        }
    }

    public void ScrollIntoView(ColorableListBoxItem item)
    {
        TheListBox.ScrollIntoView(item);
    }
    
    
    // Create the OnPropertyChanged method to raise the event
    // The calling member's name will be used as the parameter.
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    
}