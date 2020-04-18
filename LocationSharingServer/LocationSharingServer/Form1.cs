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
            ServerListener.toStop = true;
            
        }


    }
    public struct LocationClient
    {
        public IPAddress mIP;
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
                        if (data.Length < 16) continue;
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
                        }
                        else
                            newclient.msgCount = 1;
                        clientList[newclient.mIP] = newclient;
                        sendResToClient(remoteEP);
                        log = "";
                        foreach (var entry in clientList)
                        {
                            TimeSpan time = TimeSpan.FromMilliseconds(entry.Value.mLastTimeSent+ 25200000);
                            DateTime timeDate = new DateTime(1970, 1, 1) + time;
                            string newline = "";
                            newline += entry.Value.mIP.ToString();
                            while (newline.Length < 20) newline += " ";
                            newline += entry.Value.mLat.ToString();
                            while (newline.Length < 30) newline += " ";
                            newline += entry.Value.mLon.ToString();
                            while (newline.Length < 40) newline += " ";
                            newline += entry.Value.msgCount.ToString();
                            while (newline.Length < 45) newline += " ";
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
