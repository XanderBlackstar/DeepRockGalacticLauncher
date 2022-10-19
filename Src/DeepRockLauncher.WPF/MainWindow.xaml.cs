using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using DeepRockLauncher.Core.Models;
using DeepRockLauncher.Core.Models.FolderSpy;
using DeepRockLauncher.WPF.Dialogs;

namespace DeepRockLauncher.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<string> _eventList = new ObservableCollection<string>();
        private FolderSpy _monitor;

        public ObservableCollection<string> EventList
        {
            get => _eventList;
            set
            {
                _eventList = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void SetPathClick(object sender, RoutedEventArgs e)
        {
            var path = PathTextBox.Text;
            if (!Directory.Exists(path))
            {
                QuickDialogs.Error("Path does not exist" + Environment.NewLine + path);
                return;
            }

            _monitor = new FolderSpy(path);
            _monitor.DirectoryChanged += MonitorOnDirectoryChanged;
            EventList.Add("Monitoring " + path);
            _monitor.Start();
        }

        private void MonitorOnDirectoryChanged(object? sender, FolderSpyEventArgs e)
        {
            // Invoke event on UI thread
            App.Current?.Dispatcher.Invoke(new Action(delegate
            {
                UpdateEventList(e);
            }));
        }

        private void UpdateEventList(FolderSpyEventArgs folderSpyEventArgs)
        {
            var eventType = folderSpyEventArgs.EventType.ToString();
            string Full = folderSpyEventArgs.Filename;
            string Old = folderSpyEventArgs.OldFilename;
            string TimeStamp = DateTime.Now.ToString();
            
            //string formattedString = string.Format("Type: {0}, Filename: {1}, Old: {2}", eventType, Full, Old);
            //EventList.Add(formattedString);
            EventList.Add("EVENT [Type: "+ eventType + " , Time: " + TimeStamp + "]");
            EventList.Add("   Filename: " + Full);
            if (folderSpyEventArgs.EventType == FolderSpyEventType.Renamed)
                EventList.Add("   Old Filename: " + Old);
        }

        private void AddEventClick(object sender, RoutedEventArgs e)
        {
            EventList.Add("This is a test Element");
        }

        private void SoundOffClick(object sender, RoutedEventArgs e)
        {
            string message = "";

            foreach (var item in EventList)
            {
                message += item + Environment.NewLine;
            }
            
            QuickDialogs.Info(message);
        }

        private void ClearEventClick(object sender, RoutedEventArgs e)
        {
            EventList.Clear();
        }
    }
}