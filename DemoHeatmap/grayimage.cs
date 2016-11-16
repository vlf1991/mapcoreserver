using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DemoHeatmap
{
    class grayimage
    {
        List<List<int>> pixels = new List<List<int>>();
        public readonly int bitdepth;

        public readonly int Width;
        public readonly int Height;

        public grayimage(int width, int height, int depth = 255)
        {
            bitdepth = depth;
            Width = width;
            Height = height;

            for(int y = 0; y < height; y++)
            {
                pixels.Add(new List<int>());

                for (int x = 0; x < width; x++)
                {
                    pixels[y].Add(0);
                }
            }
        }

        /// <summary>
        /// Sets the int at the selected pixel
        /// </summary>
        /// <param name="x">Horizontal position</param>
        /// <param name="y">Verticle position</param>
        public void SetPixel(int x, int y, int value)
        {
            pixels[y][x] = value;
        }

        public grayimage KernalBlur(int iterations, int radius, bool normalize = false)
        {
            Console.WriteLine("Blurring using {0} iterations, kernal search radious of {1} , normalizing is set to {2}.", iterations, radius, normalize);

            grayimage final = this;

            for (int i = 0; i < iterations; i++)
            {
                grayimage worker = new grayimage(Width, Height, bitdepth);

                for (int y = 0; y < Height; y++) {

                    for (int x = 0; x < Width; x++)
                    {
                        /*      X-1     X ~     X+1
                         *      Y-1     Y-1     Y-1
                         *      
                         *      X-1     X ~     X+1
                         *      Y ~     Y ~     Y ~
                         *      
                         *      X-1     X ~     X+1
                         *      Y+1     Y+1     Y+1
                         */

                        int total = 0;
                        int ccount = 0;

                        //Total up all kernal pixels
                        for (int yy = -radius; yy <= radius; yy++)
                            for (int xx = -radius; xx <= radius; xx++)
                            {
                                total += final.pixels[(y + yy).Clamp(0, Height - 1)][(x + xx).Clamp(0, Width - 1)] * ((xx.Abs() + yy.Abs()) / radius + radius);

                                ccount++;
                            }

                        //if(total > 0)
                        //Console.Write(total + " ");

                        //Get average
                        int average = total;

                        worker.SetPixel(x, y, average);
                    }
                }

                final = worker.normalize(worker);

                Console.Write(".");
            }

            Console.WriteLine("Kernal Convolution Complete");

            return final;
        }

        public grayimage KernalFastBlur(int iterations, int radius, bool normalize = false)
        {
            Console.WriteLine("Blurring using {0} iterations, kernal search radious of {1} , normalizing is set to {2}.", iterations, radius, normalize);

            grayimage final = this;

            List<int> lArray = new List<int>();
            foreach(List<int> l in final.pixels)
            {
                foreach(int In in l)
                {
                    lArray.Add(In);
                }
            }

            for (int i = 0; i < iterations; i++)
            {
                grayimage worker = new grayimage(Width, Height, bitdepth);

                for (int y = 0; y < Height; y++)
                {

                    for (int x = 0; x < Width; x++)
                    {
                        /*      X-1     X ~     X+1
                         *      Y-1     Y-1     Y-1
                         *      
                         *      X-1     X ~     X+1
                         *      Y ~     Y ~     Y ~
                         *      
                         *      X-1     X ~     X+1
                         *      Y+1     Y+1     Y+1
                         */

                        int total = 0;
                        int ccount = 0;

                        //Total up all kernal pixels
                        for (int yy = -radius; yy <= radius; yy++)
                            for (int xx = -radius; xx <= radius; xx++)
                            {
                                total += final.pixels[(y + yy).Clamp(0, Height - 1)][(x + xx).Clamp(0, Width - 1)] * ((xx.Abs() + yy.Abs()) / radius + radius);

                                ccount++;
                            }

                        //if(total > 0)
                        //Console.Write(total + " ");

                        //Get average
                        int average = total;

                        worker.SetPixel(x, y, average);
                    }
                }

                final = worker.normalize(worker);

                Console.Write(".");
            }

            Console.WriteLine("Kernal Convolution Complete");

            return final;
        }

        public Bitmap toBitmap()
        {
            Bitmap basemap = new Bitmap(Width, Height);

            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    int value = pixels[y][x].Clamp(0, 255);
                    basemap.SetPixel(x, y, Color.FromArgb(value, value, value));
                }
            }

            return basemap;
        }

        private grayimage normalize(grayimage self)
        {
            int high = 40;
            foreach (List<int> ly in self.pixels)
                foreach (int lx in ly)
                    if (lx > high)
                        high = lx;

            grayimage newimg = new grayimage(Width, Height);

            for(int y = 0; y < Height; y++){ for(int x = 0; x < Height; x++)
                { newimg.SetPixel(x, y, self.pixels[y][x].remap(0,high,0,bitdepth)); } }

            return newimg;
        }
    }
}
