using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace DemoHeatmap.steam
{
    [Serializable]
    public class Radar
    {
        public int pos_x;
        public int pos_y;
        public float scale;
        public string matpath;

        public Radar(string filepath)
        {
            //Open a stream of the radar txt
            StreamReader s = new StreamReader(filepath);

            string line;
            List<List<string>> all = new List<List<string>>();

            //reads line by line
            while ((line = s.ReadLine()) != null)
            {
                List<string> blocks = new List<string>();

                //Starts reading blocks
                int count = 0;
                foreach (char c in line)
                {
                    //Finds an open block tag
                    if (c == '\"')
                    {
                        count++;
                        
                        //Finds close tag
                        if (count % 2 == 1)
                            blocks.Add("");
                    }

                    //Finds double tag
                    if ((count % 2 == 1) && (count > 0))
                    {
                        if(c != '\"')
                        {
                            //Adds it
                            blocks[count / 2] = blocks[count / 2] + c;
                        }
                    }
                }
                //Prepares for filtering 
                all.Add(blocks);

            }
            
            //Iterates over and filters each block inside the text file
            foreach(List<string> param in all)
            {
                if(param.Count == 2)
                {
                    if (param[0] == "pos_x")
                        pos_x = Convert.ToInt32(param[1]);
                    if(param[0] == "pos_y")
                        pos_y = Convert.ToInt32(param[1]);
                    if (param[0] == "scale")
                        scale = (float)Convert.ToDouble(param[1]);
                    if (param[0] == "material")
                        matpath = param[1];
                }
            }
            s.Close();
            //Closes and finished
        }
    }
}
