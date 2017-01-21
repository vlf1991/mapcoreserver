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
using System.Threading;
using DemoHeatmap.demofile;
using DemoHeatmap.math;
using ImageProcessor;
using System.Drawing.Imaging;
using DemoHeatmap.steam;
using DemoHeatmap.IO;

namespace DemoHeatmap
{
    class Program
    {
        static int Main(string[] args)
        {
            //First thing: Read the demo file header. This gives us all the information we need to run the program from
            //Since it stores stuff like length, mapname etc.

            //Do parsing
            DemoParser demofile = new DemoParser(File.OpenRead("sanchez.dem"));
            demofile.ParseHeader();

            //TRY AND FIND INSTANCE OF MAP ON DISK
            Debug.Info("Scanning for map on disk");

            //Do workshop check
            bool isWorkshop = false;
            string checkPath = "maps/";
            string localname = demofile.Map;

            if (demofile.Map.StartsWith("workshop")) //Detected its a workshop map
            {
                isWorkshop = true;
                checkPath = "maps/workshop/"; //Switch to the workshopped folder
                Debug.Log("Map workshopped");

                //Sets the local name to something that will not cause collisions
                string[] demoref = demofile.Map.Split('/');
                localname = demoref[1] + demoref[2];
            }

            //Go through the folder to check if the instance is still there
            foreach(string map in Directory.GetFiles(checkPath, "*.maprad"))
            {
                if(Path.GetFileNameWithoutExtension(map) == localname)
                {
                    Debug.Success("Found match on disk! {0}", map); //phew, no need to flood steam servers!

                    //Since it found a pre-downloaded instance it can just carry on with that instance + demo

                    doProcessing(serialwrite.Binary.ReadFromBinaryFile<mapData>(map), demofile);

                    //Exit the loop and method
                    Console.ReadLine();
                    return 0;
                }
            }

            //So the map wasn't found, therefore look for it on the workshop
            if (isWorkshop)
            {
                Debug.Info("Workshop map was not found on disk, downloading now...");
                WorkshopFile file = WorkshopFile.get(new WorkshopURI(demofile.Map)); //Workshop file

                Workshop.download(file, "maps/workshop/" + localname); //Download to disk
                bspinfo.UnpackBSP("maps/workshop/" + localname, "maps/workshop/" + localname);
                Debug.Success("Finished downloading from the workshop!");

                doProcessing(serialwrite.Binary.ReadFromBinaryFile<mapData>("maps/workshop" + localname + ".maprad"), demofile);

                Console.ReadLine();
                return 0;
            }
            else
            {
                // This bit will only occur if all three conditions are true:
                // The map has not been downloaded
                // The map is not on the workshop or is not public
                // The map is not official

                Debug.Error("There is no public known location of the map. Install the map first!");
            }

            Console.ReadLine();
            return 0;

            #region old
            /*shite

            string mapfile = "";
            string demofile = "";

            foreach (string str in args)
            {
                if(Path.GetExtension(str) == ".dem")
                {
                    demofile = str;
                    Debug.Success("Loaded demo file");
                }
                else if(Path.GetExtension(str) == ".bsp")
                {
                    mapfile = str;
                    Debug.Success("Loaded map file");
                }
            }

            if(mapfile == "" || demofile == "")
            {
                Debug.Error("Not enough sufficient files were found in order to run.");
            }

            mapdata map = new mapdata(mapfile);
            demodata demo = new demodata(demofile);

            string generated = Environment.CurrentDirectory + "/files/" + Path.GetFileNameWithoutExtension(mapfile) + "/";
            Debug.Log("Generate dir: {0}", generated);

            generateFlowMap(map, demo).Save(generated + "Flowmap.png");
            generateHeatMap(map, demo.shotPositions, generated + "Shots.png");
            generateHeatMap(map, demo.deathPositions, generated + "Deaths.png");
            generateHeatMap(map, demo.smokePositions, generated + "Smokes.png");
            generateHeatMap(map, demo.bombplantPositions, generated + "BombPlants.png");

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = generated,
                UseShellExecute = true,
                Verb = "open"
            });

            Console.ReadKey();
            return 0;
        }

        static Bitmap generateFlowMap(mapdata map, demodata demo)
        {
            Bitmap overlay = map.radarImage.ScaleImage(1024, 1024);
            camera cam = new camera();
            cam.offset = new vector2(map.radarDetails.pos_x, map.radarDetails.pos_y);
            cam.scale = map.radarDetails.scale;

            Pen pen = new Pen(Color.FromArgb(10, 7, 245, 255), 1.7f);

            using (var graphics = Graphics.FromImage(overlay))
            {
                int testx = 0;

                for (int b = 0; b < 2; b++)
                {
                    if (b == 1)
                        pen = new Pen(Color.FromArgb(15, 209, 120, 12), 1.7f);

                    foreach (List<List<vector3>> player in demo.positions[b].Values.ToList())
                    {
                        foreach (List<vector3> list in player)
                        {
                            for (int i = 0; i < list.Count(); i++)
                            {

                                vector2 pa = list[i].worldToScreenSpace(cam);
                                vector2 pb = list[(i - 1).Clamp(0, list.Count())].worldToScreenSpace(cam);

                                graphics.DrawLine(pen, pa.x, pa.y, pb.x, pb.y);
                            }
                        }

                        testx++;
                        overlay.Save(testx + ".png");
                    }
                }
            }

            return overlay;
        }

        static Bitmap generateHeatMap(mapdata map, List<vector3> data, string path)
        {
            //Bitmap background = map.radarImage.ScaleImage(1024, 1024);
            Bitmap overlay = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);

            using (var graphics = Graphics.FromImage(overlay))
            {
                graphics.Clear(Color.Transparent);
            }

            camera cam = new camera();
            cam.offset = new vector2(map.radarDetails.pos_x, map.radarDetails.pos_y);
            cam.scale = map.radarDetails.scale;

            Pen pen = new Pen(Color.FromArgb(32, 7, 245, 255), 2f);

            using (var graphics = Graphics.FromImage(overlay))
            {
                foreach(vector3 point in data)
                {
                    vector2 ssPoint = point.worldToScreenSpace(cam);

                    graphics.DrawRectangle(pen, new Rectangle((int)ssPoint.x - 1, (int)ssPoint.y - 1, 2, 2));
                }
            }

            using (var imageFactory = new ImageFactory())
            {
                string tempPath = Path.GetFullPath(path);
                imageFactory.Load(overlay).GaussianBlur(10).Save(tempPath);
                Debug.Log("Saved intensity map to {0}", tempPath);

                //ImageProcessor.Imaging.ImageLayer over = new ImageProcessor.Imaging.ImageLayer();
                //over.Image = new Bitmap(tempPath);

                //imageFactory.Load(background)
                //    .Overlay(over)
                //    .Save(Environment.CurrentDirectory + "/temp/test2.png");

                //Debug.Log("Saved final map to {0}", path);

            }

            return overlay;
            */
            #endregion
        }

        /// <summary>
        /// This method handles all the sequencing required to generate data from the mapData file
        /// </summary>
        /// <param name="mapfile">The mapfile to process</param>
        static void doProcessing(mapData mapfile, DemoParser parser)
        {
            Debug.Success("Starting processing");

            //First step: Make a grey version of the radar.

        }
    }
}













/*


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
using System.Threading;

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

        public static int remapClamped(this int val, int min1, int max1, int min2, int max2)
        {
            return val.remap(min1, max1, min2, max2).Clamp(min2, max2);
        }

        public static int Abs(this int val)
        {
            return Math.Abs(val);
        }

        public static Vector2 worldToImagePosition(this Vector3 val, Vector2 offset, float scale)
        {
            return new Vector2(Convert.ToInt32((val.x - offset.x) / scale), Convert.ToInt32((val.y - offset.y) / scale + 1024));
        }
    }

    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float X, float Y)
        {
            x = X;
            y = Y;
        }
    }

    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }

    public class gameTracker
    {
        public Dictionary<int, playerTracker> players = new Dictionary<int, playerTracker>();

        public void addRound()
        {
            foreach (playerTracker tracker in players.Values.ToList())
                tracker.positions.Add(new List<Vector3>());
        }
    }

    public class playerTracker
    {
        public List<List<Vector3>> positions = new List<List<Vector3>>();

        public playerTracker()
        {
            positions.Add(new List<Vector3>());
        }

        public void addPos(Vector3 pos)
        {
            positions.Last().Add(pos);
        }
    }

    class Program
    {

        static int Main(string[] args)
        {
            bool debug = true;

            string demofile = @"D:\Creative\Software\Mapcore\MapcoreServer\DemoHeatmap\DemoHeatmap\bin\Debug\aviv.dem";

            int ticks = 0;
            if(debug)
            {
                DemoParser par = new DemoParser(File.OpenRead(demofile));
                par.ParseHeader();
                par.TickDone += (object sender, TickDoneEventArgs e) => { ticks++; };
                try
                {
                    par.ParseToEnd();
                }
                catch
                {
                    Debug.Warn("Attempted read past stream...");
                }
                GC.Collect();
            }

            DemoParser parser = new DemoParser(File.OpenRead(demofile));

            gameTracker tracker = new gameTracker();

            parser.RoundStart += (object sender, RoundStartedEventArgs e) =>
            {
                tracker.addRound();
                Debug.Info("New round!");
            };

            parser.ParseHeader();


            Debug.progressBar("Parsing", 0);

            int tickC = 0;

            try
            {
                while (parser.ParseNextTick() != false)
                {
                    if(tickC % 500 == 0)
                        Debug.updateProgressBar(Convert.ToInt32(((float)tickC / (float)ticks) * 100));
                    tickC++;

                    foreach (DemoInfo.Player player in parser.PlayingParticipants)
                    {
                        if (!tracker.players.ContainsKey(player.EntityID))
                        {
                            Debug.Success("New player found! {0}", player.EntityID);
                            tracker.players.Add(player.EntityID, new playerTracker());
                        }

                        if(player.IsAlive)
                            tracker.players[player.EntityID].addPos(new Vector3(player.Position.X, player.Position.Y, player.Position.Z));
                    }
                }
            }
            catch
            {
                Debug.exitProgressBar();
                Debug.Warn("Attempted to read past end of stream...");
            }
            Debug.exitProgressBar();

            Debug.Success("Finished reading demo file");




            Bitmap overlay = new Bitmap(1024, 1024);


            Pen pen = new Pen(Color.FromArgb(16, Color.Red), 1);

            using (var graphics = Graphics.FromImage(overlay))
            {
                foreach(playerTracker player in tracker.players.Values.ToList())
                {
                    Debug.Info("Processing new player");
                    foreach(List<Vector3> list in player.positions)
                    {
                        for(int i = 0; i < list.Count(); i++)
                        {
                            Vector2 pa = list[i].worldToImagePosition(new Vector2(-3517, 2546), 5);
                            Vector2 pb = list[(i - 1).Clamp(0, list.Count())].worldToImagePosition(new Vector2(-3517, 2546), 5);

                            graphics.DrawLine(pen, pa.x, pa.y, pb.x, pb.y);
                        }
                    }
                }
            }

            overlay.Save("D:/wow.png");



                Console.ReadLine();

            return 0;
        }
    }
}


*/
