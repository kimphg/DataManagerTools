using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFileJsonFirst.Object
{
    class Coordinate
    {

        public Coordinate(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }
        public Coordinate(){}
        ~Coordinate() { }

        public double Lat ;
        public double Lng;

        //dung để sử dụng để kiểm tra toạ độ đó nằm ở đâu trong bản đồ.

        //bản đồ cha
        public Point getHashCodeParentMap()
        {
            return new Point((int)Lat, (int)Lng);
        }

        //bản đồ con
        public Point getHashCodeChildMap()
        {
            int newLat =Convert.ToInt32( ((long)(Lat * 1000) + 0.5) );
            int newLng =Convert.ToInt32( ((long)(Lng * 1000) + 0.5) );
            return new Point(newLat, newLng);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
