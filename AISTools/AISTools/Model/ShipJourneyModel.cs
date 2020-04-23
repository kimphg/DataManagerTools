
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
       
        public int Insert(Dictionary<string, Object.ShipJourney> dict, bool onlymove)
        {
            return BulkCopy(dict, onlymove);
        }
        public int BulkCopy(Dictionary<string, Object.ShipJourney> dict,bool onlymove)
        {
            int count = 0;
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
                    if (onlymove && coor.sog < 1.0) continue;
                    row["COG"] = coor.cog;
                    row["TIME"] = coor.time;
                    row["LOST"] = coor.lost;
                    row["MIN"] = coor.min;
                    count++;
                    table.Rows.Add(row);
                }
            }
            try
            {
                using (var bulk = new SqlBulkCopy(ConnectionString.connectionString))
                {
                    bulk.BulkCopyTimeout = 120;
                    bulk.DestinationTableName = "SHIPJOURNEY";
                    bulk.WriteToServer(table);
                }
            }
            catch (Exception e)
            {
                try//again
                {
                    using (var bulk = new SqlBulkCopy(ConnectionString.connectionString))
                    {
                        bulk.BulkCopyTimeout = 240;
                        bulk.DestinationTableName = "SHIPJOURNEY";
                        bulk.WriteToServer(table);
                    }
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            table.Clear();
            return count;
        }
        
    }
}
