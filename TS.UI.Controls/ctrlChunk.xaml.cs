using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TS.UI.Controls
{
    public partial class ctrlChunk : UserControl
    {
        #region Fields
        public static readonly DependencyProperty ChunkNameProperty = DependencyProperty.Register("ChunkName", typeof(string), typeof(ctrlChunk), new PropertyMetadata(null));
        public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof(double), typeof(ctrlChunk), new PropertyMetadata(null));
        public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(double), typeof(ctrlChunk), new PropertyMetadata(null));
        public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(double), typeof(ctrlChunk), new PropertyMetadata(null));
        public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(double), typeof(ctrlChunk), new PropertyMetadata(null));
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(ctrlChunk), new PropertyMetadata(null));
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ctrlChunk), new PropertyMetadata(null));

        public string ChunkName
        {
            get { return (string)GetValue(ChunkNameProperty); }
            set { SetValue(ChunkNameProperty, value); }
        }

        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        #endregion

        public ctrlChunk()
        {
            InitializeComponent();
        }
    }
}
