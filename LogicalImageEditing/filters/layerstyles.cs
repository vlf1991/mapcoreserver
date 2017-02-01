using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using LogicalImageEditing.lowlevel;
using LogicalMathFunctions;

namespace LogicalImageEditing.filters
{
    public static class layerstyles
    {
        public static Bitmap greyScaleAverage(this Bitmap input)
        {
            raw_image.imagebits source = input.toImageBits();

            for (int i = 0; i < source.b.Length; i++)
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

            return source.toBitMap();
        }

        public static Bitmap alphaOver(this Bitmap input, Bitmap overlay)
        {
            if (input.Width == overlay.Width && input.Height == overlay.Height)
            {
                raw_image.imagebits sourceImg = input.toImageBits();
                raw_image.imagebits overlayImg = overlay.toImageBits();

                for (int i = 0; i < sourceImg.b.Length; i++)
                {
                    int alphaValue = overlayImg.a[i];
                    float percent = (float)alphaValue / 255f;

                    sourceImg.b[i] = (byte)Convert.ToInt32((sourceImg.b[i] * (1 - percent)) + (overlayImg.b[i] * percent));
                    sourceImg.g[i] = (byte)Convert.ToInt32((sourceImg.g[i] * (1 - percent)) + (overlayImg.g[i] * percent));
                    sourceImg.r[i] = (byte)Convert.ToInt32((sourceImg.r[i] * (1 - percent)) + (overlayImg.r[i] * percent));
                }

                return sourceImg.toBitMap();
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
                raw_image.imagebits sourceImg = input.toImageBits();
                raw_image.imagebits overlayImg = overlay.toImageBits();

                for (int i = 0; i < sourceImg.b.Length; i++)
                {
                    int alphaValue = overlayImg.a[i];
                    float percent = (float)alphaValue / 255f;

                    sourceImg.b[i] = (byte)Convert.ToInt32(sourceImg.b[i] + (overlayImg.b[i] * percent)).Clamp(0, 255);
                    sourceImg.g[i] = (byte)Convert.ToInt32(sourceImg.g[i] + (overlayImg.g[i] * percent)).Clamp(0, 255);
                    sourceImg.r[i] = (byte)Convert.ToInt32(sourceImg.r[i] + (overlayImg.r[i] * percent)).Clamp(0, 255);
                }

                return sourceImg.toBitMap();
            }
            else
            {
                //throw new ArgumentException("Images were not the same dimentions, cannot operate", "overlay");
                return input;
            }
        }

        public static Bitmap brightness(Bitmap input, float value)
        {
            raw_image.imagebits sourceImg = input.toImageBits();

            for (int i = 0; i < sourceImg.b.Length; i++)
            {
                sourceImg.b[i] = (byte)Convert.ToInt32(sourceImg.b[i] * value).Clamp(0, 255);
                sourceImg.g[i] = (byte)Convert.ToInt32(sourceImg.g[i] * value).Clamp(0, 255);
                sourceImg.r[i] = (byte)Convert.ToInt32(sourceImg.r[i] * value).Clamp(0, 255);
            }

            return sourceImg.toBitMap();
        }
    }
}
