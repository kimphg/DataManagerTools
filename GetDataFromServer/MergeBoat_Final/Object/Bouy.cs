using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetDataFromServer.Object
{
    class Bouy
    {
        public string Mmsi;//Mã định danh
        public string Vsnm;//Tên 
        public Coordinate Coordinate;
        public Bouy()
        {

        }

        public Bouy(string mmsi, string vsnm, Coordinate coordinate)
        {
            Mmsi = mmsi;
            Vsnm = vsnm;
            Coordinate = coordinate;
        }
    }
}
