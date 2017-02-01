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
using System.Windows.Shapes;
using DemoHeatmap.demofile;
using System.Drawing;
using SharpHeatMaps.imaging;
using DemoInfo;
using DemoHeatmap;
using SharpHeatMaps.heatmaps;
using SharpHeatMaps.gradients;
using DemoHeatmap.steam;


namespace DemoHeatmapGUI
{
    /// <summary>
    /// Interaction logic for HeatMapEditorWindow.xaml
    /// </summary>
    public partial class HeatMapEditorWindow : Window
    {
        demodatainstance refInstance;
        Dictionary<long, bool> enabledSteamIDS = new Dictionary<long, bool>();

        public HeatMapEditorWindow(demodatainstance ins)
        {
            InitializeComponent();

            refInstance = ins;

            #region toggle players
            foreach(p_Player player in refInstance.players.Values.ToArray())
            {
                //Add all the players into the initialised box
                enabledSteamIDS.Add(player.steamID, true);

                //New checkbox with player name
                CheckBox playerCheckBox = new CheckBox();
                playerCheckBox.Content = player.steamName;

                //Enable it on the player dict
                playerCheckBox.Checked += (sender, e) =>
                {
                    enabledSteamIDS[player.steamID] = true;
                }; //Or disable
                playerCheckBox.Unchecked += (sender, e) =>
                {
                    enabledSteamIDS[player.steamID] = false;
                };

                //Auto check
                playerCheckBox.IsChecked = true;

                if (player.rounds.Count > 0)
                {
                    //Going by last round because of bug in parsing saying round is CT on first
                    if (player.rounds.Last().teamPlayedOnRound == p_Team_Identifier.counterterrorist)
                        target_playerpanel_T.Children.Add(playerCheckBox);
                    else
                        target_playerpanel_CT.Children.Add(playerCheckBox);
                }
            }
            #endregion

        }

        private void btn_Generate_Click(object sender, RoutedEventArgs e)
        {
            mapData mapfile = demoreading.loadMapFromDisk(refInstance.info.mapname);
            //First step: Make a grey dark version of the radar.
            Bitmap backgroundRadar = bitmapfilters.brightness(bitmapfilters.greyScaleAverage(mapfile.image_radar), 0.3f);

            //Third step: Start making heatmaps
            densitymap density_shotsfired = new densitymap();

            //Legacy, Make the camera object
            camera cam = new camera();
            cam.offset = new vector2(mapfile.radar.pos_x, mapfile.radar.pos_y);
            cam.scale = mapfile.radar.scale;

            foreach (p_Player plyr in refInstance.players.Values.ToList())
            {
                if (enabledSteamIDS[plyr.steamID]) //Check if that player was enabled
                {
                    foreach (p_Round rnd in plyr.rounds)
                    {
                        foreach (vector3 shot in rnd.shotsFired)
                        {
                            vector2 screenPos = transforms.worldToScreenSpace(shot, cam);
                            density_shotsfired.createHeatMapSplodge((int)screenPos.x, (int)screenPos.y, (int)val_dialation_slider.Value);
                        }
                    }
                }
            }

            //If blur is enabled, use the 3x3 blur filter
            if(toggle_blur.IsChecked.Value)
                density_shotsfired.averageBlur3x3();


            density_shotsfired.normalise((float)val_samplemidpoint_slider.Value / (float)val_samplemidpoint_slider.Maximum);

            Bitmap output = density_shotsfired.toBitMap();
            output = gradients.purple_red_yellow_withalpha.applyToImage(output);

            output = bitmapfilters.alphaOver(backgroundRadar, output);

            target_currentview.set_image(output);
        }
    }
}
