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
using DemoInfo;


namespace DemoHeatmapGUI
{
    /// <summary>
    /// Interaction logic for InstallMapDialogue.xaml
    /// </summary>
    public partial class InstallMapDialogue : Window
    {
        public InstallMapDialogue(DemoParser parser)
        {
            InitializeComponent();
            target_mapname.Content = parser.Map;
        }
    }
}
