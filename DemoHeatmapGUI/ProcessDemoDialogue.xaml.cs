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
using System.Windows.Shapes;
using DemoHeatmap.demofile;
using DemoHeatmap.math;

namespace DemoHeatmapGUI
{
    /// <summary>
    /// Interaction logic for ProcessDemoDialogue.xaml
    /// </summary>
    public partial class ProcessDemoDialogue : Window
    {
        demoreading.mapstatus stat;

        public ProcessDemoDialogue(demoreading.mapstatus status)
        {
            InitializeComponent();

            stat = status;

            target_demoname.Content = stat.filename;
        }

        private void btn_startprocessing_Click(object sender, RoutedEventArgs e)
        {
            ne.getDataInstanceAsync(stat.activeParser, update_progress, complete);
        }

        private void update_progress(float progress)
        {
            target_progress.Value = progress.Clamp(0, 100);
        }

        private void complete(demodatainstance instance)
        {
            demoreading.saveDemo(instance, stat);
            MessageBox.Show("Done!");
        }
    }
}
