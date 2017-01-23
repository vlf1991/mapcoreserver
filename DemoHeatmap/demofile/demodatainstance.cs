using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoInfo;

namespace DemoHeatmap.demofile
{
    //Each round for each player is packed and put into the player's list
    [Serializable]
    public class p_Round
    {
        //A list of all the positions the player was
        public List<vector3> positions = new List<vector3>();
        public List<vector3> shotsFired = new List<vector3>(); //And where they shot
    }

    //Holds all information about what the player did that game
    [Serializable]
    public class p_Player
    {
        public long steamID;
        public string steamName;

        public List<p_Round> rounds = new List<p_Round>();
        public List<vector3> deathPositions = new List<vector3>();

        public p_Player(string steamname, long steamid)
        {
            steamName = steamname;
            steamID = steamid;

            //Init with at least one round
            rounds.Add(new p_Round());
        }
    }

    [Serializable]
    public class p_Grenade
    {
        //Where did it blow up?
        public vector3 impactSpot = new vector3();
    }

    //The team class.
    [Serializable]
    public class p_Team
    {
        //Holds a list of players that are on that team
        public Dictionary<int, p_Player> players = new Dictionary<int, p_Player>();

        //Holds a list of the grenades that were thrown
        public List<p_Grenade> grenades = new List<p_Grenade>();
    }

    [Serializable]
    public class demodatainstance
    {
        //Holds a list of [2] teams, CT and T side 
        public List<p_Team> teams = new List<p_Team>();

        //General Information about the demofile
        public string mapname;
        public string ctName;
        public string tName;
        public string serverName;
        public int ctScore;
        public int tScore;

        //Initialisation step, should fire when parsing the demo for first time
        public demodatainstance(DemoParser parser)
        {
            //Assign basic information
            mapname = parser.Map;
            ctName = parser.CTClanName;
            tName = parser.TClanName;
            serverName = parser.Header.ServerName;
            ctScore = parser.CTScore;
            tScore = parser.TScore;

            //Subscribe to events here
            parser.WeaponFired += (object o, WeaponFiredEventArgs e) =>
            {
                //shotPositions.Add(new vector3(e.Shooter.Position.X, e.Shooter.Position.Y, e.Shooter.Position.Z));
                int team = 0;
                if(e.Shooter.Team == Team.Terrorist)
                    team = 1;

                //Add the weapon fired position to that players most recent round
                teams[team].players[e.Shooter.EntityID].rounds.Last().shotsFired.Add(new vector3(
                                                                                        e.Shooter.Position.X, 
                                                                                        e.Shooter.Position.Y, 
                                                                                        e.Shooter.Position.Z));
            };

            //Add a new round to each player on each team, on round start
            parser.RoundStart += (object o, RoundStartedEventArgs e) =>
            {
                foreach(p_Team recTeam in teams)
                    foreach(p_Player player in recTeam.players.Values.ToList())
                        player.rounds.Add(new p_Round());
            };

            //Log all player deaths
            parser.PlayerKilled += (object o, PlayerKilledEventArgs e) =>
            {
                //Do a team check
                int team = 0;
                if (e.Victim.Team == Team.Terrorist)
                    team = 1;

                //Add the player death
                teams[team].players[e.Victim.EntityID].deathPositions.Add(new vector3(
                                                                                        e.Victim.Position.X, 
                                                                                        e.Victim.Position.Y, 
                                                                                        e.Victim.Position.Z));
            };


            //Add T and CT teams to array, init
            teams.Add(new p_Team());
            teams.Add(new p_Team());

            try
            {
                int currentRound = 0;

                //Loop through ticks here
                while (parser.ParseNextTick() != false)
                {
                    foreach(DemoInfo.Player player in parser.PlayingParticipants)
                    {
                        //Find out which team the player is on
                        int team = 0;
                        if (player.Team == Team.Terrorist)
                            team = 1;

                        if (!teams[team].players.ContainsKey(player.EntityID))
                        {
                            teams[team].players.Add(player.EntityID, new p_Player(player.Name, player.SteamID));
                        }

                        //Check if the player is alive
                        if (player.IsAlive)
                        {
                            //Add the players position
                            teams[team].players[player.EntityID].rounds[currentRound].positions.Add(new vector3(player.Position.X, player.Position.Y, player.Position.Z));
                        }
                    }
                }
            }
            catch
            {
                //TODO: Work out the error types
                Console.WriteLine("Error while parsing, usual...");
            }
        }
    }
}
