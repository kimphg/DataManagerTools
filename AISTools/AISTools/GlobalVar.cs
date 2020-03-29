using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISTools
{
    class GlobalVar
    {
        public static string url = "http://quanlytau.vishipel.vn/Vishipel.VTS/TrackingHandler.ashx?cmd=live&range=101.042863%2C7.185843%2C121.430319%2C23.379662&areaW=0.055579335027628574%2C0.056304931640625&zoom=10&c=0&l=all&fbclid=IwAR1Y-3o65ve1rvapQv6e8rIZXZQqtJ3hQl1nhdeKm1V_AnpX-W5KaKcAWUY";
        public static string pathSaveFileBouy = "";
        public static string pathSaveFileDensity = "";
        public static string pathSaveFileLeadingMark = "";
        public static string pathSaveFileWreck = "";
        public static HashSet<string> hashMMSI = new HashSet<string>();
        public static int TIME_REQUEST = 1000 * 60 * 10;
        public static int TIME_SAVE = 1000 * 60 * 60;
    }
}
