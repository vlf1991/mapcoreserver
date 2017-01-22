using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpHeatMaps.heatmaps;
using SharpHeatMaps.math;
using SharpHeatMaps.gradients;
using SharpHeatMaps.imaging;
using System.Drawing;

namespace SharpHeatMaps
{
    class Program
    {
        static void Main(string[] args)
        {


            Bitmap inputImage = new Bitmap("1024cat.png");
            Bitmap overlayImg = new Bitmap("test2.png");
            bitmapfilters.brightness(inputImage, 0.4f).Save("brightness.png");


            return;

            //How to use the heatmaps example

            densitymap denstest = new densitymap(); //setup a new density map
            Random r = new Random();
            for (int i = 0; i < (1024*2); i++)
            {
                int rposx = r.Next(0, 1024);
                int rposy = r.Next(0, 1024);

                denstest.createHeatMapSplodge(rposx, rposy); //Add all your data, in this case it was random
            }

            denstest.averageBlur3x3(); //Add a bit of kernal blur
            denstest.normalise(); //Normalise the results so they fit between 0-255 (mid point can be adjusted for more interesting effects)


            gradients.gradients.fire.applyToImage(denstest.toBitMap()).Save("test2.png"); //Apply a gradient to it, in this case I added the fire gradient which works well
        }
    }
}
