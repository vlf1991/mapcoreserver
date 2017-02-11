using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSimpleHTTP.requests;

namespace DemoHeatmapGUI
{
    public static class version
    {
        //The version of this current build
        public static string runningVersion = "1.0.0.0";

        //Downloads the string of the latest version from a weburl
        public static string getLatestVersion()
        {
            return request.downloadString(@"https://dl.dropboxusercontent.com/u/187567742/DemoHeatmap/currentversion.txt").Split('!')[0];
        }

        public static string getDownloadURL()
        {
            return request.downloadString(@"https://dl.dropboxusercontent.com/u/187567742/DemoHeatmap/currentversion.txt").Split('!')[1];
        }

        //Small boolean switch to check if the program is running on the most recent version
        public static bool isLatestVersion()
        {
            if (getLatestVersion() == runningVersion)
                return true;
            else
                return false;
        }
    }
}
