using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace DemoHeatmap
{
    public static class imaging
    {
        /// <summary>
        /// Resizes an image to a set width and height
        /// </summary>
        /// <param name="img">Source bitmap image</param>
        /// <param name="width">New width</param>
        /// <param name="height">New Height</param>
        /// <returns>Bitmap that is scaled</returns>
        public static Bitmap ScaleImage(this Bitmap img, int width, int height)
        {
            //Blank canvas object
            Bitmap canvas = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            using (Graphics gr = Graphics.FromImage(canvas))
            {
                gr.Clear(Color.Transparent);

                // This is said to give best quality when resizing images
                //TODO: Add more support for different resizing kernals
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;

                //Draw rescaled image
                gr.DrawImage(img, new Rectangle(0, 0, width, height));
            }
            return canvas;
        }
    }
}
