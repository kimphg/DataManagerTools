
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISTools.Model
{
    class ShipModel
    {
        public ShipModel() {
        }
        public void Insert(DataRow dr)
        {

            try
            {
                //insert if doesnt exist
                string query = "INSERT INTO SHIP_LIST (MMSI,VSNM,TYPE,CLASS,LAT,LON,SOG,COG,TIME)";
                query += " VALUES (@mmsi,@vsnm,@type,@class,@lat,@lon,@sog,@cog,@time)";

                using (SqlConnection connection = new SqlConnection(ConnectionString.connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@mmsi", dr["MMSI"]);
                        command.Parameters.AddWithValue("@vsnm", dr["VSNM"]);
                        command.Parameters.AddWithValue("@type", dr["TYPE"]);
                        command.Parameters.AddWithValue("@class", dr["CLASS"]);
                        command.Parameters.AddWithValue("@lat", dr["LAT"]);
                        command.Parameters.AddWithValue("@lon", dr["LNG"]);
                        command.Parameters.AddWithValue("@sog", dr["SOG"]);
                        command.Parameters.AddWithValue("@cog", dr["COG"]);//(long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
                        long time = (long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
                        command.Parameters.AddWithValue("@time",time );
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                
            }
            catch (Exception ex)
            {
                string query = "UPDATE SHIP_LIST ";
                query += " SET LAT = @lat, LON = @lon, SOG = @sog, COG = @cog, TIME = @time ";
                query += "where [MMSI] LIKE '" + dr["MMSI"] + "' AND [VSNM] LIKE '" + dr["VSNM"] + "'";
                using (SqlConnection connection = new SqlConnection(ConnectionString.connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@lat", dr["LAT"]);
                        command.Parameters.AddWithValue("@lon", dr["LNG"]);
                        command.Parameters.AddWithValue("@sog", dr["SOG"]);
                        command.Parameters.AddWithValue("@cog", dr["COG"]);//(long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
                        command.Parameters.AddWithValue("@time", (long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            /*
            bool shipExist = false;
            
            string query1 = $"select TOP 1 [MMSI],[VSNM] FROM SHIP_LIST where [MMSI] LIKE '" + dr["MMSI"] + "' AND [VSNM] LIKE '" + dr["VSNM"] +"'";
            var table = new DataTable();
            using (var adapter = new SqlDataAdapter(query1, ConnectionString.connectionString))
            {

                adapter.Fill(table);
            };
            shipExist = (table.Rows.Count > 0);
            
            if (shipExist)
            {
                string query = "UPDATE SHIP_LIST ";
                query += " SET LAT = @lat, LON = @lon, SOG = @sog, COG = @cog, TIME = @time ";
                query += "where [MMSI] LIKE '" + dr["MMSI"] + "' AND [VSNM] LIKE '" + dr["VSNM"] + "'";
                using (SqlConnection connection = new SqlConnection(ConnectionString.connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                       
                        command.Parameters.AddWithValue("@lat", dr["LAT"]);
                        command.Parameters.AddWithValue("@lon", dr["LNG"]);
                        command.Parameters.AddWithValue("@sog", dr["SOG"]);
                        command.Parameters.AddWithValue("@cog", dr["COG"]);//(long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
                        command.Parameters.AddWithValue("@time", (long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                string query = "INSERT INTO SHIP_LIST (MMSI,VSNM,TYPE,CLASS,LAT,LON,SOG,COG,TIME)";
                query += " VALUES (@mmsi,@vsnm,@type,@class,@lat,@lon,@sog,@cog,@time)";

                using (SqlConnection connection = new SqlConnection(ConnectionString.connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@mmsi", dr["MMSI"]);
                        command.Parameters.AddWithValue("@vsnm", dr["VSNM"]);
                        command.Parameters.AddWithValue("@type", dr["TYPE"]);
                        command.Parameters.AddWithValue("@class", dr["CLASS"]);
                        command.Parameters.AddWithValue("@lat", dr["LAT"]);
                        command.Parameters.AddWithValue("@lon", dr["LNG"]);
                        command.Parameters.AddWithValue("@sog", dr["SOG"]);
                        command.Parameters.AddWithValue("@cog", dr["COG"]);//(long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
                        command.Parameters.AddWithValue("@time", (long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }*/
        }
        public HashSet<string> getAll()
        {
            var table = new DataTable();
            using (var adapter = new SqlDataAdapter($"SELECT MMSI FROM SHIP", ConnectionString.connectionString))
            {
                adapter.Fill(table);
            };
            List<string> list = table.AsEnumerable()
                                   .Select(r => r.Field<string>("MMSI"))
                                   .ToList();
            return new HashSet<string>(list);
        }  

    }
}
