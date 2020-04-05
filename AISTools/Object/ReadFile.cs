using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace AISTools.Object
{
    class ReadFile
    {
        public static Dictionary<string, Dictionary<string, long>> FileDensity(string path)
        {
            Dictionary<string, Dictionary<string, long>> densityDict = new Dictionary<string, Dictionary<string, long>>();
          
            string jsonString = "";
            using(StreamReader sr = new StreamReader(path))
            {
                jsonString = sr.ReadToEnd();
            }
            densityDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, long>>>(jsonString);
            return densityDict;
        }
        public static Dictionary<string, ShipJourney> FileAll(string path)
        {
            Dictionary<string, ShipJourney> dict = new Dictionary<string, ShipJourney>();
            string jsonString = "";
            using (StreamReader sr = new StreamReader(path))
            {
                jsonString = sr.ReadToEnd();
            }
            dict = JsonConvert.DeserializeObject<Dictionary<string, ShipJourney>>(jsonString);
            return dict;
        }
    }
}
