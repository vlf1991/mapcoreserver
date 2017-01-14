using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoHeatmap.math;

namespace DemoHeatmap
{
    public class camera
    {
        public vector2 offset;
        public float scale;
        public vector2 resolution;

        public camera()
        {
            offset = vector2.Zero();
            scale = 1.0f;
            resolution = new vector2(1024, 1024);
        }
    }


    public struct vector2
    {
        public float x;
        public float y;

        public vector2(float X, float Y)
        {
            x = X;
            y = Y;
        }

        public static vector2 Zero()
        {
            return new vector2(0, 0);
        }
    }

    public struct vector3
    {
        public float x;
        public float y;
        public float z;

        public vector3(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public static vector3 Zero()
        {
            return new vector3(0, 0, 0);
        }
    }

    public static class transforms
    {
        public static vector2 worldToScreenSpace(this vector3 position, camera cam)
        {

            return new vector2(Convert.ToInt32((position.x - cam.offset.x) / cam.scale).Clamp(0, (int)cam.resolution.x),
                (Convert.ToInt32((position.y - cam.offset.y) / cam.scale) + 1024).remap(0, (int)cam.resolution.y, (int)cam.resolution.y, 0) + (int)cam.resolution.y);
        }
    }
}
