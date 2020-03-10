using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AISTools.Object
{
    class RequestShip
    {
        private string jsonString = "";
        private string type = "";
        private int total;
        public RequestShip() { }
        ~RequestShip() { }
        public RequestShip(string url)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    var htmlData = client.DownloadData(url);
                    var htmlCode = Encoding.UTF8.GetString(htmlData);
                    jsonString = htmlCode;
                }
                catch (WebException e)
                {
                    MessageBox.Show(e.StackTrace);
                }

            }
        }
        public string getType()
        {
            try
            {
                JObject o = JObject.Parse(jsonString);
                string t = o["type"].ToString();
                return t;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
            return "null";
        }
        public int getTotal()
        {
            try
            {
                JObject o = JObject.Parse(jsonString);
                string t = o["Total"].ToString();
                return Convert.ToInt32(t);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
            return 0;
        }
        public List<Ship> getShip()
        {
            JObject o = JObject.Parse(jsonString);
            List<Ship> listShip = new List<Ship>();
            JArray array = JArray.Parse(o["ships"].ToString());
            foreach (JObject obj in array.Children<JObject>())
            {
                Ship ship = new Ship();
                ship = getShip(obj);
                listShip.Add(ship);

            }
            return listShip;

        }
        public DataTable getDataTableShip()
        {
            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("mmsi");
            dt.Columns.Add("vsnm");
            dt.Columns.Add("lat");
            dt.Columns.Add("lng");
            dt.Columns.Add("sog");
            dt.Columns.Add("cog");
            dt.Columns.Add("lost");
            dt.Columns.Add("min");
            dt.Columns.Add("type");
            dt.Columns.Add("idLog");
            dt.Columns.Add("utcPos");
            dt.Columns.Add("class");
            JObject o = JObject.Parse(jsonString);
            JArray array = JArray.Parse(o["ships"].ToString());
            foreach (JObject obj in array.Children<JObject>())
            {
                Ship ship = new Ship();
                ship = getShip(obj);
                DataRow dr = dt.NewRow();
                dr["mmsi"] = ship.mmsi;
                dr["vsnm"] = ship.vsnm;
                dr["lat"] = ship.lat;
                dr["lng"] = ship.lng;
                dr["sog"] = ship.sog;
                dr["cog"] = ship.cog;
                dr["lost"] = ship.lost;
                dr["min"] = ship.min;
                dr["type"] = ship.type;
                dr["idLog"] = ship.idLog;
                dr["utcPos"] = ship.utcPos;
                dr["class"] = ship.S_class;
                dt.Rows.Add(dr);

            }
            return dt;

        }
        //public Ship(string mmsi, string vsnm, float lng, float lat, string sog, string cog, string lost, int min, string type, int idLog, int utcPos, string s_class)
        private Ship getShip(JObject o)
        {
            Ship ship = new Ship();
            ship.mmsi = o["mmsi"].ToString();
            ship.vsnm = o["vsnm"].ToString();
            ship.lat = float.Parse(o["lat"].ToString());
            ship.lng = float.Parse(o["lng"].ToString());
            ship.sog = o["sog"].ToString();
            ship.cog = o["cog"].ToString();
            ship.lost = o["lost"].ToString();
            ship.min = Convert.ToInt32(o["min"].ToString());
            ship.type = o["type"].ToString();
            ship.idLog = Convert.ToInt32(o["idLog"].ToString());
            ship.utcPos = Convert.ToInt32(o["utcPos"].ToString());
            ship.S_class = o["class"].ToString();
            return ship;
        }
    }
}
