using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISTools
{
    class GlobalVar
    {
        public static string urlAll = "http://quanlytau.vishipel.vn/Vishipel.VTS/TrackingHandler.ashx?cmd=live&range=101.0%2C7.0%2C125.5%2C23.5&areaW=0.055579335027628574%2C0.056304931640625&zoom=10&c=0&l=all";
        public static string urlShip = "http://quanlytau.vishipel.vn/Vishipel.VTS/ShipInforPage.aspx?mmsi=";
        public static string pathSaveFileBouy = "";
        public static string pathSaveFileDensity = "";
        public static string pathSaveFileLeadingMark = "";
        public static string pathSaveFileWreck = "";
        public static HashSet<string> hashMMSI = new HashSet<string>();
        public static int TIME_REQUEST = 1000 * 60 * 5;
        public static int TIME_SAVE = 1000 * 60 * 20;
        public static int TIME_UPDATE = 1000;
    }
}
