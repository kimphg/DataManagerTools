using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetDataFromServer.Object
{
    class Ship
    {
        public string mmsi;
        public string vsnm;
        public float lng;
        public float lat;
        public string sog;
        public string cog;
        public string lost;
        public int min;
        public string type;
        public int idLog;
        public int utcPos;
        public string S_class;

        public Ship(string mmsi, string vsnm, float lng, float lat, string sog, string cog, string lost, int min, string type, int idLog, int utcPos, string s_class)
        {
            this.mmsi = mmsi;
            this.vsnm = vsnm;
            this.lng = lng;
            this.lat = lat;
            this.sog = sog;
            this.cog = cog;
            this.lost = lost;
            this.min = min;
            this.type = type;
            this.idLog = idLog;
            this.utcPos = utcPos;
            S_class = s_class;
        }

        public Ship(){}
        ~Ship() { }
    }
}
