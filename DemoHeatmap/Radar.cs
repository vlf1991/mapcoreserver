using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DemoHeatmap
{
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
                        if(c != '\"')
                        {
                            blocks[count / 2] = blocks[count / 2] + c;
                        }
                    }
                }

                all.Add(blocks);

            }

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
        }
    }
}
