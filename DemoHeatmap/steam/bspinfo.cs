﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
using Imaging.DDSReader;
using System.Drawing;
using DemoHeatmap.IO;

namespace DemoHeatmap.steam
{
    [Serializable]
    public struct mapData
    {
        public Radar radar;
        public Bitmap image_radar;
        public Bitmap image_radar_spectate;
    }

    class bspinfo
    {
        public struct paths
        {
            public string path_radar;
            public string path_radar_spectate;
            public string path_radar_txt;     
        }

        public static mapData UnpackBSP(string sourceFolder, string outputPath)
        {
            //Find the bsp file
            string[] found = Directory.GetFiles(sourceFolder, "*.bsp");
            if (found.Count() == 0)
            {
                throw new ArgumentException("Could not find BSP file in folder", "outputFolder");
            }
            else
            {
                string bsp = Directory.GetFiles(sourceFolder, "*.bsp")[0]; //Find the bsp file

                //Setup extraction arguments
                string extractarg = "-extract \"" + Path.GetFullPath(bsp) + "\" \"" + sourceFolder + "/extracted" + "\"";

                //Run the process
                Process bspzip = new Process();
                bspzip.StartInfo = new ProcessStartInfo
                {
                    FileName = Environment.CurrentDirectory + "/bspzip.exe",
                    Arguments = extractarg,
                    UseShellExecute = false,
                };

                bspzip.Start();
                bspzip.WaitForExit();

                paths overview = new paths();

                //Extract the zip file's overview components
                using (var zip = ZipFile.OpenRead(sourceFolder + "/extracted.zip"))
                {
                    foreach (var entry in zip.Entries.ToList())
                    {
                        if (entry.FullName.ToLower().Contains("overviews"))
                        {
                            //Setup target dir
                            string targetdir = sourceFolder + "/files/" + Path.GetDirectoryName(entry.FullName);
                            Directory.CreateDirectory(targetdir);

                            //Extract to target dir
                            string targetfile = sourceFolder + "/files/" + entry.FullName;
                            entry.ExtractToFile(targetfile, true);
                            Debug.Success("Extracted " + entry.FullName);


                            //File name sorting for the paths struct
                            string purefilename = Path.GetFileName(entry.FullName);
                            if (purefilename.Contains("radar_spectate"))
                                overview.path_radar_spectate = targetfile;

                            else if (purefilename.Contains("_radar"))
                                overview.path_radar = targetfile;

                            else if (purefilename.Contains(".txt"))
                                overview.path_radar_txt = targetfile;
                        }
                    }
                }

                mapData toSave = new mapData();

                if(overview.path_radar != null)
                    toSave.image_radar = DDS.LoadImage(overview.path_radar, false);

                if(overview.path_radar_spectate != null)
                    toSave.image_radar_spectate = DDS.LoadImage(overview.path_radar_spectate, false);

                if(overview.path_radar_txt != null)
                    toSave.radar = new Radar(overview.path_radar_txt);

                serialwrite.Binary.WriteToBinaryFile<mapData>(Path.ChangeExtension(outputPath, ".maprad"), toSave);

                return toSave;
            }
        }
    }
}
