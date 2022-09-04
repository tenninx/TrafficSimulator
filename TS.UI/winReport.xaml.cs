using System;
using System.Windows;
using TS.Library;

namespace TS.UI
{
    public partial class winReport : Window
    {
        clsReport m_objReportStats;

        public winReport(clsReport p_objReportStats)
        {
            InitializeComponent();

            m_objReportStats = p_objReportStats;
        }

        private void winReport_Loaded(object sender, RoutedEventArgs e)
        {
            labSimTime.Text = TimeSpan.FromSeconds(m_objReportStats.SecondsRun).ToString(@"hh\:mm\:ss") + " (" + TimeSpan.FromSeconds(m_objReportStats.RealSecondsRun).ToString(@"hh\:mm\:ss") + ")";
            labTotalVehicles.Text = m_objReportStats.TotalVehicles.ToString();
            labTotalReckless.Text = m_objReportStats.TotalReckless.ToString();
            labVehiclesSlowed.Text = m_objReportStats.SlowedVehicles.ToString();
            labCompleted.Text = m_objReportStats.TotalCompleted.ToString();
            labAvgJourney.Text = m_objReportStats.AverageJourneyTime.ToString(@"hh\:mm\:ss");
            labAvgSpeed.Text = m_objReportStats.AverageSpeed.ToString("0.00") + " km/h";
            labConRate.Text = m_objReportStats.CongestionRate.ToString("0.00 %");
            labTotalThreads.Text = m_objReportStats.TotalThreads.ToString("#,#") + " (" + (m_objReportStats.TotalThreads / m_objReportStats.SecondsRun).ToString("#.#") + " / sec)";
            labTotalEvents.Text = m_objReportStats.TotalEvents.ToString("#,#") + " (" + (m_objReportStats.TotalEvents / m_objReportStats.SecondsRun).ToString("#.#") + " / sec)";
        }
    }
}
