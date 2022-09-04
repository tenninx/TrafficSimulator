using System.Windows;
using TS.Library;

namespace TS.UI
{
    public partial class winAdvConfig : Window
    {
        public winAdvConfig()
        {
            InitializeComponent();
        }

        #region Event Handling
        private void cboxIsTimed_Checked(object sender, RoutedEventArgs e)
        {
            objCfgTermTime.IsEnabled = cboxIsTimed.IsChecked.Value;
            if (!objCfgTermTime.IsEnabled)
            {
                clsConfig.TerminateTimer = 0;
                labCfgTermTime.Text = clsConfig.TerminateTimer + " secs";
            }
            else
            {
                if (clsConfig.TerminateTimer == 0)
                    clsConfig.TerminateTimer = 60;
                objCfgTermTime.Value = clsConfig.TerminateTimer;
                labCfgTermTime.Text = clsConfig.TerminateTimer + " secs";
            }
        }

        private void winAdvConfig_Loaded(object sender, RoutedEventArgs e)
        {
            // Map Scale
            objCfgMapScale.Minimum = 1;
            objCfgMapScale.Maximum = 10;
            objCfgMapScale.SmallChange = 1;
            objCfgMapScale.LargeChange = 2;
            objCfgMapScale.Value = clsConfig.MeterPerPixel;
            labCfgMapScale.Text = clsConfig.MeterPerPixel + " m";
            objCfgMapScale.ValueChanged += new RoutedPropertyChangedEventHandler<double>(objCfgMapScale_ValueChanged);

            // Reckless Driver Rate
            objCfgReckRate.Minimum = 0;
            objCfgReckRate.Maximum = 50;
            objCfgReckRate.SmallChange = 1;
            objCfgReckRate.LargeChange = 10;
            objCfgReckRate.Value = clsConfig.RecklessPercentage;
            labCfgReckRate.Text = clsConfig.RecklessPercentage + " %";
            objCfgReckRate.ValueChanged += new RoutedPropertyChangedEventHandler<double>(objCfgReckRate_ValueChanged);

            // Non-congested Rate
            objCfgNonConRate.Minimum = 20;   // 20% min
            objCfgNonConRate.Maximum = 70;   // 70% max
            objCfgNonConRate.SmallChange = 1;
            objCfgNonConRate.LargeChange = 10;
            objCfgNonConRate.Value = clsConfig.NonCongestedPercent * 100;
            labCfgNonConRate.Text = clsConfig.NonCongestedPercent * 100 + " %";
            objCfgNonConRate.ValueChanged += new RoutedPropertyChangedEventHandler<double>(objCfgNonConRate_ValueChanged);

            // Vehicle Size
            objCfgVehicleSize.Minimum = 3;
            objCfgVehicleSize.Maximum = 10;
            objCfgVehicleSize.SmallChange = 1;
            objCfgVehicleSize.LargeChange = 3;
            objCfgVehicleSize.Value = clsConfig.VehicleLength;
            labCfgVehicleSize.Text = clsConfig.VehicleLength.ToString() + " m";
            objCfgVehicleSize.ValueChanged += new RoutedPropertyChangedEventHandler<double>(objCfgVehicleSize_ValueChanged);

            // Reckless Effect
            objCfgRecklessEffect.Minimum = 1;
            objCfgRecklessEffect.Maximum = 50;
            objCfgRecklessEffect.SmallChange = 1;
            objCfgRecklessEffect.LargeChange = 5;
            objCfgRecklessEffect.Value = clsConfig.RecklessEffect * 100;
            labCfgRecklessEffect.Text = clsConfig.RecklessEffect * 100 + " %";
            objCfgRecklessEffect.ValueChanged += new RoutedPropertyChangedEventHandler<double>(objCfgRecklessEffect_ValueChanged);

            // Termination Timer
            objCfgTermTime.Minimum = 30;
            objCfgTermTime.Maximum = 600;
            objCfgTermTime.SmallChange = 1;
            objCfgTermTime.LargeChange = 60;
            objCfgTermTime.Value = clsConfig.TerminateTimer;
            labCfgTermTime.Text = clsConfig.TerminateTimer + " secs";
            if (clsConfig.TerminateTimer >= 30)
                cboxIsTimed.IsChecked = true;
            objCfgTermTime.ValueChanged += new RoutedPropertyChangedEventHandler<double>(objCfgTermTime_ValueChanged);

            // Super Time
            objCfgSuperTime.Minimum = 1;
            objCfgSuperTime.Maximum = 5;
            objCfgSuperTime.SmallChange = 1;
            objCfgSuperTime.LargeChange = 1;
            objCfgSuperTime.Value = clsConfig.SuperTime;
            labCfgSuperTime.Text = clsConfig.SuperTime + " x";
            objCfgSuperTime.ValueChanged += new RoutedPropertyChangedEventHandler<double>(objCfgSuperTime_ValueChanged);
        }

        private void objCfgMapScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            clsConfig.MeterPerPixel = (int)objCfgMapScale.Value;
            labCfgMapScale.Text = clsConfig.MeterPerPixel + " m";
        }

        private void objCfgReckRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            clsConfig.RecklessPercentage = (int)objCfgReckRate.Value;
            labCfgReckRate.Text = clsConfig.RecklessPercentage + " %";
        }

        private void objCfgNonConRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            clsConfig.NonCongestedPercent = objCfgNonConRate.Value / 100;
            labCfgNonConRate.Text = clsConfig.NonCongestedPercent * 100 + " %";
        }

        private void objCfgVehicleSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            clsConfig.VehicleLength = objCfgVehicleSize.Value;
            labCfgVehicleSize.Text = clsConfig.VehicleLength + " m";
        }

        private void objCfgRecklessEffect_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            clsConfig.RecklessEffect = objCfgRecklessEffect.Value / 100;
            labCfgRecklessEffect.Text = clsConfig.RecklessEffect * 100 + " %";
        }

        private void objCfgTermTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            clsConfig.TerminateTimer = (int)objCfgTermTime.Value;
            labCfgTermTime.Text = clsConfig.TerminateTimer + " secs";
        }

        private void objCfgSuperTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            clsConfig.SuperTime = (int)objCfgSuperTime.Value;
            labCfgSuperTime.Text = clsConfig.SuperTime + " x";
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        #endregion
    }
}
