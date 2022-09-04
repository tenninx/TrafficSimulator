using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TS.Map.TEST
{
    /// <summary>
    /// MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            cvMain.MouseLeftButtonUp += new MouseButtonEventHandler(cvMain_MouseLeftButtonUp);

            button1.Click += new RoutedEventHandler(button1_Click);

        }

        void button1_Click(object sender, RoutedEventArgs e)
        {
            List<Chunk> list=TS.Map.Generator.GetMap(0);
            foreach (Chunk c in list)
            {
                LineGeometry line = new LineGeometry();
                line.StartPoint = new Point(c.X1, c.Y1);
                line.EndPoint = new Point(c.X2, c.Y2);

                // Path
                Path myPath = new Path();
                myPath.Stroke = Brushes.Black;
                myPath.StrokeThickness = 1;

                // Add to canvas
                myPath.Data = line;
                cvMain.Children.Add(myPath);
            }
        }

        void cvMain_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string[] strFirst =txtFirstno.Text.Split(',');

            LineGeometry line = new LineGeometry();
            line.StartPoint = new Point(Convert.ToInt32(strFirst[0]), Convert.ToInt32(strFirst[1]));
            line.EndPoint = Mouse.GetPosition(cvMain);

            // Text Reset
            string strLastPoint = Convert.ToInt32(Mouse.GetPosition(cvMain).X).ToString() + "," + Convert.ToInt32(Mouse.GetPosition(cvMain).Y).ToString();

            //Chunk
            txtResult.Text += txtFirstno.Text + "_" + strLastPoint+"@";
            txtFirstno.Text = strLastPoint;

            // Path
            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 1;

            // Add to canvas
            myPath.Data = line;
            cvMain.Children.Add(myPath);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        // Get Intersection
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            List<Intersection> Intersection = TS.Map.Generator.GetIntersection(0);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            List<Chunk> list = TS.Map.Generator.GetRoute(0);
            foreach (Chunk c in list)
            {
                LineGeometry line = new LineGeometry();
                line.StartPoint = new Point(c.X1, c.Y1);
                line.EndPoint = new Point(c.X2, c.Y2);

                // Path
                Path myPath = new Path();
                myPath.Stroke = Brushes.Black;
                myPath.StrokeThickness = 1;

                // Add to canvas
                myPath.Data = line;
                cvMain.Children.Add(myPath);
            }
        }

        private void cvMain_MouseMove(object sender, MouseEventArgs e)
        {
            labCoor.Text = e.GetPosition(cvMain).X + ", " + e.GetPosition(cvMain).Y;
        }
    }
}
