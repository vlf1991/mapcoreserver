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
using System.Windows.Shapes;
using DemoHeatmap.demofile;
using DemoHeatmap.IO;

namespace DemoHeatmapGUI.controls
{
    /// <summary>
    /// Interaction logic for LoadedDemoControl.xaml
    /// </summary>
    public partial class LoadedDemoControl : UserControl
    {
        // ----------------------------------------------------------- //
        // TODO: CHECK IF MAP IS INSTALLED AND ADD WARNING IF IT INS'T //
        // ----------------------------------------------------------- //

        public LoadedDemoControl(demostat dispStat, MainWindow main)
        {
            InitializeComponent();

            //Auto setting the server logo
            if (dispStat.serverName.ToLower().Contains("mapcore"))
                target_server_logo.Source = new BitmapImage(new Uri("/DemoHeatmapGUI;component/images/servers/serverlogo_mapcore.png", UriKind.Relative));

            //Set the server and map info
            target_server_name.Content = dispStat.serverName;
            target_map_name.Content = dispStat.mapname;

            //CT info
            target_CT_name.Content = dispStat.ctName;
            target_CT_score.Content = dispStat.tName;

            //T info
            target_T_name.Content = dispStat.tName;
            target_T_score.Content = dispStat.tScore;

            //Set the click event
            btn_opendemo.Click += (sender, e) =>
            {
                HeatMapEditorWindow editor = new HeatMapEditorWindow(serialwrite.Binary.ReadFromBinaryFile<demodatainstance>(dispStat.filepath));
                editor.Show();
                //main.IsEnabled = false;
            };

            //Add a context menu
            
        }
    }
}
