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

using DemoHeatmap.demofile;
using DemoHeatmapGUI.controls;
using System.IO;

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

            //Load demos onto the stack panel
            doLoad();
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

            doLoad();
        }

        public void doLoad()
        {
            List<string> dirsToMake = new List<string>();
            dirsToMake.Add("demos");
            dirsToMake.Add("maps");
            dirsToMake.Add("maps/workshop");
            dirsToMake.Add("bin");

            foreach (string dir in dirsToMake)
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }

            foreach (string dir in Directory.GetFiles("valve"))
            {
                if(!File.Exists(Path.GetDirectoryName(dir) + "/../" + Path.GetFileName(dir)))
                    File.Copy(dir, Path.GetDirectoryName(dir) + "/../" + Path.GetFileName(dir));
            }
            foreach(string dir in Directory.GetFiles("valve/bin"))
                if (!File.Exists(Path.GetDirectoryName(dir) + "/../../bin/" + Path.GetFileName(dir)))  
                    File.Copy(dir, Path.GetDirectoryName(dir) + "/../../bin/" + Path.GetFileName(dir));


            List<demostat> stats = demoreading.getSavedDemos();

            target_demofiles.Children.Clear();

            foreach (demostat stat in stats)
            {
                LoadedDemoControl disp = new LoadedDemoControl(stat, this);
                

                target_demofiles.Children.Add(disp);

            }
        }

        private void btn_click_refresh(object sender, RoutedEventArgs e)
        {
            doLoad();
        }
    }
}
