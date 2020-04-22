
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISTools.Model
{
    class ShipJourneyModel
    {
      
        public ShipJourneyModel()
        {
           
        }
       
        public void Insert(Dictionary<string, Object.ShipJourney> dict)
        {
            BulkCopy(dict);
        }
        public void BulkCopy(Dictionary<string, Object.ShipJourney> dict)
        {
            var table = new DataTable();
            using (var adapter = new SqlDataAdapter($"SELECT TOP 0 * FROM ShipJourney", ConnectionString.connectionString))
            {
                adapter.Fill(table);
            };
            foreach (var item in dict.Values.ToList())
            {
                foreach (var coor in item.ListCoor)
                {
                   
                    var row = table.NewRow();
                    row["MMSI"] = item.Mmsi;
                    row["LAT"] = coor.lat;
                    row["LNG"] = coor.lng;
                    row["SOG"] = coor.sog;
                    row["COG"] = coor.cog;
                    row["TIME"] = coor.time;
                    row["LOST"] = coor.lost;
                    row["MIN"] = coor.min;
                    table.Rows.Add(row);
                }
            }
            try
            {
                using (var bulk = new SqlBulkCopy(ConnectionString.connectionString))
                {
                    bulk.DestinationTableName = "SHIPJOURNEY";
                    bulk.WriteToServer(table);
                }
            }
            catch (Exception e)
            {
            }
            table.Clear();
        }
    }
}
