using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISTools.Object
{
    class ShipJourney
    {
        public string Mmsi;//Mã định danh của tàu
        public string Vsnm;//Tên của tàu
        public string Type;//loại
        public string @class;
        public List<Coordinate> ListCoor;
      

        public ShipJourney(string mmsi, string vsnm, string type, List<Coordinate> listCoor)
        {
            Mmsi = mmsi;
            Vsnm = vsnm;
            Type = type;
            this.ListCoor = listCoor;
        }
        public ShipJourney(string mmsi, string vsnm, string type,string _class)
        {
            Mmsi = mmsi;
            Vsnm = vsnm;
            Type = type;
            @class = _class;
            ListCoor = new List<Coordinate>();

        }
     
        public ShipJourney()
        {
            ListCoor = new List<Coordinate>();
        }
        ~ShipJourney() { }
        public void addCoordinate(Coordinate coordinate)
        {
            ListCoor.Add(coordinate);
        }

        public string toString()
        {
            return Mmsi + " " + Vsnm + " " + Type;
        }
    }
}
