namespace TS.Library
{
    public class clsConfig
    {
        public static double AdjustX = 10;
        public static double AdjustY = 10;
        public static double ChunkThickness = 10;
        public static int TrafficLightSize = 30;
        public static int AccelerateTime = 1000;    // in Milliseconds
        public static double AccelerationLength = 10;        // Meters need to accelerate/decelerate every 10 km/h
        public static double MaxAllowableCongestion = 0.7;

        // For Config shown in UI
        public static int MeterPerPixel = 2;
        public static int TrafficLightTimer = 10;   // in Seconds
        public static int RecklessPercentage = 2;   // as Percentage
        public static double NonCongestedPercent = 0.3;
        public static double VehicleLength = 5;
        public static double RecklessEffect = 0.1;  // % slowed per reckless drivers in chunk
        public static int TerminateTimer = 0;       // in Seconds (0 means disabled)
        public static int SuperTime = 1;            // 1 second is equivalent to how many second in simulation
    }
}
