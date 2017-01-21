using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using SuperSimpleHTTP.requests;
using System.IO.Compression;
using System.Diagnostics;
using DemoHeatmap.IO;


namespace DemoHeatmap.steam
{
    public class Tag
    {
        public string tag { get; set; }
    }

    public class Publishedfiledetail
    {
        public string publishedfileid { get; set; }
        public int result { get; set; }
        public string creator { get; set; }
        public int creator_app_id { get; set; }
        public int consumer_app_id { get; set; }
        public string filename { get; set; }
        public int file_size { get; set; }
        public string file_url { get; set; }
        public string hcontent_file { get; set; }
        public string preview_url { get; set; }
        public string hcontent_preview { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int time_created { get; set; }
        public int time_updated { get; set; }
        public int visibility { get; set; }
        public int banned { get; set; }
        public string ban_reason { get; set; }
        public int subscriptions { get; set; }
        public int favorited { get; set; }
        public int lifetime_subscriptions { get; set; }
        public int lifetime_favorited { get; set; }
        public int views { get; set; }
        public List<Tag> tags { get; set; }
    }

    public class WorkshopFileResponse
    {
        public int result { get; set; }
        public int resultcount { get; set; }
        public List<Publishedfiledetail> publishedfiledetails { get; set; }
    }

    public class WorkshopFile
    {
        public WorkshopFileResponse response { get; set; }

        /// <summary>
        /// Gets a workshop file using a workshopURI object
        /// </summary>
        /// <param name="workshopURI">Workshop URI needed to get the workshop data</param>
        /// <returns>A workshop file with correct data</returns>
        public static WorkshopFile get(WorkshopURI workshopURI)
        {
            //Base method
            string addr = "http://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/V0001/";

            //Dictionary of method parameters
            Dictionary<string, string> options = new Dictionary<string, string>();
            options.Add("itemcount", "1");
            options.Add("publishedfileids[0]", workshopURI.id);

            //The webrequest
            string web = request.POST(addr, options);

            //Return the workshop file
            WorkshopFile final = new WorkshopFile();
            final.response = JsonConvert.DeserializeObject<WorkshopFile>(web).response;
            return final; //Done
        }
    }

    public struct LocalFileData
    {
        string radarfileDDS;
        string radarfilePNG;
        string bspfile;
    }

    public class Workshop
    {
        /// <summary>
        /// Downloads the zip file of the map from the workshop
        /// </summary>
        /// <param name="self">The WorkshopFile object</param>
        /// <param name="path">Path to download to, ".zip" file</param>
        public static void download(WorkshopFile self, string outputFolder)
        {
            //Make sure its actually a folder, not a file
            if (Path.HasExtension(outputFolder))
                throw new ArgumentException("pathname must reference a folder", "outputFolder");
            else
            {
                outputFolder += "/";//make sure it will place stuff in the folder, not an instance of its name + var

                //Create the directory if it doesn't exist
                if (!Directory.Exists(outputFolder))
                    Directory.CreateDirectory(outputFolder);

                //Download the file from the workshop
                using (var client = new WebClient())
                {
                    client.DownloadFile(self.response.publishedfiledetails[0].file_url, outputFolder + "download.zip");
                }

                //Extract the zip
                using (var zip = ZipFile.OpenRead(outputFolder + "download.zip"))
                {
                    foreach (var entry in zip.Entries.ToList())
                    {
                        entry.ExtractToFile(outputFolder + entry.FullName, true); //Extract the file to its download folder
                    }
                }
            }
        }
    }

    public class WorkshopURI
    {
        public string fullURL { get; set; }
        public string id { get; set; }

        /// <summary>
        /// Workshop URI class, holds url and id of that file
        /// <para/>
        /// Can be in input formats of:
        /// <para/>
        /// ID, URL, FOLDER
        /// </summary>
        /// <param name="input">ID/URL/FOLDER</param>
        public WorkshopURI(string input)
        {
            //Check whether the input was jsut an ID
            try
            {
                Convert.ToInt32(input);
                id = input;
                fullURL = "https://steamcommunity.com/sharedfiles/filedetails/?id=" + id;
            }
            //Or if it's a workshop file / folder reference
            catch
            {
                //If it's a workshop file
                if(input.Contains("?id="))
                {
                    fullURL = input;
                    string segment = input.Split(new string[] { "?id=" }, StringSplitOptions.None)[1];

                    foreach(char c in segment)
                    {
                        try
                        {
                            Convert.ToInt32(c);
                            id += c;
                        }
                        catch { break; }
                    }
                }
                //If its a folder reference, from a demo, for example.
                else if(input.Contains("workshop"))
                {
                    string segment = input.Split(new string[] { "workshop" }, StringSplitOptions.None)[1];

                    foreach(char c in segment.Substring(1)) //Need to exclude that pesky slash
                    {
                        if(c != '/' && c != '\\')
                        {
                            id += c;
                        }
                        else
                        {
                            break;
                        }
                    }

                    fullURL = "https://steamcommunity.com/sharedfiles/filedetails/?id=" + id;
                }
                //Error if else/
                else
                {
                    throw new ArgumentException("Could not resolve workshop file", "input");
                }
            }
        }
    }
}
