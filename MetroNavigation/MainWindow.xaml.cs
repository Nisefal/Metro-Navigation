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
using System.Windows.Media.Animation;
using System.IO;

namespace MetroNavigation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private string stations = "";   //was used wihile initializing points
        //private bool JustOpened = true; //was used wihile initializing points
        private int[,] coord;
        private int[,] matrX;
        private string[] StatNums;
        private string[] StatNames;
        private bool[] visited;


        public MainWindow()
        {
            InitializeComponent();
            InitializeFT();
            InitCoord();
            InitPics();
            matrX = ReadMetr();
        }

        /// <summary>
        /// Creates animation of beaming ellipses
        /// </summary>
        /// <param name="m"></param>
        /// <param name="i"></param>
        private void CreateAnimation(Point m, int i)
        {
            App.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;

            Ellipse circle = new Ellipse();
            circle.Height = 15;
            circle.Width = 15;
            circle.Opacity = 1;
            circle.Fill = Brushes.Blue;
            circle.SetValue(Canvas.LeftProperty, (Double)m.X);
            circle.SetValue(Canvas.TopProperty, (Double)m.Y);

            DoubleAnimation myDoubleAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
            myDoubleAnimation.AutoReverse = true;
            myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            myDoubleAnimation.BeginTime = TimeSpan.FromSeconds(0.5*i);
            circle.BeginAnimation(Ellipse.OpacityProperty, myDoubleAnimation);

            DrawDesk.Children.Add(circle);
        }

        /// <summary>
        /// Event handler. Starts animation of path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            DrawDesk.Children.Clear();
            WayPoints.Items.Clear();
            List<int> way = FindWay(StatNums[From.SelectedIndex], StatNums[To.SelectedIndex]);
            if (way != null)
            {
                int i = 0;
                foreach (int el in way)
                {
                    CreateAnimation(new Point() { X = coord[el, 0], Y = coord[el, 1] }, i);
                    WayPoints.Items.Add(new Label() { Content = StatNames[el] });
                    i++;
                }
            }
            else 
            {
                WayPoints.Items.Add(new Label() { Content = "Turn left and one step at left." });
                WayPoints.Items.Add(new Label() { Content = "Then turn left again." });
                WayPoints.Items.Add(new Label() { Content = "At last turn back and make one step forward." });
                WayPoints.Items.Add(new Label() { Content = "You are at the right place!" });
            }
        }

        /// <summary>
        /// Returns list of point ID's between first and last
        /// </summary>
        /// <param name="location">first point number like "110"</param>
        /// <param name="finish">last point number</param>
        /// <returns></returns>
        private List<int> FindWay(string location, string finish)
        {
            if (location == finish)
                return null;
            List<int> loclist = new List<int>();
            List<int> finlist = new List<int>();
            visited = new bool[52];
            int st=-1, cur, fin=-1;
            for (int i = 0; i < 52; i++)
            {
                if (location == StatNums[i])
                    st = i;
                if (finish == StatNums[i])
                    fin = i;
                if (st > -1 && fin > -1)
                    break;
            }
            cur = st;
            int locb, anb;
            loclist.Add(st);
            visited[st] = true;
            while (cur!=fin)
	        {
                locb = -1;
                anb = -1;
                for (int i = 0; i < 52; i++)
                {
                    if (matrX[cur, i] != 0 && !visited[i] && (StatNums[cur])[0] == (StatNums[i])[0]) locb = i;
                    else if (matrX[cur, i] != 0 && !visited[i] && (StatNums[fin])[0] == (StatNums[i])[0]) anb = i;
                }
                if(anb>0)
                {
                    cur = anb;
                    visited[cur] = true;
                    st = cur;
                    loclist.Add(cur);
                    foreach(var el in loclist)
                        finlist.Add(el);
                    loclist.Clear();
                }
                else if(locb>0)
                {
                    cur = locb;
                    visited[cur] = true;
                    loclist.Add(cur);
                }
                else if (cur != st) 
                { 
                    cur = st;
                    loclist.Clear();
                    loclist.Add(cur);
                }
	        }
            foreach (int el in loclist)
                finlist.Add(el);
            return finlist;
        }

        /// <summary>
        /// Calibration function. 
        /// Was used to get points X,Y position on map and write coordinates in list. 
        /// 
        /// 
        /// DO NOT UNCOMMENT THIS FUNCTION BODY IF YOU DON'T WANT TO SEE THIS APP CRASHING!!!
        /// 
        /// 
        /// You'll just break all point positions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawDesk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ////Point p = Mouse.GetPosition(DrawDesk);
            ////Draw(p);
            ////if (JustOpened)
            ////{
            ////    JustOpened = false;
            ////    FileStream fs1 = new FileStream("Stations.txt", FileMode.Create, FileAccess.Write);
            ////    StreamWriter sw1 = new StreamWriter(fs1);
            ////    sw1.WriteLine("");
            ////    sw1.Close();
            ////    fs1.Close();
            ////}
            ////StreamReader sr = new StreamReader("Stations.txt", Encoding.UTF8);
            ////stations = sr.ReadToEnd();
            ////if (stations[0] == '\r')
            ////    stations = "";
            ////sr.Dispose();
            ////FileStream fs = new FileStream("Stations.txt", FileMode.OpenOrCreate, FileAccess.Write);
            ////StreamWriter sw = new StreamWriter(fs);
            ////stations += p.X + " " + p.Y;
            ////sw.WriteLine(stations);
            ////sw.Close();
            ////fs.Close();
        }

        ///first function of drawing points
        //private void Draw(Point m)
        //{
        //    int mX = (int)m.X;
        //    int mY = (int)m.Y;
        //    Ellipse el = new Ellipse();
        //    el.Width = 15;
        //    el.Height = 15;
        //    el.SetValue(Canvas.LeftProperty, (Double)mX);
        //    el.SetValue(Canvas.TopProperty, (Double)mY);
        //    el.Fill = Brushes.LightBlue;
        //    DrawDesk.Children.Add(el);
        //}

        /// <summary>
        /// Returns matrix of edges. Each edge betwen two points is 1. No edge - 0.
        /// </summary>
        /// <returns></returns>
        private int[,] ReadMetr()
        {
            try
            {
                int[,] A = new int[52, 52];

                StreamReader sr = new StreamReader("./Lines.txt", Encoding.UTF8);
                string list_stat = sr.ReadToEnd();
                sr.Dispose();
                string[] strings = list_stat.Split('\n');
                int a, b;
                foreach (string s in strings)
                {
                    if (s.Length != 0)
                    {
                        string[] loc = s.Split(' ');
                        a = Convert.ToInt32(loc[0]);
                        b = Convert.ToInt32((loc[1].Split('\r'))[0]);
                        A[a, b] = 1;
                        A[b, a] = 1;
                    }
                }
                return A;
            }
            catch(Exception)
            {
                MessageBox.Show("Matr!");
                return null;
            }
        }

        /// <summary>
        /// Initialize "From" and "To" comboboxes
        /// </summary>
        private void InitializeFT()
        {
            StatNames = new string[52];
            StatNums = new string[52];
            try
            {
                StreamReader sr = new StreamReader("./Names.txt", Encoding.UTF8);
            
                string list_stat = sr.ReadToEnd();
                sr.Dispose();
                string[] strings = list_stat.Split('\n');
                string[] tmp;
                int iter = 0;
                foreach (string s in strings)
                {
                    if (s.Length != 0)
                    {
                        tmp = ((s.Split('\r'))[0]).Split(',');
                        StatNums[iter] = tmp[0];
                        StatNames[iter] =tmp[1];
                        From.Items.Add(new ComboBoxItem() { Content = Convert.ToString(StatNames[iter]) });
                        To.Items.Add(new ComboBoxItem() { Content = Convert.ToString(StatNames[iter]) });
                        iter++;
                    }
                }
            }
            catch (Exception)
            {
                try
                {
                    StreamReader sr = new StreamReader("../Names.txt", Encoding.UTF8);
                }
                catch(Exception)
                {
                    MessageBox.Show("FT!");
                }
            }

        }

        /// <summary>
        /// Read XY coordinates of stations.
        /// </summary>
        private void InitCoord()
        {
            try
            {
                Point p = new Point();
                StreamReader sr = new StreamReader("./Stations.txt", Encoding.UTF8);
                string list_stat = sr.ReadToEnd();
                sr.Dispose();
                string[] strings = list_stat.Split('\n');
                coord = new int[strings.Length, 2];
                int iter = 0;
                foreach (string s in strings)
                {
                    if (s.Length != 0)
                    {
                        string[] loc = s.Split(' ');
                        coord[iter, 0] = Convert.ToInt32(loc[0]);
                        p.X = coord[iter, 0];
                        coord[iter, 1] = Convert.ToInt32((loc[1].Split('\r'))[0]);
                        p.Y = coord[iter, 1];
                        iter = iter + 1;
                    }
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Coord!");
            }

        }

        /// <summary>
        /// Sets map image.
        /// </summary>
        private void InitPics()
        {
            try
            {
            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource = new BitmapImage(new Uri("../../Img/kiev-metro.jpg", UriKind.RelativeOrAbsolute));
            DrawDesk.Background = myBrush;
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                try
                {
                    ImageBrush myBrush = new ImageBrush();
                    myBrush.ImageSource = new BitmapImage(new Uri("../Img/kiev-metro.jpg", UriKind.RelativeOrAbsolute));
                    DrawDesk.Background = myBrush;
                }
                catch (DirectoryNotFoundException)
                {
                    try
                    {
                        ImageBrush myBrush = new ImageBrush();
                        myBrush.ImageSource = new BitmapImage(new Uri("../Img/kiev-metro.jpg", UriKind.RelativeOrAbsolute));
                        DrawDesk.Background = myBrush;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        try
                        {
                            ImageBrush myBrush = new ImageBrush();
                            myBrush.ImageSource = new BitmapImage(new Uri("./kiev-metro.jpg", UriKind.RelativeOrAbsolute));
                            DrawDesk.Background = myBrush;

                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Pics!");
                        }
                    }
                }
            }
        }
    }
}
