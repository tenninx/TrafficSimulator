using System.Windows;
using System.Windows.Input;

namespace TS.UI
{
    public partial class winAbout : Window
    {
        public winAbout()
        {
            InitializeComponent();
        }

        private void btnClose_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
