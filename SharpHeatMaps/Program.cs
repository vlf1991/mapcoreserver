using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpHeatMaps.heatmaps;
using SharpHeatMaps.math;

namespace SharpHeatMaps
{
    class Program
    {
        static void Main(string[] args)
        {
            densitymap denstest = new densitymap();
            Random r = new Random();
            for (int i = 0; i < (1024*2); i++)
            {
                int rposx = r.Next(0, 1024);
                int rposy = r.Next(0, 1024);

                int sZ = 20;

                for (int yy = -sZ; yy < sZ; yy++)
                {
                    for (int xx = -sZ; xx < sZ; xx++)
                    {
                        if (rposx + xx < 1024 && rposy + yy < 1024 && rposx + xx > 0 && rposy + yy > 0)
                        {
                            denstest.addToPixelDensity((rposx + xx).Clamp(0, 1023), (rposy + yy).Clamp(0, 1023),
                                (
                                (sZ - xx.Abs()) *
                                (sZ - yy.Abs()) -
                                sZ
                                ) * 2);
                        }
                    }
                }
            }

            Console.WriteLine("Finished random seeding");

            denstest.averageBlur3x3();
            denstest.normalise();

            denstest.toBitMap().Save("test.png");
        }
    }
}
