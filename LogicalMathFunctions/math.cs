using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalMathFunctions
{
    public static class math
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static int remap(this int val, int min1, int max1, int min2, int max2)
        {
            return min1 + (val - min1) * (max2 - min2) / (max1 - min1);
        }

        public static int remapClamped(this int val, int min1, int max1, int min2, int max2)
        {
            return val.remap(min1, max1, min2, max2).Clamp(min2, max2);
        }



        //FLOAT CLONES
        public static float remapF(this float val, float min1, float max1, float min2, float max2)
        {
            return min1 + (val - min1) * (max2 - min2) / (max1 - min1);
        }

        public static float remapClampedF(this float val, float min1, float max1, float min2, float max2)
        {
            return val.remapF(min1, max1, min2, max2).Clamp(min2, max2);
        }


        public static int Abs(this int val)
        {
            return Math.Abs(val);
        }
    }
}
