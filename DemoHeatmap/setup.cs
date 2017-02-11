using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace DemoHeatmap
{
    public static class setup
    {
        public static void runFirstTimeSetup()
        {
            List<string> dirsToMake = new List<string>();
            dirsToMake.Add("demos");
            dirsToMake.Add("maps");
            dirsToMake.Add("maps/workshop");
            dirsToMake.Add("bin");

            foreach (string dir in dirsToMake)
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }

            foreach (string dir in Directory.GetFiles("valve"))
            {
                if (!File.Exists(Path.GetDirectoryName(dir) + "/../" + Path.GetFileName(dir)))
                    File.Copy(dir, Path.GetDirectoryName(dir) + "/../" + Path.GetFileName(dir));
            }
            foreach (string dir in Directory.GetFiles("valve/bin"))
                if (!File.Exists(Path.GetDirectoryName(dir) + "/../../bin/" + Path.GetFileName(dir)))
                    File.Copy(dir, Path.GetDirectoryName(dir) + "/../../bin/" + Path.GetFileName(dir));
        }
    }
}
