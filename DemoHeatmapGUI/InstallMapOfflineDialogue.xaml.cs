using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using System.Drawing;
using LogicalImageEditing.conversions;
using Imaging.DDSReader;
using DemoHeatmap.steam;
using DemoHeatmap.IO;


namespace DemoHeatmapGUI
{
    /// <summary>
    /// Interaction logic for InstallMapOfflineDialogue.xaml
    /// </summary>
    public partial class InstallMapOfflineDialogue : Window
    {
        mapData insmap = new mapData();

        public InstallMapOfflineDialogue(mapData editmap, string mapname = "")
        {
            InitializeComponent();

            if (editmap.image_radar != null) //If the map file already exists
            {
                insmap = editmap;

                target_displayimage.Source = editmap.image_radar.toBitMapImage();

                input_offset_x.Value = editmap.radar.pos_x;
                input_offset_y.Value = editmap.radar.pos_y;
                input_radar_scale.Value = (decimal)editmap.radar.scale;

                this.Title = "Editing " + mapname;
            }
            else //If it doesn't
            {
                this.Title = "Setting up new map";
            }

            if (mapname != "")
            {
                input_mapname.Text = mapname; //Should usually receive a stripped filename
                input_mapname.IsEnabled = false; //Disable the mapname box so it doest break
            }

        }

        private void numericTextBoxHandler(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        //Handles the open image button
        private void btn_select_radar_image(object sender, RoutedEventArgs e)
        {
            //file dialogue
            OpenFileDialog openf = new OpenFileDialog();
            openf.Filter = "Image files (*.jpg, *.jpeg, *.png, *.dds) | *.jpg; *.jpeg; *.png; *.dds";

            if (openf.ShowDialog() == true)
            {
                target_selected_radar_filename.Content = Path.GetFileName(openf.FileName);

                Bitmap loadimage = new Bitmap(1,1);//Create temp bitmap before assigning it
                if(Path.GetExtension(openf.FileName) == ".dds")//If its a dds image, convert that
                    loadimage = DDS.LoadImage(openf.FileName, false); //Loads the bitmap imag from a dds source
                else
                    loadimage = new Bitmap(openf.FileName); //If it is a valid image file then load that

                
                target_displayimage.Source = loadimage.toBitMapImage(); //Display the image

                //Add reference of bitmap to the mapData
                insmap.image_radar = loadimage;
            }
        }

        //Handles saving and error correction
        private void btn_save_map(object sender, RoutedEventArgs e)
        {
            //VALIDATION OF PARAMS
            List<string> validationErrors = new List<string>();

            if(input_mapname.Text == null || input_mapname.Text.Replace(" ", "") == "")
                validationErrors.Add("No mapname specified");

            if (input_radar_scale.Value <= 0)
                validationErrors.Add("Scale must be greater than 0");

            if (insmap.image_radar == null)
                validationErrors.Add("Image must be added to save");

            if(validationErrors.Count > 0)
            {
                string outputErrors = "Errors: \n\n";
                foreach(string error in validationErrors)
                {
                    outputErrors = outputErrors + "- " + error + "\n";
                }

                MessageBox.Show(outputErrors, "Couldn't validate.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else //Do save
            {
                //Edit params
                Radar rad = new Radar();
                rad.pos_x = (int)input_offset_x.Value;
                rad.pos_y = (int)input_offset_y.Value;
                rad.scale = (int)input_radar_scale.Value;

                insmap.radar = rad;

                serialwrite.Binary.WriteToBinaryFile<mapData>(Environment.CurrentDirectory + "/maps/" + input_mapname.Text + ".maprad", insmap);

                MessageBox.Show("Saved!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
