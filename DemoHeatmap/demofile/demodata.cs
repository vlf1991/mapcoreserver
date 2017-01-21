using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoInfo;
using System.IO;
using DemoHeatmap.math;

namespace DemoHeatmap.demofile
{
    public class demodata
    {
        public List<Dictionary<int, List<List<vector3>>>> positions = new List<Dictionary<int, List<List<vector3>>>>();

        public List<vector3> shotPositions = new List<vector3>();
        public List<vector3> deathPositions = new List<vector3>();
        public List<vector3> smokePositions = new List<vector3>();
        public List<vector3> bombplantPositions = new List<vector3>();

        public demodata(string path)
        {


            DemoParser scan2 = new DemoParser(File.OpenRead(path));
            scan2.RoundStart += (object o, RoundStartedEventArgs e) =>
            {
                for(int i = 0; i < 2; i++)
                foreach (List<List<vector3>> chunks in positions[i].Values.ToList())
                {
                    chunks.Add(new List<vector3>());
                }
            };

            scan2.WeaponFired += (object o, WeaponFiredEventArgs e) =>
            {
                shotPositions.Add(new vector3(e.Shooter.Position.X, e.Shooter.Position.Y, e.Shooter.Position.Z));
            };
            scan2.PlayerKilled += (object o, PlayerKilledEventArgs e) =>
            {
                deathPositions.Add(new vector3(e.Victim.Position.X, e.Victim.Position.Y, e.Victim.Position.Z));
            };
            scan2.SmokeNadeStarted += (object o, SmokeEventArgs e) =>
            {
                smokePositions.Add(new vector3(e.Position.X, e.Position.Y, e.Position.Z));
            };
            scan2.BombPlanted += (object o, BombEventArgs e) =>
            {
                bombplantPositions.Add(new vector3(e.Player.Position.X, e.Player.Position.Y, e.Player.Position.Z));
            };

            scan2.ParseHeader();

            Debug.progressBar("Reading", 0);

            positions.Add(new Dictionary<int, List<List<vector3>>>());
            positions.Add(new Dictionary<int, List<List<vector3>>>());

            int c = 0;
            try
            {
                while (scan2.ParseNextTick() != false)
                {
                    foreach (DemoInfo.Player info in scan2.PlayingParticipants)
                    {
                        int team = 0;

                        if (info.Team == Team.Terrorist)
                            team = 1;

                        if (!positions[team].ContainsKey(info.EntityID))
                        {
                            positions[team].Add(info.EntityID, new List<List<vector3>>());
                            Debug.Log("+{0}/", info.EntityID);
                        }

                        if (info.IsAlive)
                            if (positions[team][info.EntityID].Count != 0)
                                positions[team][info.EntityID].Last().Add(new vector3(info.Position.X, info.Position.Y, info.Position.Z));

                    }

                    c++;
                    if (c % 500 == 0)
                        try
                        {
                            Debug.updateProgressBar(Convert.ToInt32(scan2.ParsingProgess * 100).Clamp(0, 100));
                        }
                        catch
                        {
                            
                        }
                }
            }
            catch
            {
                Debug.Error("Something went wrong while reading stream");
            }

            Debug.exitProgressBar();
        }
    }
}
