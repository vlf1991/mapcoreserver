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

namespace DemoHeatmapGUI
{
    /// <summary>
    /// Interaction logic for HeatMapEditorWindow.xaml
    /// </summary>
    public partial class HeatMapEditorWindow : Window
    {
        public HeatMapEditorWindow(demodatainstance instance)
        {
            InitializeComponent();

            #region toggle players
            Dictionary<long, bool> enabledSteamIDS = new Dictionary<long, bool>();
            foreach(p_Player player in instance.players.Values.ToArray())
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
    }
}
