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

    public class ServerListener
    {
        
        public static string log = "";
        public ServerListener()
        {
            
        }
        public static void Run()
        {
            IPEndPoint remoteEP;
            UdpClient udpServer;
            udpServer = new UdpClient(50000);
            remoteEP = new IPEndPoint(IPAddress.Any, 0);
            
            log += "server start ok";
            while (true)
            {
                Thread.Sleep(1000);

                byte[] data = udpServer.Receive(ref remoteEP); // listen to packet
                byte[] dataLon = { data[11], data[10], data[9], data[8] };
                byte[] dataLat = { data[15], data[14], data[13], data[12] };
                float mlong = System.BitConverter.ToSingle(dataLon, 8);
                float mlat = System.BitConverter.ToSingle(dataLat, 12);
                //udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back
                log += "\n"+remoteEP.Address.ToString() + ", long:" + mlong.ToString() + ", lat:" + mlat.ToString();
            }
            
        }
    }
    public partial class Form1 : Form
    {


        Thread thread1;
        public Form1()
        {
            InitializeComponent();
            thread1 = new Thread(ServerListener.Run);
            thread1.Start();
            
        }
        

        private void timer1_Tick(object sender, EventArgs e)
        {
            richTextBox1.Text = ServerListener.log;
        }
        

    }
}
