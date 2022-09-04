using System;

namespace TS.Library
{
    public class clsReport
    {
        public int SecondsRun { get; set; }
        public int RealSecondsRun { get; set; }
        public int TotalVehicles { get; set; }
        public int TotalReckless { get; set; }
        public int SlowedVehicles { get; set; }
        public int TotalCompleted { get; set; }
        public TimeSpan AverageJourneyTime { get; set; }
        public double AverageSpeed { get; set; }
        public double CongestionRate { get; set; }
        public long TotalThreads { get; set; }
        public long TotalEvents { get; set; }
    }
}
