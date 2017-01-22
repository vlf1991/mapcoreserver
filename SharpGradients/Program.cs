using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SharpGradients
{
    class Program
    {
        static void Main(string[] args)
        {
            image_gradient imgr = new image_gradient("gradients/purple-red-yellow.png");

            Bitmap testimage = new Bitmap("cat.jpg");
            testimage = imgr.applyToImage(testimage);
            testimage.Save("test.png");
        }
    }
}
