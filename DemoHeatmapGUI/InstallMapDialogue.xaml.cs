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
using DemoHeatmap.demofile;
using DemoHeatmap.steam;


namespace DemoHeatmapGUI
{
    /// <summary>
    /// Interaction logic for InstallMapDialogue.xaml
    /// </summary>
    public partial class InstallMapDialogue : Window
    {
        public InstallMapDialogue(demoreading.mapstatus status)
        {
            InitializeComponent();

            if(status.isWorkshop)
            {
                WorkshopFile workshopPrev = WorkshopFile.get(new WorkshopURI(status.activeParser.Map));
                target_mapname.Content = "Workshop: " + workshopPrev.response.publishedfiledetails[0].filename;

                target_previewImage.Source = //new BitmapImage(new Uri(workshopPrev.response.publishedfiledetails[0].hcontent_preview, UriKind.Absolute));


                Workshop.downloadUGCImage(workshopPrev.response.publishedfiledetails[0].preview_url);

                //new BitmapImage( workshopPrev.response.publishedfiledetails[0].hcontent_preview);
            }

        }
    }
}
