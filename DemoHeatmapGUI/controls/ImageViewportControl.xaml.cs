using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DemoHeatmapGUI.imaging;

namespace DemoHeatmapGUI.controls
{
    /// <summary>
    /// Interaction logic for ImageViewportControl.xaml
    /// </summary>
    public partial class ImageViewportControl : UserControl
    {
        //Running params
        private Point origin;
        private Point start;

        public string test;

        public ImageViewportControl()
        {
            InitializeComponent();

            //Transforms
            TransformGroup group = new TransformGroup();

            ScaleTransform xform = new ScaleTransform();
            group.Children.Add(xform);

            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);


            //Setting render transforms and handlers
            disp.RenderTransform = group;
            disp.MouseLeftButtonDown += image_MouseLeftButtonDown;
            disp.MouseLeftButtonUp += image_MouseLeftButtonUp;
            disp.MouseMove += image_MouseMove;
            disp.MouseWheel += image_MouseWheel;

            //TEST BITMAP
            //System.Drawing.Bitmap test = new System.Drawing.Bitmap("D:/floor.jpg");

            //set_image(test);
        }

        //Button up handler
        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            disp.ReleaseMouseCapture();
        }

        //Mouse move handler
        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!disp.IsMouseCaptured) return;

            var tt = (TranslateTransform)((TransformGroup)disp.RenderTransform).Children.First(tr => tr is TranslateTransform);
            Vector v = start - e.GetPosition(border);
            tt.X = origin.X - v.X;
            tt.Y = origin.Y - v.Y;
        }

        //Button down handler
        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            disp.CaptureMouse();
            var tt = (TranslateTransform)((TransformGroup)disp.RenderTransform).Children.First(tr => tr is TranslateTransform);
            start = e.GetPosition(border);
            origin = new Point(tt.X, tt.Y);
        }

        //Scroll handler
        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            TransformGroup transformGroup = (TransformGroup)disp.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];

            double zoom = e.Delta > 0 ? .2 : -.2;
            transform.ScaleX += zoom;
            transform.ScaleY += zoom;
        }

        //Zoom in Method
        private void image_zoomin(object sender, EventArgs e)
        {
            TransformGroup transformGroup = (TransformGroup)disp.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];

            transform.ScaleX += 0.1;
            transform.ScaleY += 0.1;
        }

        //Zoom out
        private void image_zoomout(object sender, EventArgs e)
        {
            TransformGroup transformGroup = (TransformGroup)disp.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];
            transform.ScaleX -= 0.1;
            transform.ScaleY -= 0.1;
        }

        //Reset
        private void image_full(object sender, EventArgs e)
        {
            TransformGroup transformGroup = (TransformGroup)disp.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];
            transform.ScaleX = 1;
            transform.ScaleY = 1;

            var tt = (TranslateTransform)((TransformGroup)disp.RenderTransform).Children.First(tr => tr is TranslateTransform);
            tt.X = 0;
            tt.Y = 0;
        }

        //Sets the image to a bitmap
        public void set_image(System.Drawing.Bitmap display)
        {
            disp.Source = display.toBitMapImage();
        }
    }
}
