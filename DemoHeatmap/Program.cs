using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoInfo;
using System.IO;
using System.Drawing;
using Imaging.DDSReader;
using System.Diagnostics;
using System.IO.Compression;

namespace DemoHeatmap
{
    public static class ExtensionMethods
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static int remap(this int val, int min1, int max1, int min2, int max2)
        {
            return min1 + (val - min1) * (max2 - min2) / (max1 - min1);
        }

        public static int Abs(this int val)
        {
            return Math.Abs(val);
        }
    }

    public class Vector2
    {
        public float x;
        public float y;

        public Vector2(float X, float Y)
        {
            x = X;
            y = Y;
        }
    }

    class Program
    {
        static int Main(string[] args)
        {
            string serverfolder = "";
            string mapname = "";

            string demofile = "";
            string mapfile = "";


            #region args

            if (args.Length != 2)
            {
                Console.WriteLine("Usage: demoheatmap.exe <demopath> <mapname>");
                Console.ReadLine();
                return 1;
            }

            Debug.Info("Processing demo {0}, mapfile {1}", args[0], args[1]);

            //Setting strings

            mapname = args[1].Replace(".bsp", "");
            serverfolder = "files/" + mapname;

            Debug.Info("Working directory {0}", serverfolder);

            demofile = args[0];
            mapfile = args[1];

            #endregion args

            #region servercheck

            if (Directory.Exists(serverfolder + "/currentlybugged"))
            {
                Debug.Warn("Working directory already exists...");
            }
            else
            {
                Debug.Info("Creating working directory on disk, adding files");
                Directory.CreateDirectory(serverfolder);
                File.Copy(mapfile, serverfolder + "/" + Path.GetFileName(mapfile), true);

                mapfile = serverfolder + "/" + Path.GetFileName(mapfile);
            }

            #endregion servercheck


            #region extractfiles

            //Construct arguments
            string extractzip = Path.GetFullPath(serverfolder) + "/" + mapname + ".zip";

            string extractarg = "-extract \"" + Path.GetFullPath(mapfile) + "\" \"" + extractzip + "\"";

            

            //Console.WriteLine(extractarg);

            Process bspzip = new Process();
            bspzip.StartInfo = new ProcessStartInfo
            {
                FileName = Environment.CurrentDirectory + "/bspzip.exe",

                Arguments = extractarg,
                UseShellExecute = false
            };
            bspzip.Start();
            bspzip.WaitForExit();

            //Extract zip to folder

            Debug.Info("Extracting...");

            using (var zip = ZipFile.OpenRead(extractzip))
            {
                foreach(var entry in zip.Entries.ToList())
                {
                    if (entry.FullName.ToLower().Contains("overviews"))
                    {
                        Directory.CreateDirectory(Path.GetFullPath(serverfolder) + "/" + Path.GetDirectoryName(entry.FullName));


                        entry.ExtractToFile(Path.GetFullPath(serverfolder) + "/" + entry.FullName, true);
                        Debug.Success("Extracted " + entry.FullName);
                    }
                }
            }

            #endregion

            #region workradar

            Radar demoradar = new Radar(serverfolder + "/resource/overviews/" + mapname + ".txt");

            Debug.Success("Radar file parsed");

            Bitmap convert = DDS.LoadImage(serverfolder + "/resource/" + demoradar.matpath + ".dds", false);//new Bitmap("dust2radar.jpg");
            convert.Save(serverfolder + "/resource/overviews/" + mapname + "_radar.png");

            Debug.Success("Radar texture converted and saved as type .png");

            #endregion workradar


            #region demoparsing

            grayimage image = new grayimage(1024, 1024);

            DemoParser parser = new DemoParser(File.OpenRead(demofile));

            Dictionary<string, List <Vector2>> fulldata = new Dictionary<string, List<Vector2>>();

            string[] types = {"shots", "deaths"};

            foreach(string type in types)
            {
                fulldata.Add(type, new List<Vector2>());
            }


            parser.WeaponFired += (object sender, WeaponFiredEventArgs e) =>
            {
                Debug.Info("Added data for shots at {0}, {1}", e.Shooter.Position.X, e.Shooter.Position.Y);
                fulldata["shots"].Add(new Vector2(e.Shooter.Position.X, e.Shooter.Position.Y));

                /*
                int posx = Convert.ToInt32((e.Shooter.Position.X - demoradar.pos_x) / demoradar.scale);
                int posy = Convert.ToInt32((e.Shooter.Position.Y - demoradar.pos_y) / demoradar.scale) + 1024;

                Debug.Info("X: {0} Y: {1}", posx, posy);

                image.SetPixel(posx.Clamp(0, 1023), posy.Clamp(0, 1023).remap(0, 1023, 1023, 0) + 1024, image.bitdepth);
                */
            };

            parser.PlayerKilled += (object sender, PlayerKilledEventArgs e) =>
            {
                Debug.Info("Added data for deaths at {0}, {1}", e.Victim.Position.X, e.Victim.Position.Y);
                fulldata["deaths"].Add(new Vector2(e.Victim.Position.X, e.Victim.Position.Y));
            };

            parser.ParseHeader();

            try
            {
                while (parser.ParseNextTick() != false)
                {

                }
            }
            catch
            {
                Debug.Warn("Attempted to read past end of stream...");
            }

            Debug.Success("Finished reading demo file");

            foreach(KeyValuePair<string, List<Vector2>> entry in fulldata)
            {
                grayimage workImage = new grayimage(1024, 1024);

                Debug.Info("Processing heat data for {0}", entry.Key);
                foreach(Vector2 p in entry.Value)
                {
                    int posx = Convert.ToInt32((p.x - demoradar.pos_x) / demoradar.scale);
                    int posy = Convert.ToInt32((p.y - demoradar.pos_y) / demoradar.scale) + 1024;

                    workImage.SetPixel(posx.Clamp(0, 1023), posy.Clamp(0, 1023).remap(0, 1023, 1023, 0) + 1024, image.bitdepth);
                }
                Debug.Success("Completed image for {0}. Saving to disk...", entry.Key);

                Directory.CreateDirectory(serverfolder + "/exported/");
                workImage.toBitmap().Save(serverfolder + "/exported/" + mapname + "_heatmap_" + entry.Key + ".png");

                Debug.Info("Saved image");

            }

            //image = image.KernalFastBlur(10, 1);

            

#endregion demoparsing

            /*

            //Bitmap fromfile = DevILSharp.IL.LoadImage("de_dust2_radar.dds");
            //Console.Write(DevILSharp.IL.LoadImage("de_dust2_radar.dds"));
            Bitmap convert = DDS.LoadImage("cs_market_v016_radar.dds", false);//new Bitmap("dust2radar.jpg");
            convert.Save("Radar.png");

            grayimage image = new grayimage(1024, 1024);

            Radar demoradar = new Radar("cs_market_v016.txt");
            //Bitmap image = new Bitmap(1024, 1024);

            Console.Write(demoradar.pos_y);
            Console.Write(demoradar.pos_x);
            Console.Write(demoradar.scale);


            DemoParser parser = new DemoParser(File.OpenRead("cs_market.dem"));

            parser.WeaponFired += (object sender, WeaponFiredEventArgs e) =>
            {
                int posx = Convert.ToInt32((e.Shooter.Position.X - demoradar.pos_x) / demoradar.scale);
                int posy = Convert.ToInt32((e.Shooter.Position.Y - demoradar.pos_y) / demoradar.scale) + 1024;

                Console.WriteLine("X: {0} Y: {1}", posx, posy);

                image.SetPixel(posx.Clamp(0, 1023), posy.Clamp(0, 1023).remap(0, 1023, 1023, 0) + 1024, image.bitdepth);
            };

            parser.ParseHeader();

            try
            {
                while (parser.ParseNextTick() != false)
                {

                }
            }
            catch
            {
                Console.WriteLine("Attempted to read past end of stream...");
            }

            //image = image.KernalFastBlur(10, 1);

            image.toBitmap().Save("heatmap_de_dust2.png");


            */

            Console.Write("End... Press any key to close.");
            Console.ReadLine();


            return 0;


            /*STEPS

            -------------------------------------------

[]          - READ SOME MAP CONTEXT (Map file name, version etc)

            -------------------------------------------

[]          - CHECK IF DATA EXISTS ON SERVER
[]          - COPY FILES TO FOLDER

            -------------------------------------------

[]          - EXTRACT RADAR FILES FROM BSP
[]          - PARSE RADAR COORDINATES
            
            -------------------------------------------

[]          - READ AND PARSE DEMO
[]          - GENERATE STATIC PIXEL MAPS
[]          - SAVE TO DISK
            
            -------------------------------------------

            - GENERATE WEB DATA

            -------------------------------------------

            - PUSH EVERYTHING BACK






            */
        }
    }
}
