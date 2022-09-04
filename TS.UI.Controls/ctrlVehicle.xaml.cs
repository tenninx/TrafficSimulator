using System.Windows;
using System.Windows.Controls;

namespace TS.UI.Controls
{
    public partial class ctrlVehicle : UserControl
    {
        #region Fields
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(string), typeof(ctrlVehicle), new PropertyMetadata(null));

        public string Speed
        {
            get { return (string)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }
        #endregion

        public ctrlVehicle()
        {
            InitializeComponent();
        }
    }
}
