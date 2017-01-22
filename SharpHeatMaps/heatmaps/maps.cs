using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpHeatMaps.math;
using System.Drawing;
using SharpHeatMaps.gradients;
using System.Collections.Concurrent;

namespace SharpHeatMaps.heatmaps
{

    public class densitymap
    {
        public int width;
        public int height;

        public int[] pixelDensities;

        public densitymap(int w = 1024, int h = 1024)
        {
            width = w;
            height = h;

            //Setup new array
            pixelDensities = new int[w * h];

            //Clear each pixel to black
            for (int i = 0; i < w * h; i++)
                pixelDensities[i] = 0;
        }

        public void setPixelDensity(int x, int y, int density)
        {
            //Find the pixel at the right location   //Inverts
            _doSetPixelDensity(x, y, density, pixelDensities);
        }

        private void _doSetPixelDensity(int x, int y, int density, int[] array)
        {
            array[(width * (height - y - 1)) + x] = density;
        }

        public void addToPixelDensity(int x, int y, int density)
        {
            //Set the pixel density at the right location
            pixelDensities[(width * (height - y - 1)) + x] += density;
        }

        public void createHeatMapSplodge(int x, int y, int radius = 20)
        {
            //Iterate over Y axis, negative RAD to + RAD
            for (int yy = -radius; yy < radius; yy++)
            {
                //Iterate over X axis, negative RAD to + RAD
                for (int xx = -radius; xx < radius; xx++)
                {
                    //Chopping the value to avoid edge build up
                    if (x + xx < 1024 && y + yy < 1024 && x + xx > 0 && y + yy > 0)
                    {
                        //Add density to that pixel
                        addToPixelDensity((x + xx).Clamp(0, 1023), (y + yy).Clamp(0, 1023),
                            (
                            (radius - xx.Abs()) *
                            (radius - yy.Abs()) -
                            radius
                            ) * 2);
                    }
                }
            }
        }

        public int getPixelValue(int x, int y)
        {
            return pixelDensities[(width * (height - y - 1)) + x];
        }

        public void averageBlur3x3()
        {
            //Setup a new pixel array
            int[] newPixels = new int[width * height];

            Parallel.ForEach(Partitioner.Create(0, height), (range) =>

             {
                 for (int y = range.Item1; y < range.Item2; y++)
                 {

                     for (int x = 0; x < width; x++)
                     {

                         //total value
                         int total = 0;
                         int c = 0;

                         int kS = 3;

                         //Get pixels that surround the working pixel
                         for (int yy = -kS; yy < kS; yy++)
                             for (int xx = -kS; xx < kS; xx++)
                             {
                                 total += getPixelValue((x + xx).Clamp(0, width - 1), (y + yy).Clamp(0, height - 1));
                                 c++;
                             }

                         //Set the pixel

                         _doSetPixelDensity(x, y, total / c, newPixels);
                     }
                 }
             });

            //Edit the images pixels
            pixelDensities = newPixels;
        }


        //Method that normalises everything
        public void normalise(float midpoint = 0.5f, int max = 255)
        {
            int highest = 0;
            foreach (int pix in pixelDensities)
                if (pix > highest)
                    highest = pix;

            for(int i = 0; i < pixelDensities.Length; i++)
            {   
                //sets the pixel    Original pixel    remap from v (0 to M*2?TOTAL * HIGH) to    v (0 to df=255)
                pixelDensities[i] = pixelDensities[i].remapClamped(0, (int)((midpoint * 2) * highest), 0, max);
            }
        }

        //Returns itself in bitmap form. All channels equals density map
        public Bitmap toBitMap()
        {
            Bitmap output = new Bitmap(width, height);

            imgLowLvl.imagebits bits = imgLowLvl.fastbits(output);

            for (int i = 0; i < bits.a.Length; i++)
            {
                bits.a[i] = 255;
                bits.r[i] = (byte)pixelDensities[i];
                bits.g[i] = (byte)pixelDensities[i];
                bits.b[i] = (byte)pixelDensities[i];
            }

            return imgLowLvl.imageBitsToBitMap(bits, width, height);
        }
    }

    class heatmap
    {

    }
}
