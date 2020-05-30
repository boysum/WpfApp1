using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using Point = System.Windows.Point;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
        }

        protected bool validData;
        string path;
        protected Thread getImageThread;
        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            string filename;
            validData = GetFilename(out filename, e);
            if (validData)
            {
                path = filename;
                //getImageThread = new Thread(new ThreadStart(LoadImage));
                //getImageThread.Start();
                LoadImage();
                e.Effects = DragDropEffects.Copy;
            }
            else
                e.Effects = DragDropEffects.None;
        }

        private bool GetFilename(out string filename, DragEventArgs e)
        {
            bool ret = false;
            filename = String.Empty;
            if ((e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                Array data = ((IDataObject)e.Data).GetData("FileDrop") as Array;
                if (data != null)
                {
                    if ((data.Length == 1) && (data.GetValue(0) is String))
                    {
                        filename = ((string[])data)[0];
                        string ext = Path.GetExtension(filename).ToLower();
                        if ((ext == ".jpg") || (ext == ".png") || (ext == ".bmp"))
                        {
                            ret = true;
                        }
                    }
                }
            }
            return ret;
        }
        BitmapImage image;
        protected void LoadImage()

        {
            image = new BitmapImage(new Uri(path));
        }
        public static void DoEvents()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(
                    delegate (object f)
                    {
                        ((DispatcherFrame)f).Continue = false;
                        return null;
                    }), frame);
            Dispatcher.PushFrame(frame);
        }
        private void Grid_Drop(object sender, DragEventArgs e)
        {
            //ImageSource image = e.Data.GetData(typeof(ImageSource)) as ImageSource;

           

            if (validData)
            {
                //while (getImageThread.IsAlive)
                //{
                //    DoEvents();
                //    Thread.Sleep(0);
                //}
                pic1.Source = image;
                tabItems.SelectedIndex = 0;
                //pictureBox1.Top = 6;
                //pictureBox1.Left = 6;
                //pictureBox1.Size = new Size(pictureBox3.Width, pictureBox3.Height);
                //pictureBox1.Image = image;
                //pic1.Source = BitmapToImageSource(image);
                //UpdateCrop();

                //tbControl.SelectTab(1);
            }
        }

        Point mouseLocation;
        Point pointOrig;
        TranslateTransform transPoint;
        bool isDragging;
        private void pic1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isDragging)
            {
                isDragging = true;
                var myLocation = e.GetPosition(canPic);
                pointOrig = new Point(myLocation.X, myLocation.Y);
                //'Debug.WriteLine("pointOrig.x = " & pointOrig.X & " pointOrig.y = " & pointOrig.Y)

                transPoint = new TranslateTransform(pointOrig.X, pointOrig.Y);
                //'Debug.WriteLine("transPoint.x = " & transPoint.X & " transPoint.y = " & transPoint.Y)
            }
        }

        private void pic1_MouseMove(object sender, MouseEventArgs e)
        {
            
            if(isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(canPic);
                var offset = position - pointOrig;
                pointOrig = position;
                Canvas.SetLeft(pic1, Canvas.GetLeft(pic1) + offset.X);
                Canvas.SetTop(pic1, Canvas.GetTop(pic1) + offset.Y);
                //mouseLocation = e.GetPosition(canPic);
                //transPoint.X = (mouseLocation.X - pointOrig.X);
                //transPoint.Y = (mouseLocation.Y - pointOrig.Y);
                //pointOrig = mouseLocation;
                //pic1.RenderTransform = transPoint;
            }
        }

        private void canPic_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            pic1.Height = Convert.ToInt32(pic1.Height + e.Delta * 0.15); 
            pic1.Width = Convert.ToInt32(pic1.Width + e.Delta * 0.15);
            //pictureBox1.Top = Convert.ToInt32(pictureBox1.Top - e.Delta * 0.1);
            //pictureBox1.Left = Convert.ToInt32(pictureBox1.Left - e.Delta * 0.075);
        }

        private void pic1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            //canPic.ReleaseMouseCapture();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string ImageDirectory = @"C:\testimages";
            Directory.CreateDirectory(ImageDirectory);
            int panelWidth = (int)imgOverlay.Width;
            int panelHeight = (int)imgOverlay.Height;
            string filePath = Path.Combine(ImageDirectory, "Snapshot " + DateTime.Now.ToString("hhmmddss") + ".jpg");
            Point pnt = imgOverlay.PointToScreen(new Point(0,0));
            borderOverlay.Visibility = Visibility.Collapsed;
            this.imgOverlay.Visibility = Visibility.Collapsed;
            //Canvas.SetZIndex(borderOverlay, 0);
            //Canvas.SetZIndex(imgOverlay, 0);
            //Canvas.SetZIndex(pic1, 1);
            Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            
            using (var bmp = new Bitmap(panelWidth, panelHeight))
            {
                using (var grx = Graphics.FromImage(bmp))
                {                    
                    //var src = new System.Drawing.Point((int)Canvas.GetLeft(pic1), (int)Canvas.GetTop(pic1));
                    grx.CopyFromScreen(new System.Drawing.Point((int)pnt.X, (int)pnt.Y), System.Drawing.Point.Empty, new System.Drawing.Size(panelWidth, panelHeight));
                    
                }

                //var format = new ImageFormat();
               
                bmp.Save(filePath, ImageFormat.Jpeg);
            }
            //Canvas.SetZIndex(pic1, 0);
            borderOverlay.Visibility = Visibility.Visible;
            imgOverlay.Visibility = Visibility.Visible;
            Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);

        }
        private bool isMaximized;
        private Rect normalBounds;
        private void Window_StateChanged(object sender, EventArgs e)
        {
            //Application.Current.MainWindow.WindowState = WindowState.Maximized;
            //if (WindowState == WindowState.Maximized && !isMaximized)
            //{
            //    WindowState = WindowState.Normal;
            //    isMaximized = true;

            //    normalBounds = RestoreBounds;

            //    Height = SystemParameters.WorkArea.Height;
            //    MaxHeight = Height;
            //    MinHeight = Height;
            //    Top = 0;
            //    Left = SystemParameters.WorkArea.Right - Width;
            //    //SetMovable(false);
            //}
            //else if (WindowState == WindowState.Maximized && isMaximized)
            //{
            //    WindowState = WindowState.Normal;
            //    isMaximized = false;

            //    MaxHeight = Double.PositiveInfinity;
            //    MinHeight = 0;

            //    Top = normalBounds.Top;
            //    Left = normalBounds.Left;
            //    Width = normalBounds.Width;
            //    Height = normalBounds.Height;
            //   // SetMovable(true);
            //}
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }
        //BitmapImage BitmapToImageSource(Bitmap bitmap)
        //{
        //    using (MemoryStream memory = new MemoryStream())
        //    {
        //        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
        //        memory.Position = 0;
        //        BitmapImage bitmapimage = new BitmapImage();
        //        bitmapimage.BeginInit();
        //        bitmapimage.StreamSource = memory;
        //        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapimage.EndInit();

        //        return bitmapimage;
        //    }
        //}
    }
}
