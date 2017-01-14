using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Drawing;
using Imaging.DDSReader;
using ImageProcessor;

namespace DemoHeatmap.demofile
{
    class mapdata
    {
        public Bitmap radarImage;
        public Radar radarDetails;

        public mapdata(string path)
        {

            string mapname = Path.GetFileNameWithoutExtension(path);
            string serverfolder = "files/" + mapname;
            string extractzip = Path.GetFullPath(serverfolder) + "/" + mapname + ".zip";

            string extractarg = "-extract \"" + Path.GetFullPath(path) + "\" \"" + extractzip + "\"";

            #region servercheck

            if (Directory.Exists(serverfolder))
                Debug.Warn("Working directory already exists");


            Debug.Info("Adding files");
            Directory.CreateDirectory(serverfolder);
            File.Copy(path, serverfolder + "/" + Path.GetFileName(path), true);

            path = serverfolder + "/" + Path.GetFileName(path);

            #endregion servercheck

            Process bspzip = new Process();
            bspzip.StartInfo = new ProcessStartInfo
            {
                FileName = Environment.CurrentDirectory + "/bspzip.exe",
                Arguments = extractarg,
                UseShellExecute = false,
            };

            bspzip.Start();
            bspzip.WaitForExit();

            using (var zip = ZipFile.OpenRead(extractzip))
            {
                foreach (var entry in zip.Entries.ToList())
                {
                    if (entry.FullName.ToLower().Contains("overviews"))
                    {
                        Directory.CreateDirectory(Path.GetFullPath(serverfolder) + "/" + Path.GetDirectoryName(entry.FullName));

                        entry.ExtractToFile(Path.GetFullPath(serverfolder) + "/" + entry.FullName, true);
                        Debug.Success("Extracted " + entry.FullName);
                    }
                }
            }

            Radar radar = new Radar(serverfolder + "/resource/overviews/" + mapname + ".txt");
            radarDetails = radar;
            Debug.Success("Radar file parsed");

            string ddsRadar = Directory.GetFiles(serverfolder + "/resource/overviews/", "*.dds")[0];
            Bitmap convert = DDS.LoadImage(ddsRadar, false);

            string imgLoc = serverfolder + "/resource/overviews/" + Path.GetFileNameWithoutExtension(ddsRadar) + ".png";

            convert.Save(imgLoc);

            using (var imageFactory = new ImageFactory())
            {
                imageFactory.Load(imgLoc)
                    .Saturation(-100)
                    .Contrast(-60)
                    .Brightness(-35)
                    .Contrast(-20)
                    .Brightness(-20)
                    .Save(imgLoc);
            }

            radarImage = new Bitmap(imgLoc);

            Debug.Success("Radar file exported");
        }
    }

    class Radar
    {
        public int pos_x;
        public int pos_y;
        public float scale;
        public string matpath;

        public Radar(string filepath)
        {
            StreamReader s = new StreamReader(filepath);

            string line;


            List<List<string>> all = new List<List<string>>();

            while ((line = s.ReadLine()) != null)
            {
                List<string> blocks = new List<string>();


                int count = 0;
                foreach (char c in line)
                {
                    if (c == '\"')
                    {
                        count++;

                        if (count % 2 == 1)
                            blocks.Add("");
                    }

                    if ((count % 2 == 1) && (count > 0))
                    {
                        if (c != '\"')
                        {
                            blocks[count / 2] = blocks[count / 2] + c;
                        }
                    }
                }

                all.Add(blocks);

            }

            foreach (List<string> param in all)
            {
                if (param.Count == 2)
                {
                    if (param[0] == "pos_x")
                        pos_x = Convert.ToInt32(param[1]);
                    if (param[0] == "pos_y")
                        pos_y = Convert.ToInt32(param[1]);
                    if (param[0] == "scale")
                        scale = (float)Convert.ToDouble(param[1]);
                    if (param[0] == "material")
                        matpath = param[1];
                }
            }
            s.Close();
        }
    }
}