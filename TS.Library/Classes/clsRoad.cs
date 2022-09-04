namespace TS.Library
{
    public class clsRoad
    {
        public int RoadID { get; set; }
        public string RoadName { get; set; }
        public clsChunkCollection ChunkCollection { get; set; }

        public double SpeedLimit { get; set; }

        public clsRoad()
        {
        }
    }
}
