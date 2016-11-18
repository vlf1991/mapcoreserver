﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;

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



        /*



                                SOME VERY WIP PROCESSING FUNCTIONS



       */





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








        static grayimage workerimage;
        static bool inuse = false;

        static int threadsActive;

        static void ProcessChunk(Vector2 chunksize, Vector2 position, int radius, grayimage source)
        {

            //Debug.Core("Processign chunk at {0},{1} with size {2}, {3}", position.x, position.y, chunksize.x, chunksize.y);
            //Debug.Core("Processing chunk");

            grayimage subworker = new grayimage((int)chunksize.x, (int)chunksize.y, source.bitdepth);

            for (int y = (int)position.y; y < (int)position.y + (int)chunksize.y; y++)
            //for (int y = 0; y < 1024; y++)
            {
                //Debug.Core("Y:" + y);
                for (int x = (int)position.x; x < (int)position.x + (int)chunksize.x; x++)
                //for (int x = 0; x < 1024; x++)
                {
                    //Debug.Core("" + x);

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
                            //total += source.pixels[(y + yy).Clamp(0, source.Height - 1)][(x + xx).Clamp(0, source.Width - 1)] * ((xx.Abs() + yy.Abs()) / radius + radius);

                            total += source.pixels[(y + yy).Clamp(0, source.Height - 1)][(x + xx).Clamp(0, source.Width - 1)] * (radius * 2 - (xx.Abs() + yy.Abs()));

                            ccount++;
                        }

                    //if(total > 0)
                    //Console.Write(total + " ");

                    //Get average
                    int average = total / ccount;

                    //lock (workerimage)
                    //{
                    //workerimage.SetPixel(x, y, average);
                    subworker.SetPixel(x - (int)position.x, y - (int)position.y, average);
                    //}
                    //Debug.Core(source.pixels[x][y].ToString());
                }


            }

            Thread.Sleep(10);

            while (inuse) { /*Just waiting to deposit stuff */ Debug.Core("Deposit wating"); Thread.Sleep(1000); }

            inuse = true;

            for(int xx = 0; xx < subworker.Width; xx++)
            {
                for(int yy = 0; yy < subworker.Height; yy++)
                {
                    workerimage.SetPixel(xx + (int)position.x, yy + (int)position.y, subworker.pixels[yy][xx]);
                }
            }

            inuse = false;

            //Debug.Core("Chunk complete!");

            threadsActive -= 1;
        }

        public grayimage KernalBlurMultithread(int radius, int chunksize = 256, bool normalize = false)
        {
            Debug.Log("Kernal Blurring using multithreading");
            workerimage = new grayimage(Width, Height, bitdepth);

            grayimage final = this;



            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    while (threadsActive > 4) { }

                    ThreadPool.QueueUserWorkItem(o => ProcessChunk(new Vector2(chunksize, chunksize), new Vector2(x * chunksize, y * chunksize), radius, final));
                    threadsActive++;

                    Thread.Sleep(10);

                    Console.Write("\rCurrent Active Threads {0}     ", threadsActive.ToString());
                }
            }

            Console.Write("\n");

            Debug.Success("COMPLETE");
            
            return workerimage;


            /*for (int i = 0; i < 500; i++)
            {
                ThreadPool.QueueUserWorkItem(o => Thread1(i));
                Debug.Log("STARTED THREAD {0}.", i);
            };*/
        }



    }
}
