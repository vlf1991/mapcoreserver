﻿using System;
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
using System.Windows.Shapes;
using DemoHeatmap.demofile;
using DemoHeatmapGUI.controls;

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
                    ProcessDemoDialogue procDemo = new ProcessDemoDialogue(status);
                    procDemo.Show();
                }
                else
                {
                    InstallMapDialogue insMap = new InstallMapDialogue(status);
                    insMap.Show();
                }
            }
        }

        private void doLoad()
        {
            List<demostat> stats = demoreading.getSavedDemos();

            foreach(demostat stat in stats)
            {
                LoadedDemoControl disp = new LoadedDemoControl(stat, this);
                

                target_demofiles.Children.Add(disp);

            }
        }
    }
}
