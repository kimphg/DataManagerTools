using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
namespace ReadFileJsonFirst.Object
{
    /*
     class để xử lý 1 file tàu đầu vào để đưa ra mật độ 
     1 file tàu cung cấp cho được 1 danh sách cách vị trí của nó.
         */
    public struct DensityPixel
    {
        // phần nguyên của tọa độ 
        public  Point parentCode;
        //phần thập phân
        public  Point childCode;
        public DensityPixel(Point parent, Point child)
        {
            parentCode = parent;
            childCode = child;
        }
    }
    
    class HandlingCoordinates
    {

        private string nameFileSource;

        public HandlingCoordinates(string nameFileSource )
        {
            this.nameFileSource = nameFileSource;
           // DirectorySearchFile(nameFolderDestination);
        }
        ~HandlingCoordinates() { }
        List<string> files = new List<string>();
        public void checkCoordinate(Dictionary<Int32, Dictionary<Int32, Int32>> densityMap)//densityMap là dictionary trong dictionary, có 2 key, 1 là lat_x1000(dòng), 2 là lon_x1000(cột)
        {
            List<Coordinate> coordinates = new List<Coordinate>();
            coordinates = JsonTools.readFileJsonBoat(nameFileSource);
            foreach(Coordinate coor in coordinates)
            {
                // lat_x1000 là phần nguyên của 1000 lần vĩ độ, như vậy độ phân giải của lat_x1000 là 10^-1 độ
                Int32 lat_x1000 = Convert.ToInt32(Math.Round(coor.Lat * 1000));
                // Xác định lon_x1000 tương tự như lat_x1000
                Int32 lon_x1000 = Convert.ToInt32(Math.Round(coor.Lng * 1000));
                // kiểm tra xem dòng lat_x1000 có trong densityMap hay không
                if (densityMap.ContainsKey(lat_x1000))
                {
                    // kiểm tra xem pixel lon_x1000 có trong dòng lat_x1000 của densityMap hay không
                    if (densityMap[lat_x1000].ContainsKey(lon_x1000))
                    {
                        // nếu đã tồn tại pixel, tăng 1 đơn vị cho nó
                        Int32 value = densityMap[lat_x1000][lon_x1000];
                        if (value < Int32.MaxValue) value++;
                        densityMap[lat_x1000][lon_x1000] = value;
                    }
                    else// tạo mới nếu chưa tồn tại
                    {
                        densityMap[lat_x1000].Add(lon_x1000, 1);
                    }
                }
                else// tạo mới nếu chưa tồn tại
                {

                    Dictionary<Int32, Int32> longDic = new Dictionary<Int32, Int32>();
                    longDic.Add(lon_x1000, 1);
                    densityMap.Add(lat_x1000, longDic);
                }
            }
        }
        private void DirectorySearchFile(string sDir)
        {
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    DirectorySearchFile(d);
                }
            }
            catch (System.Exception excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }



    }
    
}
