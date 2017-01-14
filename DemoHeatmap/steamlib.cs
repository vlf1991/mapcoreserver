using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace DemoHeatmap
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

    public class WorkshopResponse
    {
        public int result { get; set; }
        public int resultcount { get; set; }
        public List<Publishedfiledetail> publishedfiledetails { get; set; }
    }

    public class WorkshopRootObject
    {
        public WorkshopResponse response { get; set; }
    }

    public static class steamlib
    {
        public static string apikey = "A5E2B82EE446010609CA5DB1C9898CC1";

        public static string urlToID(string url)
        {
            string[] scan1 = url.Split(new string[] { "?id=" }, StringSplitOptions.None);
            string final = "";
            foreach (char character in scan1[1])
            {
                try
                {
                    final += Convert.ToInt32(character).ToString();
                }
                catch
                {
                    break;
                }
            }

            return final;
        }
    }

    public class fileData
    {
        
    }

    public class workshop
    {
        WorkshopRootObject data;

        workshop(string url)
        {
            WebClient web = new WebClient();
            string addr = "https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/?key=" + steamlib.apikey;
            addr += "?itemcount=1";
            addr += "?publishedfileids[0]=" + steamlib.urlToID(url);

            string result = web.DownloadString(addr);

            data = JsonConvert.DeserializeObject<WorkshopRootObject>(result);
        }
    }
}
