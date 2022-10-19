using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DeepRockLauncher.Core.Models.Settings;
using DeepRockLauncher.WPF.Dialogs;
using DeepRockLauncher.WPF.Windows;

namespace DeepRockLauncher.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Settings Settings;
        public static string LaunchFolder;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            LaunchFolder = Environment.CurrentDirectory;
            Settings = new Settings(LaunchFolder);
            
            if (!Settings.Load() || Settings.IsDefaultValues)
            {
                SettingsWindow settingsWindow = new SettingsWindow();
                QuickDialogs.Info("For this to work properly I am going to need the following information\n" +
                                  "\n" +
                                  "Steam client executable\n" +
                                  "DRG's Steam Quick Save\n" +
                                  "DRG's Game executable (Xbox version, not the steam version)\n" +
                                  "DRG's XBox save file\n" +
                                  "\n" +
                                  "Click Ok to go to the settings window");
                settingsWindow.ShowDialog();
            }
            
            ChooseGameWindow chooseGameWindow = new ChooseGameWindow();
            chooseGameWindow.ShowDialog();

            App.Current?.Shutdown();
        }

    }
}