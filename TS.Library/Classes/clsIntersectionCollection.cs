using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace TS.Library
{
    public class clsIntersectionCollection : ObservableCollection<clsIntersection>
    {
        public List<Point> AllTLPoints = new List<Point>();

        public clsIntersection GetIntersection(Point p_objTLPosition)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].TrafficLightPosition == p_objTLPosition)
                    return this[i];
            }

            return null;
        }
    }
}
