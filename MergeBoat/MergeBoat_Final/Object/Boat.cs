using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeBoat_Final.Object
{
    class Boat
    {
        private string _mmsi;//Mã định danh của tàu
        private string _vsnm;//Tên của tàu
        private string _type;//loại
        private List<Coordinate> listCoor;

        public string Mmsi { get;set; }
        public string Vsnm { get;set;}
        public List<Coordinate> ListCoor { get; set; }
        public string Type { get; set; }

        public Boat(string mmsi, string vsnm,string type, List<Coordinate> listCoor)
        {
            Mmsi = mmsi;
            Vsnm = vsnm;
            Type = type;
            this.ListCoor = listCoor;
        }
        public Boat()
        {
            ListCoor = new List<Coordinate>();
        }

        public string toString()
        {
            return Mmsi + " " + Vsnm + " " + Type;
        }

    }
}
