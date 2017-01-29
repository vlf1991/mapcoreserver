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
        private Point origin;
        private Point start;

        public string test;

        public ImageViewportControl()
        {
            InitializeComponent();

            TransformGroup group = new TransformGroup();

            ScaleTransform xform = new ScaleTransform();
            group.Children.Add(xform);

            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);

            disp.RenderTransform = group;
            disp.MouseLeftButtonDown += image_MouseLeftButtonDown;
            disp.MouseLeftButtonUp += image_MouseLeftButtonUp;
            disp.MouseMove += image_MouseMove;
            disp.MouseWheel += image_MouseWheel;


            System.Drawing.Bitmap test = new System.Drawing.Bitmap("D:/floor.jpg");

            set_image(test);
        }

        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            disp.ReleaseMouseCapture();
        }

        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!disp.IsMouseCaptured) return;

            var tt = (TranslateTransform)((TransformGroup)disp.RenderTransform).Children.First(tr => tr is TranslateTransform);
            Vector v = start - e.GetPosition(border);
            tt.X = origin.X - v.X;
            tt.Y = origin.Y - v.Y;
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            disp.CaptureMouse();
            var tt = (TranslateTransform)((TransformGroup)disp.RenderTransform).Children.First(tr => tr is TranslateTransform);
            start = e.GetPosition(border);
            origin = new Point(tt.X, tt.Y);
        }

        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            TransformGroup transformGroup = (TransformGroup)disp.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];

            double zoom = e.Delta > 0 ? .2 : -.2;
            transform.ScaleX += zoom;
            transform.ScaleY += zoom;
        }

        private void image_zoomin(object sender, EventArgs e)
        {
            TransformGroup transformGroup = (TransformGroup)disp.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];
            transform.ScaleX += 0.1;
            transform.ScaleY += 0.1;
        }

        private void image_zoomout(object sender, EventArgs e)
        {
            TransformGroup transformGroup = (TransformGroup)disp.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];
            transform.ScaleX -= 0.1;
            transform.ScaleY -= 0.1;
        }

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

        public void set_image(System.Drawing.Bitmap display)
        {
            disp.Source = display.toBitMapImage();
        }
    }
}
