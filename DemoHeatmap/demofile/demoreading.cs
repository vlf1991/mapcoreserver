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
using DemoHeatmap.steam;
using DemoHeatmap.IO;
using SharpHeatMaps.heatmaps;
using SharpHeatMaps.gradients;
using SharpHeatMaps.imaging;

namespace DemoHeatmap.demofile
{
    public class demoreading
    {
        public struct mapstatus
        {
            public bool isDownloaded;
            public bool isWorkshop;
            public DemoParser activeParser;
        }

        public static mapstatus isDownloaded(string demoPath)
        {
            //Make our return var
            mapstatus stat = new mapstatus();

            //Do parsing
            DemoParser demofile = new DemoParser(File.OpenRead(demoPath));
            demofile.ParseHeader();

            //TRY AND FIND INSTANCE OF MAP ON DISK
            Debug.Info("Scanning for map on disk");

            //Do workshop check
            stat.isWorkshop = false;
            string checkPath = "maps/";
            string localname = demofile.Map;

            if (demofile.Map.StartsWith("workshop")) //Detected its a workshop map
            {
                stat.isWorkshop = true;

                checkPath = "maps/workshop/"; //Switch to the workshopped folder
                Debug.Log("Map workshopped");

                //Sets the local name to something that will not cause collisions
                string[] demoref = demofile.Map.Split('/');
                localname = demoref[1] + demoref[2];
            }

            //Go through the folder to check if the instance is still there
            foreach (string map in Directory.GetFiles(checkPath, "*.maprad"))
            {
                if (Path.GetFileNameWithoutExtension(map) == localname)
                {
                    Debug.Success("Found match on disk! {0}", map); //phew, no need to flood steam servers!
                    stat.isDownloaded = true;
                    //Since it found a pre-downloaded instance it can just carry on with that instance + demo
                }
            }

            stat.activeParser = demofile;
            return stat;
        }

        public static void installMap(mapstatus dlStatus)
        {            

        }
    }
}
