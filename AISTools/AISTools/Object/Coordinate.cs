using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISTools.Object
{
    class Coordinate
    {
        //private float lat;
        //private float lng;
        //private string time;
        public float lat;
        public float lng;
        public string time;
        public float sog;
        public float cog;
        public int lost;
        public int min;

        public Coordinate(float lat, float lng, string time)
        {
            this.lat = lat;
            this.lng = lng;
            this.time = time;
        }
        public Coordinate(float lat, float lng, string time,float sog,float cog,int lost,int min)
        {
            this.lat = lat;
            this.lng = lng;
            this.time = time;
            this.cog = cog;
            this.sog = sog;
            this.lost = lost;
            this.min = min;
        }
        public Coordinate()
        {
        }
        ~Coordinate() { }

        public override string ToString()
        {
            string str = "'lat' : {0}, 'lng' : {1}, 'time_create' : {2}";
            return string.Format(str, lat, lng, time);
        }
        public string getLat()
        {
            return ((int)(lat * 1000)).ToString();
        }
        public string getLng()
        {
            return ((int)(lng * 1000)).ToString();
        }
    }
}
