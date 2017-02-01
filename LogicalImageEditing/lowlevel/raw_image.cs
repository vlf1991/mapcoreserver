using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace LogicalImageEditing.lowlevel
{
    public static class raw_image
    {
        public struct imagebits
        {
            public int width;
            public int height;

            public byte[] r;
            public byte[] g;
            public byte[] b;
            public byte[] a;
        }

        [Obsolete("Use .toBitMap()")]
        public static Bitmap imageBitsToBitMap(imagebits img)
        {
            var b = new Bitmap(img.width, img.height, PixelFormat.Format32bppArgb);

            var BoundsRect = new Rectangle(0, 0, img.width, img.height);
            BitmapData bmpData = b.LockBits(BoundsRect,
                                            ImageLockMode.WriteOnly,
                                            b.PixelFormat);

            IntPtr ptr = bmpData.Scan0;


            byte[] orderedbits = new byte[img.r.Length * 4];

            for (int i = 0; i < img.r.Length; i++)
            {
                orderedbits[i * 4] = img.b[i];
                orderedbits[i * 4 + 1] = img.g[i];
                orderedbits[i * 4 + 2] = img.r[i];
                orderedbits[i * 4 + 3] = img.a[i];
            }

            Marshal.Copy(orderedbits, 0, ptr, img.r.Length * 4);
            b.UnlockBits(bmpData);
            return b;
        }

        [Obsolete("Use .toImageBits()")]
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

            allbits.width = bmpData.Width;
            allbits.height = bmpData.Height;

            source.UnlockBits(bmpData);

            return allbits;
        }

        //New extension methods
        public static Bitmap toBitMap(this imagebits img)
        {
            return imageBitsToBitMap(img);
        }
        public static imagebits toImageBits(this Bitmap source)
        {
            return fastbits(source);
        }

        public static BitmapImage toBitMapImage(this Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}
