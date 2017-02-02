using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGradients.color
{
    public static class format
    {
        public struct hsl
        {
            public float H;
            public float S;
            public float L;

            public hsl(float h, float s, float l)
            {
                H = h;
                S = s;
                L = l;
            }
        }

        public struct rgb
        {
            public int R;
            public int G;
            public int B;

            public int A;

            public rgb(int r, int g, int b, int a = 255)
            {
                R = r;
                G = g;
                B = b;

                A = a;
            }
        }

        public static hsl toHSL(this rgb source)
        {
            hsl output = new hsl();

            //GETTING 0 - 1 VALUES

            float r = (float)source.R / 255.0f;
            float g = (float)source.G / 255.0f;
            float b = (float)source.B / 255.0f;

            //FIND MAX AND MIN
            float min = Math.Min(Math.Min(r, g), b);
            float max = Math.Max(Math.Max(r, g), b);

            //LUMINANCE CALCULATION
            output.L = (min + max)/2;

            //SATURATION CALCULATIONS
            if(min == max)
            {
                output.H = 0;
                output.S = 0;
            }
            else
                output.S = output.L < 0.5f ? (max - min) / (max + min) : (max - min) / (2.0f - max - min);

            //HUE CALCULATIONS
            if (r == max)
                output.H = (g - b) / (max - min);
            else if (g == max)
                output.H = 2.0f + (b - r) / (max - min);
            else
                output.H = 4.0f + (r - g) / (max - min);

            //Fit to circle
            output.H *= 60;

            if (output.H < 0)
                output.H += 360;

            return output;
        }

        public static rgb toRGB(this hsl source)
        {
            rgb output = new rgb();

            if (source.S == 0) //The colour is a shade of grey
            {
                float val = source.L * 255f;
                output.R = (int)val;
                output.G = (int)val;
                output.B = (int)val;
            }
            else //It has some saturation
            {
                float temp2 = source.L < 0.5f ? source.L * (1.0f + source.S) : source.L + source.S - (source.L * source.S);
                float temp1 = 2.0f * source.L - temp2;

                //Get all the color component
                output.R = (int)Math.Round(convGetColorComponent(temp1, temp2, (source.H / 360f) + 1.0f / 3.0f) * 255.0f);
                output.G = (int)Math.Round(convGetColorComponent(temp1, temp2, (source.H / 360f)) * 255.0f);
                output.B = (int)Math.Round(convGetColorComponent(temp1, temp2, (source.H / 360f) - 1.0f / 3.0f) * 255.0f);
            }
            return output;
        }

        private static float convGetColorComponent(float temp1, float temp2, float temp3)
        {
            if (temp3 < 0.0f)
                temp3 += 1.0f;
            else if (temp3 > 1.0f)
                temp3 -= 1.0f;

            if (temp3 < 1.0f / 6.0f)
                return temp1 + (temp2 - temp1) * 6.0f * temp3;
            else if (temp3 < 0.5f)
                return temp2;
            else if (temp3 < 2.0f / 3.0f)
                return temp1 + ((temp2 - temp1) * ((2.0f / 3.0f) - temp3) * 6.0f);
            else
                return temp1;
        }
    }
}
