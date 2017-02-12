using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using DemoHeatmap.IO;
using DemoHeatmap;
using DemoHeatmap.demofile;
using DemoHeatmapGUI.controls;
using System.IO;
using DemoHeatmap.steam;

namespace DemoHeatmapGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Run {CHECK} first time setup before running anything else
            if (!File.Exists("demos"))
            {
                setup.runFirstTimeSetup();
            }

            //Check for updates before running any other code
            checkForUpdates();

            //Load demos onto the stack panel
            doLoadDemos();

            //Load maps
            doLoadMaps();
        }

        //Update checking method
        private void checkForUpdates()
        {
            if(!version.isLatestVersion())
            {
                MessageBoxResult result = MessageBox.Show("There is an update availible!\n\nVersion " + version.getLatestVersion() + " has been released, would you like to download it?", "Update availible", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                if(result == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(version.getDownloadURL());
                    Environment.Exit(0);
                }
            }
        }

        private void DemoPanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // MessageBox.Show(files[0]);

                demoreading.mapstatus status = demoreading.isDownloaded(files[0]);
                if(status.isDownloaded)
                {
                    //Process demo file config window
                    ProcessDemoDialogue procDemo = new ProcessDemoDialogue(status, this);
                    procDemo.Show();
                }
                else
                {
                    InstallMapDialogue insMap = new InstallMapDialogue(status);
                    insMap.Show();
                }
            }

            doLoadDemos();
        }

        public void doLoadDemos()
        {
            List<demostat> stats = demoreading.getSavedDemos();

            target_demofiles.Children.Clear();

            foreach (demostat stat in stats)
            {
                LoadedDemoControl disp = new LoadedDemoControl(stat, this);
                
                target_demofiles.Children.Add(disp);
            }
        }

        public void doLoadMaps()
        {
            Dictionary<string, mapData> maps = bspinfo.getSavedMaps(); //Retrieve all the maps on disk
            target_mapfiles.Children.Clear();

            foreach (string mapPath in maps.Keys)
            {
                SavedMapControl mapControl = new SavedMapControl(maps[mapPath], Path.GetFileNameWithoutExtension(mapPath));
                target_mapfiles.Children.Add(mapControl);
            }
        }

        private void btn_click_refresh(object sender, RoutedEventArgs e)
        {
            doLoadDemos();
        }
    }
}
