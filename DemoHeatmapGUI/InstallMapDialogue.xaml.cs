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
        demoreading.mapstatus globalStat;
        WorkshopFile workshopfile;

        public InstallMapDialogue(demoreading.mapstatus status)
        {
            InitializeComponent();

            globalStat = status;

            if (status.isWorkshop)
            {
                //Download the workshop object (only info)
                workshopfile = WorkshopFile.get(new WorkshopURI(globalStat.activeParser.Map));

                //Set mapname title
                target_mapname.Content = workshopfile.response.publishedfiledetails[0].title + "   |   " + workshopfile.response.publishedfiledetails[0].file_size / 1024 / 1024 + " mb";

                //Temporarily using the description for this box right now
                string desc = workshopfile.response.publishedfiledetails[0].description;
                if (desc.Length > 128)
                    desc = desc.Substring(0, 128) + "...";
                target_mapcreator.Content = desc;

                //Set the preview image
                target_previewImage.Source = Workshop.downloadUGCImage(workshopfile.response.publishedfiledetails[0].preview_url);
            }

        }

        private void Install_Click(object sender, RoutedEventArgs e)
        {
            Workshop.download(workshopfile, "maps/workshop/" + globalStat.localname); //Download to disk
            bspinfo.UnpackBSP("maps/workshop/" + globalStat.localname, "maps/workshop/" + globalStat.localname);
            MessageBox.Show("Map installed!");
        }
    }
}
