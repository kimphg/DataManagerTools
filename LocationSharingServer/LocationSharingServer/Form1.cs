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
    public partial class Form1 : Form
    {
        UdpClient udpServer;
        IPEndPoint remoteEP;
        public Form1()
        {
            InitializeComponent();
            UdpClient udpServer = new UdpClient(50000);
            remoteEP = new IPEndPoint(IPAddress.Any,0);
        }
        private void  initNetwork()
        {
            

            while (true)
            {
                
                var data = udpServer.Receive(ref remoteEP); // listen on port 11000
                Console.Write("receive data from " + remoteEP.ToString());
                udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back
            }
        }

    }
}
