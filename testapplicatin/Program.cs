using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGradients.color;

namespace testapplicatin
{
    class Program
    {
        static void Main(string[] args)
        {
            format.rgb rgbvalue = new format.rgb(218, 112, 214);
            format.hsl hslval = rgbvalue.toHSL();

            Console.WriteLine("HSL value is {0} {1} {2} !", hslval.H, hslval.S * 100, (hslval.L / 2.0f) * 100);

            Console.ReadKey();
        }
    }
}
