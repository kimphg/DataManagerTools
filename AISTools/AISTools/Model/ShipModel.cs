
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
        public void Insert(string mmsi,string vsnm,int type,string _class)
        {
            string query = "INSERT INTO SHIP (MMSI,VSNM,TYPE,CLASS)";
            query += " VALUES (@mmsi,@vsnm,@type,@class)";

            using (SqlConnection connection = new SqlConnection(ConnectionString.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@mmsi", mmsi);
                    command.Parameters.AddWithValue("@vsnm", vsnm);
                    command.Parameters.AddWithValue("@type", type);
                    command.Parameters.AddWithValue("@class", _class);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
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
