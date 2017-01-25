using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.IO;

namespace SuperSimpleHTTP.requests
{
    public class request
    {
        private static async Task<string> do_POST(string url, Dictionary<string, string> data)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(data);

            var response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Simple POST request using HTTP
        /// </summary>
        /// <param name="url">Method URI to use</param>
        /// <param name="data">Dictionary object that contains parameters for POST method</param>
        /// <returns>HTTP POST response</returns>
        public static string POST(string url, Dictionary<string, string> data)
        {
            return do_POST(url, data).Result;
        }





        private static async Task<string> do_GET(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Simple GET request using HTTP
        /// </summary>
        /// <param name="url">URL of the method in use</param>
        /// <returns>HTTP GET response</returns>
        public static string GET(string url)
        {
            return do_GET(url).Result;
        }
    }
}
