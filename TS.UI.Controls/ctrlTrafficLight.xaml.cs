using System.Windows;
using System.Windows.Controls;

namespace TS.UI.Controls
{
    public partial class ctrlTrafficLight : UserControl
    {
        #region Field
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(ctrlTrafficLight), new PropertyMetadata(null));
        public static readonly DependencyProperty CountDownProperty = DependencyProperty.Register("CountDown", typeof(int), typeof(ctrlTrafficLight), new PropertyMetadata(null));

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public int CountDown
        {
            get { return (int)GetValue(CountDownProperty); }
            set { SetValue(CountDownProperty, value); }
        }
        #endregion

        public ctrlTrafficLight()
        {
            InitializeComponent();
        }
    }
}
