namespace TS.Library
{
    public static class clsGlobalStorage
    {
        public static clsChunkCollection AllChunks = new clsChunkCollection();
        public static clsVehicleCollection AllVehicles = new clsVehicleCollection();
        public static clsVehicleCollection CompletedVehicles = new clsVehicleCollection();
        public static clsVehicleCollection SlowedVehicles = new clsVehicleCollection();
        public static clsIntersectionCollection AllIntersections = new clsIntersectionCollection();
        public static bool IsRunning = false;
        public static bool BlinkReckless = false;
        public static bool HighlightSelection = true;
        public static long TotalThreads = 0;
        public static long TotalEvents = 0;

        private static object m_objVehicleLocker = new object();
        private static object m_objChunkLocker = new object();

        public static void InsertVehicle(clsVehicle p_objVehicle)
        {
            lock (m_objVehicleLocker)
                AllVehicles.Add(p_objVehicle);
        }

        public static void RemoveVehicle(clsVehicle p_objVehicle)
        {
            lock (m_objVehicleLocker)
                AllVehicles.Remove(p_objVehicle);
        }

        public static void InsertChunk(clsChunk p_objChunk)
        {
            lock (m_objChunkLocker)
                AllChunks.Add(p_objChunk);
        }

        public static void RemoveChunk(clsChunk p_objChunk)
        {
            lock (m_objChunkLocker)
                AllChunks.Remove(p_objChunk);
        }
    }
}
