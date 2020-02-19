using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeBoat_Final.Object
{
    class JsonToolBoat
    {
        public static Boat readFileJson(string pathInput)
        {
            string jsonString = File.ReadAllText(pathInput);
            JObject o = JObject.Parse(jsonString);
            Boat res = new Boat();
            res.Mmsi = o["mmsi"].ToString();
            res.Vsnm = o["vsnm"].ToString();
            res.Type = o["type"].ToString();
            FileInfo fInfo = new FileInfo(pathInput);
            var fFirstTime = fInfo.LastWriteTime.ToString("dd/MM/yyyy h:mm tt");
            res.ListCoor.Add(new Coordinate((float)o["lat"], (float)o["lng"],fFirstTime ));
            return res;
        }
        public static void writeFileJson(string pathOutput, Boat boat)
        {
            //ghi data ra file json

            //kiem tra su ton tai cua file
            if (!File.Exists(pathOutput))
            {
                FileStream fs = File.Create(pathOutput);
                fs.Close();
            }

            string jsonStringSource = File.ReadAllText(pathOutput);
            if (jsonStringSource == "")
            {
                //neu file chua co du lieu
                string jsonString = JsonConvert.SerializeObject(boat, Formatting.Indented);

                using (var tw = new StreamWriter(pathOutput))
                {
                    tw.WriteLine(jsonString);
                    tw.Close();
                }
            }
            else
            {
                //neu file da co du lieu
                List<Coordinate> listCoodinate = new List<Coordinate>();
                JObject jo = JObject.Parse(jsonStringSource);
                //JArray array = JArray.Parse(jo["ListCoor"].ToString());
                var arr = jo.GetValue("ListCoor") as JArray;
                foreach(Coordinate coor in boat.ListCoor)
                {
                    arr.Add(JObject.Parse(JsonConvert.SerializeObject(coor)));
                }
                jo["ListCoor"] = arr;

                string jsonString = JsonConvert.SerializeObject(jo, Formatting.Indented);
                using (var tw = new StreamWriter(pathOutput))
                {
                    tw.WriteLine(jsonString);
                    tw.Close();
                }


            }
          
            
           
        }
        public static void writeMultiObject(string pathOutput, Dictionary<string, Boat> listBoat)
        {
            foreach (var item in listBoat)
            {
                writeFileJson(pathOutput + item.Key + ".json", item.Value);
            }
        }
    }
}
