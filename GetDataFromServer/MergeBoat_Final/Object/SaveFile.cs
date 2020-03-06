using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetDataFromServer.Object
{
    class SaveFile
    {
        public static void toBouy(Dictionary<string, ShipJourney> list, string path)
        {
            List<Bouy> listBouys = new List<Bouy>();
            foreach (var item in list.Values)
            {
                if (item.Type == "-2")
                {
                    listBouys.Add(new Bouy(item.Mmsi, item.Vsnm, item.ListCoor[0]));
                }

            }

            string jsonString = JsonConvert.SerializeObject(listBouys, Formatting.Indented);
            string pathfile = path + "\\bouy_" + getNowDate() + ".json";
            using (var tw = new StreamWriter(pathfile))
            {
                tw.WriteLine(jsonString);
                tw.Close();
            }
        }
        public static void toDensity(Dictionary<string, ShipJourney> list, string path)
        {
            Dictionary<string, Dictionary<string, long>> dictDensity = new Dictionary<string, Dictionary<string, long>>();
            foreach (var item in list.Values)
            {
                if (item.Type != "-2" || item.Type != "-3")
                {
                    foreach (var coor in item.ListCoor)
                    {
                        if (dictDensity.ContainsKey(coor.getLat()))
                        {
                            Dictionary<string, long> temp = new Dictionary<string, long>();
                            temp = dictDensity[coor.getLat()];
                            //check new lng
                            if (temp.ContainsKey(coor.getLng()))
                            {
                                long num = temp[coor.getLng()];
                                num++;
                                temp[coor.getLng()] = num;
                            }
                            else
                            {
                                //add new lng
                                temp.Add(coor.getLng(), 1);
                            }
                            dictDensity[coor.getLat()] = temp;

                        }
                        else
                        {
                            Dictionary<string, long> temp = new Dictionary<string, long>();
                            temp.Add(coor.getLng(), 1);
                            //add new lat
                            dictDensity.Add(coor.getLat(), temp);
                        }
                    }
                }

            }

            string jsonString = JsonConvert.SerializeObject(dictDensity, Formatting.Indented);
            using (var tw = new StreamWriter(path + @"\density_" + getNowDate() + ".json"))
            {
                tw.WriteLine(jsonString);
                tw.Close();
            }
        }

        public static void toLeadingMark(Dictionary<string, ShipJourney> list,string path)
        {
            List<Bouy> listBouys = new List<Bouy>();
            foreach (var item in list.Values)
            {
                if (item.Type == "-3")
                {
                    listBouys.Add(new Bouy(item.Mmsi, item.Vsnm, item.ListCoor[0]));
                }

            }

            string jsonString = JsonConvert.SerializeObject(listBouys, Formatting.Indented);
            string pathfile = path + "\\leadingmark_" + getNowDate() + ".json";
            using (var tw = new StreamWriter(pathfile))
            {
                tw.WriteLine(jsonString);
                tw.Close();
            }
        }
        public static void toShipWreck(Dictionary<string, ShipJourney> list, string path)
        {
            List<Bouy> listBouys = new List<Bouy>();
            foreach (var item in list.Values)
            {
                if (item.Vsnm.Contains("wreck")|| item.Vsnm.Contains("WRECK"))
                {
                    listBouys.Add(new Bouy(item.Mmsi, item.Vsnm, item.ListCoor[0]));
                }

            }

            string jsonString = JsonConvert.SerializeObject(listBouys, Formatting.Indented);
            string pathfile = path + "\\shipWreck_" + getNowDate() + ".json";
            using (var tw = new StreamWriter(pathfile))
            {
                tw.WriteLine(jsonString);
                tw.Close();
            }
        }
        public static void toSaveAll(Dictionary<string, ShipJourney> list, string path)
        {
            string jsonString = JsonConvert.SerializeObject(list, Formatting.Indented);
            string pathfile = path + "\\total_ship_" + getNowDate() + ".json";
            using (var tw = new StreamWriter(pathfile))
            {
                tw.WriteLine(jsonString);
                tw.Close();
            }
        }
        private static string getNowDate()
        {
            return DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day;
        }
    }
}
