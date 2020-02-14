using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ReadFileJsonFirst.Object
{
    class Density
    {
        private string _nameFile;


        //public string _nameFile { get => _nameFile; set => _nameFile = value; }

        public Density(string nameFile)
        {
            _nameFile = nameFile;
        }
        ~Density() { }  
        public void setDensity(Coordinate coordinate)
        {
            //tạo một dictionary để lưu trữ
            Dictionary<string, long> data = new Dictionary<string, long>();
            string jsonString = File.ReadAllText(this._nameFile);
            if (jsonString == "")
            {
                data.Add(PointfToString(coordinate.getHashCodeChildMap()), 1);
            }
            else
            {
                JObject o = JObject.Parse(jsonString);
                //đưa vào dictionary 
                data = JsonConvert.DeserializeObject< Dictionary<string, long>>(jsonString);
                //kiểm tra sự tồn tại của đối tượng
                string temp = PointfToString(coordinate.getHashCodeChildMap());
                if (data.ContainsKey(temp))
                {
                    long value = data[temp];
                    value++;
                    data[temp] = value;
                }
                else
                {
                    //tạo mới một điểm và ghi lại mật độ 
                    data.Add(PointfToString(coordinate.getHashCodeChildMap()), 1);
                }   
            }
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            //ghi vao file 
            JsonTools.writeFile(_nameFile, json);


        }
        private string PointfToString(PointF coor)
        {
            string str = "lat : {0}, lng : {1}";
            return string.Format(str, coor.X, coor.Y);
        }
        private PointF StringToPointF(string key)
        {
            var doubleArray = Regex.Split(key, @"[^0-9\.]+").Where(c => c != "." && c.Trim() != "").ToList();
            return new PointF(float.Parse(doubleArray[0]), float.Parse(doubleArray[1]));
        }
        public void removeDensity()
        {

        }
       

    }
}
