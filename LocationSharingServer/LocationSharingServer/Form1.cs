﻿using System;
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
    }
    public struct LocationClient
    {
        public IPAddress mIP;
        public string dev;
        public float mLat, mLon;
        public long mLastTimeSent;
        public long mLastTimeRec;
        public int msgCount;
    }

    public class ServerListener
    {
        static IPEndPoint remoteEP;
        static IPEndPoint localUser;
        static UdpClient udpServer;
        public static string log = "";
        internal static bool toStop = false;
        public static Dictionary<IPAddress, LocationClient> clientList = new Dictionary<IPAddress, LocationClient>();
        public static void Run()
        {
            
            try
            {
                udpServer = new UdpClient(50000);
                remoteEP = new IPEndPoint(IPAddress.Any, 0);
                localUser = new IPEndPoint(IPAddress.Parse("192.168.0.66"), 21400);
                log += "server start ok";
                while (true)
                {
                    try
                    {
                        Thread.Sleep(200);
                        if(toStop)break;
                        byte[] data = udpServer.Receive(ref remoteEP); // listen to packet
                        udpServer.Send(data, data.Count(), localUser);
                        if (data.Length == 16)
                        {
                            Array.Reverse(data, 0, data.Length);
                            LocationClient newclient = new LocationClient();
                            newclient.mIP = remoteEP.Address;
                            newclient.mLastTimeRec = (long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
                            newclient.mLon = System.BitConverter.ToSingle(data, 4);
                            newclient.mLat = System.BitConverter.ToSingle(data, 0);
                            newclient.mLastTimeSent = System.BitConverter.ToInt64(data, 8);
                            if (clientList.ContainsKey(newclient.mIP))
                            {
                                newclient.msgCount = clientList[newclient.mIP].msgCount + 1;
                                newclient.dev = clientList[newclient.mIP].dev;
                            }
                            else
                                newclient.msgCount = 1;
                            AddLocationClient(newclient);
                            sendResToClient(remoteEP);
                        }
                        else if (data.Length == 20)
                        {
                            LocationClient newclient = new LocationClient();
                            newclient.mIP = remoteEP.Address;
                            newclient.mLon = 0;
                            newclient.mLat = 0;
                            newclient.mLastTimeRec = (long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
                            newclient.dev = System.Text.Encoding.UTF8.GetString(data);
                            AddLocationClient(newclient);
                            sendResToClient(remoteEP);
                        }
                        log = "";
                        foreach (var entry in clientList)
                        {
                            TimeSpan time = TimeSpan.FromMilliseconds(entry.Value.mLastTimeSent+ 25200000);
                            DateTime timeDate = new DateTime(1970, 1, 1) + time;
                            string newline = "";
                            newline += entry.Value.mIP.ToString();
                            while (newline.Length < 20) newline += "_";
                            newline += entry.Value.dev;
                            while (newline.Length < 40) newline += "_";
                            newline += entry.Value.mLat.ToString();
                            while (newline.Length < 50) newline += "_";
                            newline += entry.Value.mLon.ToString();
                            while (newline.Length < 60) newline += "_";
                            newline += entry.Value.msgCount.ToString();
                            while (newline.Length < 65) newline += "_";
                            newline += timeDate.ToString()+"\n";
                            log += newline;
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        //log += "server start failed:" + ex.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                log += "server start failed:" + ex.ToString();
            }

            


        }
        const int frameLen = 10;
        const int numOfFrames = 20;
        const int maxAgeSec = 600;
        public static string connectionString = @"Data Source=WIN-CS49MK82IQN\SQLEXPRESS;Initial Catalog=seamap;Integrated Security=True";

        public static void AddLocationClient(LocationClient newclient)
        {
            
            clientList[newclient.mIP] = newclient;
            if (newclient.mIP.Equals(IPAddress.Parse("27.72.56.161"))) return;
            var table = new DataTable();
            using (var adapter = new SqlDataAdapter($"SELECT TOP 0 * FROM [SEAMAP].[dbo].[LOCATION_RECORD]", connectionString))
            {
                adapter.Fill(table);
            };
            var row = table.NewRow();
            row["IP"] = newclient.mIP.ToString();
            row["LAT"] = newclient.mLat;
            row["DEV"] = newclient.dev;
            row["LNG"] = newclient.mLon;
            row["TIME"] = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");
            table.Rows.Add(row);

            using (var bulk = new SqlBulkCopy(connectionString))
            {
                bulk.DestinationTableName = "LOCATION_RECORD";
                bulk.WriteToServer(table);
            }
            table.Clear();
        }
        private static void sendResToClient(IPEndPoint ep)
        {
            byte[] data = new byte [frameLen* numOfFrames];
            int indexStart = 0;
            foreach(var entry in clientList)
            {

                if (entry.Key == ep.Address) continue;
                
                byte[] lat = BitConverter.GetBytes(entry.Value.mLat);
                byte[] lon = BitConverter.GetBytes(entry.Value.mLon);
                short ageSec = (short)((((long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds - entry.Value.mLastTimeRec)) / 1000);
                if (ageSec > maxAgeSec) continue;
                byte[] age = BitConverter.GetBytes(ageSec);
                byte[] frame = new byte[frameLen];
                Buffer.BlockCopy(age, 0, frame, 0, 2);
                Buffer.BlockCopy(lat, 0, frame, 2, 4);
                Buffer.BlockCopy(lon, 0, frame, 6, 4);
                for (int i = 0; i < frameLen; i++)
                {
                    data[i+indexStart] = frame[frameLen-1 - i];
                }
                indexStart += frameLen;
                if (indexStart > frameLen * (numOfFrames - 1)) break;
                // frame 10 bytes: 2byte age in seconds, 4byte lon, 4byte lat
            }
            udpServer.Send(data, indexStart, ep); // reply back
        }

        
    }
}
