using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SharpHeatMaps.gradients;
using SharpHeatMaps.math;

namespace SharpHeatMaps.imaging
{
    public static class bitmapfilters
    {
        public static Bitmap greyScaleAverage(Bitmap input)
        {
            imgLowLvl.imagebits source = imgLowLvl.fastbits(input);

            for(int i = 0; i < source.b.Length; i++)
            {
                int total = 0;
                total += source.b[i];
                total += source.g[i];
                total += source.r[i];

                int newVal = total / 3;

                source.b[i] = (byte)newVal;
                source.g[i] = (byte)newVal;
                source.r[i] = (byte)newVal;
            }

            return imgLowLvl.imageBitsToBitMap(source, input.Width, input.Height);
        }

        public static Bitmap alphaOver(Bitmap input, Bitmap overlay)
        {
            if (input.Width == overlay.Width && input.Height == overlay.Height)
            {
                imgLowLvl.imagebits sourceImg = imgLowLvl.fastbits(input);
                imgLowLvl.imagebits overlayImg = imgLowLvl.fastbits(overlay);

                for (int i = 0; i < sourceImg.b.Length; i++)
                {
                    int alphaValue = overlayImg.a[i];
                    float percent = (float)alphaValue / 255f;

                    sourceImg.b[i] = (byte)Convert.ToInt32((sourceImg.b[i] * (1 - percent)) + (overlayImg.b[i] * percent));
                    sourceImg.g[i] = (byte)Convert.ToInt32((sourceImg.g[i] * (1 - percent)) + (overlayImg.g[i] * percent));
                    sourceImg.r[i] = (byte)Convert.ToInt32((sourceImg.r[i] * (1 - percent)) + (overlayImg.r[i] * percent));
                }

                return imgLowLvl.imageBitsToBitMap(sourceImg, input.Width, input.Height);
            }
            else
            {
                //throw new ArgumentException("Images were not the same dimentions, cannot operate", "overlay");
                return input;
            }
        }

        public static Bitmap addition(Bitmap input, Bitmap overlay)
        {
            if (input.Width == overlay.Width && input.Height == overlay.Height)
            {
                imgLowLvl.imagebits sourceImg = imgLowLvl.fastbits(input);
                imgLowLvl.imagebits overlayImg = imgLowLvl.fastbits(overlay);

                for (int i = 0; i < sourceImg.b.Length; i++)
                {
                    int alphaValue = overlayImg.a[i];
                    float percent = (float)alphaValue / 255f;

                    sourceImg.b[i] = (byte)Convert.ToInt32(sourceImg.b[i] + (overlayImg.b[i] * percent)).Clamp(0, 255);
                    sourceImg.g[i] = (byte)Convert.ToInt32(sourceImg.g[i] + (overlayImg.g[i] * percent)).Clamp(0, 255);
                    sourceImg.r[i] = (byte)Convert.ToInt32(sourceImg.r[i] + (overlayImg.r[i] * percent)).Clamp(0, 255);
                }

                return imgLowLvl.imageBitsToBitMap(sourceImg, input.Width, input.Height);
            }
            else
            {
                //throw new ArgumentException("Images were not the same dimentions, cannot operate", "overlay");
                return input;
            }
        }

        public static Bitmap brightness(Bitmap input, float value)
        {
            imgLowLvl.imagebits sourceImg = imgLowLvl.fastbits(input);

            for (int i = 0; i < sourceImg.b.Length; i++)
            {
                sourceImg.b[i] = (byte)Convert.ToInt32(sourceImg.b[i] * value).Clamp(0, 255);
                sourceImg.g[i] = (byte)Convert.ToInt32(sourceImg.g[i] * value).Clamp(0, 255);
                sourceImg.r[i] = (byte)Convert.ToInt32(sourceImg.r[i] * value).Clamp(0, 255);
            }

            return imgLowLvl.imageBitsToBitMap(sourceImg, input.Width, input.Height);
        }
    }
}
