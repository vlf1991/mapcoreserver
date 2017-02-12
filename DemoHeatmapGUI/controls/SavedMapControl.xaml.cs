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
using DemoHeatmap.steam;
using LogicalImageEditing.conversions;

namespace DemoHeatmapGUI.controls
{
    /// <summary>
    /// Interaction logic for SavedMapControl.xaml
    /// </summary>
    public partial class SavedMapControl : UserControl
    {
        public SavedMapControl(mapData mapdat, string mapname)
        {
            InitializeComponent();

            //Sets all the params respectively
            target_mapname.Header = mapname;
            target_offset_x.Content = "Offset X: " + mapdat.radar.pos_x;
            target_offset_y.Content = "Offset Y: " + mapdat.radar.pos_y;
            target_scale.Content = "Radar Scale: " + mapdat.radar.scale;
            target_radarpreview.Source = mapdat.image_radar.toBitMapImage();
        }
    }
}
