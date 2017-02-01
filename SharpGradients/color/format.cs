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

            public rgb(int r, int g, int b)
            {
                R = r;
                G = g;
                B = b;
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
                output.S = 0;
            }
            else
            {
                if (output.L < 0.5f)
                    output.S = (max - min) / (max + min);
                else
                    output.S = (max - min) / (2.0f - max - min);

            }

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

            if(source.S == 0) //The colour is a shade of grey
            {
                float val = source.S * 255;
                output.R = (int)val;
                output.G = (int)val;
                output.B = (int)val;
            }
            else //Its not a shade of grey
            {
                float temporary_1 = 0.0f;
                if (source.L < 0.5f)
                    temporary_1 = source.L * (1.0f + source.S);
                else
                    temporary_1 = source.L + source.S - source.L * source.S;


                float temporary_2 = 2 * source.L - temporary_1;

                float pHue = source.H / 360.0f;


                float RR = Math.Abs(pHue + 0.333f);
                float GG = Math.Abs(pHue);
                float BB = Math.Abs(pHue - 0.333f);

            }




            //TODO: FINISH THIS
            return output;
        }
    }
}
