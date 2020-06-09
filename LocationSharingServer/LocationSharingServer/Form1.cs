using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocationSharingServer
{
    using System;
    using System.Data.SqlClient;
    using System.Threading;


    public partial class Form1 : Form
    {
        //public static string connectionString = @"Data Source=WIN-CS49MK82IQN\SQLEXPRESS;Initial Catalog=seamap;Integrated Security=True";//
        
        Thread thread1;
        public Form1()
        {
            InitializeComponent();
            thread1 = new Thread(ServerListener.Run);
            thread1.Start();
            //dataGridView1.DataSource = new BindingSource(new DictionaryAdapter(ServerListener.clientList), "");
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ServerListener.log.Count() > 0)

            {
                richTextBox1.Text = ServerListener.log;
                ServerListener.log = "";
            }
            //dataGridView1.Update();
            
        }
        private void  OnClosed()
        {
            
            
        }
        private void FormClosing(Object sender, FormClosingEventArgs e)
        {
            ServerListener.toStop = true;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
       
        private void timer2_Tick(object sender, EventArgs e)
        {
           
        }
    }
    public struct LocationClient
    {
        public IPEndPoint mIP;
        public string dev;
        public int ID;
        public float mLat, mLon;
        //public long mLastTimeSent;
        public long mLastTimeRec;
        public int msgCount;
    }

    public class ServerListener
    {
        private static bool isShiplistOutdated = true;
        private static Timer timer20s;
        static IPEndPoint remoteEP;
        static IPEndPoint localUser;
        static UdpClient udpServer;
        public static string log = "";
        internal static bool toStop = false;
        public static Dictionary<int, LocationClient> clientList = new Dictionary<int, LocationClient>();
        public static string connectionString = @"Data Source=WIN-CS49MK82IQN\SQLEXPRESS;Initial Catalog=seamap;Integrated Security=True";
        private static DataTable shipList = new DataTable();
        private static int FRAME_HEADER_LEN = 2;
        public static void Run()
        {
            timer20s = new Timer(timer20sTick, new AutoResetEvent(true), 0, 20000);

            try
            {
                udpServer = new UdpClient(50000);
                remoteEP = new IPEndPoint(IPAddress.Any, 0);
                localUser = new IPEndPoint(IPAddress.Parse("192.168.0.70"), 50000);
                log += "server start ok";
                while (true)
                {
                    try
                    {
                        Thread.Sleep(100);
                        if (toStop) break;
                        byte[] frame = udpServer.Receive(ref remoteEP); // listen to packet
                        udpServer.Send(frame, frame.Count(), localUser);
                        if (frame.Count() < FRAME_HEADER_LEN) continue;
                        int framLen = frame.Count() - FRAME_HEADER_LEN;
                        byte[] data = new byte[framLen];
                        Array.Copy(frame, 2, data, 0, framLen);
                        if (frame[0] == 0x00 & frame[1] == 0x00 & frame.Length == 18)//location report
                        {
                            Array.Reverse(data, 0, data.Length);
                            LocationClient newclient = new LocationClient();
                            newclient.mIP = remoteEP;
                            newclient.mLastTimeRec = (long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
                            newclient.mLon = System.BitConverter.ToSingle(data, 4);
                            newclient.mLat = System.BitConverter.ToSingle(data, 0);
                            newclient.ID = System.BitConverter.ToInt32(data, 8);
                            newclient.msgCount = 1;
                            newclient.dev = "";
                            if (clientList.ContainsKey(newclient.ID))
                            {
                                newclient.msgCount = clientList[newclient.ID].msgCount + 1;
                                newclient.dev = clientList[newclient.ID].dev;
                            }
                            while (newclient.dev.Length > 0)
                            {
                                if (newclient.dev.ElementAt(0) == ' ')
                                    newclient.dev = newclient.dev.Remove(0,1);
                                else break;
                            }
                            if (newclient.dev.Length < 1)
                            {
                                requestDevName(remoteEP, newclient.ID);
                            }
                            
                            AddLocationClient(newclient);
                            sendResToClient(remoteEP, newclient.mLat, newclient.mLon);
                        }
                        else if (frame[0] == 0x5a & frame[1] == 0xa5)//device name report
                        {
                            LocationClient newclient = new LocationClient();
                            if (data.Count() > 30)
                            {
                                byte[] name = new byte[30];
                                Array.Copy(data, name, 30);
                                newclient.dev = System.Text.Encoding.UTF8.GetString(name);
                            }
                            else newclient.dev = System.Text.Encoding.UTF8.GetString(data);
                            newclient.mIP = remoteEP;
                            newclient.mLastTimeRec = (long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
                            //check device same ID
                            int devExist = 0;
                            foreach (var entry in clientList)//check if device at that ip already exist
                            {
                                if (entry.Value.mIP.Equals(newclient.mIP))
                                {

                                    devExist = entry.Key;
                                    newclient.mLat = entry.Value.mLat;
                                    newclient.mLon = entry.Value.mLon;
                                    newclient.ID = entry.Key;
                                    break;
                                }

                            }
                            if (devExist != 0)
                            {
                                clientList.Remove(devExist);
                                clientList.Add(devExist, newclient);
                            }
                            //if (!clientList.ContainsKey(newclient.ID))
                              //  clientList.Add(newclient.ID, newclient);
                            /*Byte[] dataOut = new Byte[6];
                            dataOut[0] = 0x5a;
                            dataOut[1] = 0xa5;
                            dataOut[2] = (byte)(newclient.ID >> 24);
                            dataOut[3] = (byte)(newclient.ID >> 16);
                            dataOut[4] = (byte)(newclient.ID >> 8);
                            dataOut[5] = (byte)(newclient.ID);
                            udpServer.Send(dataOut, 6, remoteEP); // reply back
                            udpServer.Send(dataOut, 6, remoteEP); // reply back
                            udpServer.Send(dataOut, 6, remoteEP); // reply back
                            //AddLocationClient(newclient);
                            //sendResToClient(remoteEP);*/
                        }
                        log = "";
                        foreach (var entry in clientList)
                        {
                            TimeSpan time = TimeSpan.FromMilliseconds(entry.Value.mLastTimeRec);
                            DateTime timeDate = new DateTime(1970, 1, 1) + time;
                            string newline = "";
                            newline += entry.Value.mIP.ToString();
                            while (newline.Length < 20) newline += " ";
                            newline += " \t";
                            //newline += " ";
                            newline += entry.Value.mLat.ToString("0.0000");
                            newline += " ";
                            newline += entry.Value.mLon.ToString("0.0000");
                            newline += " \t";
                            newline += entry.Value.msgCount.ToString("0000");
                            newline += " \t";
                            
                            newline += timeDate.ToString() + "\t";
                            newline += entry.Value.ID.ToString("00000000000");
                            newline += " \t";
                            newline += entry.Value.dev;
                            newline += " \n";
                            log += newline;
                        }

                    }
                    catch (Exception ex)
                    {
                        log += "Exeption:" + ex.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                log += "server start failed:" + ex.ToString();
            }



            log = "server stopped";
        }
        private static void requestDevName(IPEndPoint remoteEP,int ID)
        {
            Byte[] dataOut = new Byte[6];
            dataOut[0] = 0x5a;
            dataOut[1] = 0xa5;
            dataOut[2] = (byte) (ID >> 24);
            dataOut[3] = (byte) (ID >> 16);
            dataOut[4] = (byte) (ID >> 8);
            dataOut[5] = (byte) (ID);
            udpServer.Send(dataOut, 6, remoteEP); // send request
        }
        private static void addDevToServer(LocationClient newclient)
        {
            try
            {
                //insert if doesnt exist
                string query = "INSERT INTO DEV_LIST (LAT,LON,DEV_NAME,ID)";
                query += " VALUES (@lat,@lon,@dev,@id)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@lat", newclient.mLat);
                        command.Parameters.AddWithValue("@lon", newclient.mLon);
                        command.Parameters.AddWithValue("@dev", newclient.dev);
                        command.Parameters.AddWithValue("@id", newclient.ID);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                
            }
            catch (Exception ex)
            {
                //update value 
                string query = "UPDATE DEV_LIST ";
                query += " SET LAT = @lat, LON = @lon, DEV_NAME = @dev ";
                query += "where [ID] LIKE " + newclient.ID.ToString() + "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@lat", newclient.mLat);
                        command.Parameters.AddWithValue("@lon", newclient.mLon);
                        command.Parameters.AddWithValue("@dev", newclient.dev);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            
        }
        private static void addNewRecToServer(LocationClient newclient)
        {
            //add to device location history
            var table = new DataTable();
            using (var adapter = new SqlDataAdapter($"SELECT TOP 0 * FROM [SEAMAP].[dbo].[DEV_HISTORY]", connectionString))
            {
                adapter.Fill(table);
            };
            var row = table.NewRow();
            row["LAT"] = newclient.mLat;
            row["TIME"] = newclient.mLastTimeRec;
            row["LON"] = newclient.mLon;
            row["ID"] = newclient.ID;
            row["IP"] = newclient.mIP.ToString();
            table.Rows.Add(row);

            using (var bulk = new SqlBulkCopy(connectionString))
            {
                bulk.DestinationTableName = "DEV_HISTORY";
                bulk.WriteToServer(table);
            }

            table.Clear();
        }
        private static void timer20sTick(object state)
        {
            isShiplistOutdated = true;
        }
        private static void LoadSHipListFromSQL()
        {
            try
            {
                string time = ((long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds - 60000).ToString();

                using (var adapter = new SqlDataAdapter($" select [LAT],[LON],[SOG],[COG] from SHIP_LIST where [TIME]> " + time + " AND [TYPE] != '-2'", connectionString))
                {

                    adapter.Fill(shipList);
                };

            }
            catch (Exception ex)
            {
                return;
            }
            isShiplistOutdated = false;
        }
        const int frameLen = 10;
        const int MAX_FRAMES_OUTPUT = 500;
        const int maxAgeSec = 600;

        private void getDevNameFromDatabase(int ID)
        {
        }
        public static void AddLocationClient(LocationClient newclient)
        {
            
            //find  in databases device list
            using (var adapter = new SqlDataAdapter($" select [ID],[DEV_NAME] from DEV_LIST where [ID] = "+ newclient.ID.ToString(), connectionString))
            {
                DataTable tab = new DataTable();
                adapter.Fill(tab);

                if ((tab.Rows.Count) > 0 && newclient.mLon > -1000 && newclient.mLat > -1000)// device with that ID was found in the database
                {
                    if (newclient.dev.Length == 0) newclient.dev = tab.Rows[0]["DEV_NAME"].ToString();//add device name from database
                    addDevToServer(newclient);
                    
                }
                else if((newclient.dev!=null))// ID not found, create a new device
                {
                    addDevToServer(newclient);
                }
            };
            clientList[newclient.ID] = newclient;
            if (newclient.mIP.Equals(IPAddress.Parse("27.72.56.161"))) return;//test data, dont save
            var table = new DataTable();
            using (var adapter = new SqlDataAdapter($"SELECT TOP 0 * FROM [SEAMAP].[dbo].[DEV_HISTORY]", connectionString))
            {
                adapter.Fill(table);
            };
            var row = table.NewRow();
            row["ID"] = newclient.ID.ToString();
            row["IP"] = newclient.mIP.ToString();
            row["LAT"] = newclient.mLat;
            row["LON"] = newclient.mLon;
            row["TIME"] = (long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
            table.Rows.Add(row);
            try
            {
                using (var bulk = new SqlBulkCopy(connectionString))
                {
                    bulk.DestinationTableName = "DEV_HISTORY";
                    bulk.WriteToServer(table);
                }
            }
            catch (Exception e)
            {
                return;
            }
            table.Clear();
        }
        private static void sendResToClient(IPEndPoint ep, double clat,double clon)
        {
            if (isShiplistOutdated) LoadSHipListFromSQL();
            byte[] data = new byte [frameLen* MAX_FRAMES_OUTPUT];
            int indexStart = 0;
            foreach(DataRow dr in shipList.Rows)
            {

                //if(abs(clat-dr["LAT"])>1.0)
                try
                {
                    float lat = float.Parse(dr["LAT"].ToString());//BitConverter.GetBytes(entry.Value.mLat);
                    float lon = float.Parse(dr["LON"].ToString());//BitConverter.GetBytes(entry.Value.mLat);
                    float cog = float.Parse(dr["COG"].ToString());
                    if (cog == 0) continue;
                    if (Math.Abs(clat - lat) > 0.1 || Math.Abs(clon - lon) > 0.1) continue;
                    byte[] blat = BitConverter.GetBytes(lat);
                    byte[] blon = BitConverter.GetBytes(lon);
                    //short ageSec = (short)((((long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds - entry.Value.mLastTimeRec)) / 1000);
                    //if (ageSec > maxAgeSec) continue;
                    byte[] bcog = BitConverter.GetBytes(Convert.ToInt16(float.Parse(dr["COG"].ToString())));
                    byte[] frame = new byte[frameLen];
                    Buffer.BlockCopy(bcog, 0, frame, 0, 2);
                    Buffer.BlockCopy(blat, 0, frame, 2, 4);
                    Buffer.BlockCopy(blon, 0, frame, 6, 4);
                    for (int i = 0; i < frameLen; i++)
                    {
                        data[i + indexStart] = frame[frameLen - 1 - i];
                    }
                    indexStart += frameLen;
                    if (indexStart > frameLen * (MAX_FRAMES_OUTPUT - 1)) break;
                    // frame 10 bytes: 2byte age in seconds, 4byte lon, 4byte lat
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            udpServer.Send(data, indexStart, ep); // reply back
        }

        
    }
}
