using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadFileJsonFirst.Object
{
    class JsonTools
    {
        public static List<Coordinate> readFileJsonBoat(string path)
        {
            List<Coordinate> listCoodinate = new List<Coordinate>();
            string jsonString = File.ReadAllText(path);
            JObject jo = JObject.Parse(jsonString);
            JArray array = JArray.Parse(jo["ListCoor"].ToString());
            foreach (JObject obj in array.Children<JObject>())
            {
                //foreach (JProperty singleProp in obj.Properties())
                //{
                //    string parent = singleProp.Parent.ToString();
                //    Coordinate coor = new Coordinate();
                //    coor = getCoordinate(parent);
                //    listCoodinate.Add(coor);

                //    //Do something with name and value
                ////    MessageBox.Show(coor.getHashCodeChildMap().ToString());

                //}
                Coordinate coor = new Coordinate();
                coor = getCoordinate(obj);
                listCoodinate.Add(coor);

            }
            return listCoodinate;
        }
        private static Coordinate getCoordinate(JObject jsonPoint)

        {
            Coordinate coor = new Coordinate();
          //  JObject jsonPoint = JObject.Parse(jsonString);
            float x = float.Parse(jsonPoint["lat"].ToString());
            float y = float.Parse(jsonPoint["lng"].ToString());
            coor.Lat = x;
            coor.Lng = y;
            return coor;
        }
       
       
        public static void writeFile(string path, string data)
        {
            char[] dataLines = data.ToArray();
            using (var tw = new StreamWriter(path))
            {
                foreach (var line in dataLines)
                {
                    tw.Write(line);
                    tw.Flush();
                }
                tw.Close();
            }
        }
        

    }
}
