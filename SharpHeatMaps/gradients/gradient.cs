using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using SharpHeatMaps.math;

namespace SharpHeatMaps.gradients
{
    public static class gradients
    {
        public static image_gradient blue_green_red = new image_gradient("gradients/blue-green-red.png");
        public static image_gradient purple_red_yellow = new image_gradient("gradients/purple-red-yellow.png");
    }

    public class image_gradient
    {
        imgLowLvl.imagebits gradientBits;

        /// <summary>
        /// Loads an image gradient from disk
        /// </summary>
        /// <param name="path">The path to load image gradient from</param>
        public image_gradient(string path)
        {
            Bitmap source = new Bitmap(path);
            gradientBits = imgLowLvl.fastbits(source);
        }

        public Bitmap applyToImage(Bitmap image)
        {
            imgLowLvl.imagebits allbits = imgLowLvl.fastbits(image);

            int gradLen = gradientBits.a.Length;

            for(int i = 0; i < allbits.r.Length; i++)
            {
                int average = (allbits.r[i] + allbits.g[i] + allbits.b[i]) / 3;
                allbits.r[i] = gradientBits.r[average.remap(0, 255, 0, gradLen)];
                allbits.g[i] = gradientBits.g[average.remap(0, 255, 0, gradLen)];
                allbits.b[i] = gradientBits.b[average.remap(0, 255, 0, gradLen)];
            }

            return imgLowLvl.imageBitsToBitMap(allbits, image.Width, image.Height);
        }
    }

    public static class imgLowLvl
    {
        public struct imagebits
        {
            public byte[] r;
            public byte[] g;
            public byte[] b;
            public byte[] a;
        }

        /// <summary>
        /// Converts back to a bitmap image ready for use again
        /// </summary>
        /// <returns>The channels all in bitmap form</returns>
        public static Bitmap imageBitsToBitMap(imagebits img, int x, int y)
        {
            var b = new Bitmap(x, y, PixelFormat.Format32bppArgb);

            var BoundsRect = new Rectangle(0, 0, x, y);
            BitmapData bmpData = b.LockBits(BoundsRect,
                                            ImageLockMode.WriteOnly,
                                            b.PixelFormat);

            IntPtr ptr = bmpData.Scan0;


            byte[] orderedbits = new byte[img.r.Length * 4];

            for (int i = 0; i < img.r.Length; i++)
            {
                orderedbits[i * 4] = img.r[i];
                orderedbits[i * 4 + 1] = img.g[i];
                orderedbits[i * 4 + 2] = img.b[i];
                orderedbits[i * 4 + 3] = img.a[i];
            }

            Marshal.Copy(orderedbits, 0, ptr, img.r.Length * 4);
            b.UnlockBits(bmpData);
            return b;
        }

        public static imagebits fastbits(Bitmap source)
        {
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);
            BitmapData bmpData = source.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride * source.Height;
            byte[] rgbaValues = new byte[bytes];
            byte[] r = new byte[bytes / 4];
            byte[] g = new byte[bytes / 4];
            byte[] b = new byte[bytes / 4];
            byte[] a = new byte[bytes / 4];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbaValues, 0, bytes);

            int count = 0;
            int stride = bmpData.Stride;

            for (int column = 0; column < bmpData.Height; column++)
            {
                for (int row = 0; row < bmpData.Width; row++)
                {
                    b[count] = (byte)(rgbaValues[(column * stride) + (row * 4)]);
                    g[count] = (byte)(rgbaValues[(column * stride) + (row * 4) + 1]);
                    r[count] = (byte)(rgbaValues[(column * stride) + (row * 4) + 2]);
                    a[count++] = (byte)(rgbaValues[(column * stride) + (row * 4) + 3]);
                }
            }

            imagebits allbits = new imagebits();
            allbits.r = r;
            allbits.g = g;
            allbits.b = b;
            allbits.a = a;

            source.UnlockBits(bmpData);

            return allbits;
        }
    }
}
