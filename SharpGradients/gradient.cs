using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using LogicalImageEditing.lowlevel;
using SharpGradients.color;
using LogicalMathFunctions;

namespace SharpGradients
{
    public class defined_gradient
    {
        //Holds all the keys at their position in the gradient
        private Dictionary<float, format.rgb> keys = new Dictionary<float, format.rgb>();

        public format.rgb getColorAtPosition(float pos)
        {
            float lowPos = 0.0f;
            float highPos = 0.0f;

            //Look for high and low values around the key
            foreach (float s in keys.Keys)
            {
                if (s < pos && s > lowPos) //Found low
                    lowPos = s;
                else if (s > pos && s < highPos) //Found high
                    highPos = s;
            }

            format.hsl bottomColor = keys[lowPos].toHSL();
            format.hsl topColor = keys[highPos].toHSL();

            format.hsl newHSL = new format.hsl();
            //Get percent of each one
            // CUrs:
            //
            // BOTTOM COLOR         TOP COLOR
            // H    S   L           H   S   L
            // 10   0.5 0.5         20  0.1 1

            float distance = highPos - lowPos;      //Get the distance going normally
            float loopDistance = 360 - distance;    //Get the distance looping across directions

            //Find out whether to loop backwards or forwards.
            if (highPos + loopDistance - 360 != lowPos)//reverse it
                loopDistance = -loopDistance;

            if(distance < Math.Abs(loopDistance))
            {

            }
            else
            {
                
            }


            return new format.rgb();
        }
    }

    public class image_gradient
    {
        raw_image.imagebits gradientBits;

        /// <summary>
        /// Loads an image gradient from disk
        /// </summary>
        /// <param name="path">The path to load image gradient from</param>
        public image_gradient(string path)
        {
            Bitmap source = new Bitmap(path);
            gradientBits = source.toImageBits();
        }

        public Bitmap applyToImage(Bitmap image)
        {
            raw_image.imagebits allbits = image.toImageBits();

            int gradLen = gradientBits.a.Length;

            for(int i = 0; i < allbits.r.Length; i++)
            {
                int average = (allbits.r[i] + allbits.g[i] + allbits.b[i]) / 3;
                allbits.r[i] = gradientBits.r[average.remap(0, 255, 0, gradLen)];
                allbits.g[i] = gradientBits.g[average.remap(0, 255, 0, gradLen)];
                allbits.b[i] = gradientBits.b[average.remap(0, 255, 0, gradLen)];
            }

            return allbits.toBitMap();
        }
    }
}
