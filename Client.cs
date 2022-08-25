using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP_UDP_example
{
    internal class Client
    {
        string transportLayer;
        string addr = "127.0.0.1";
        bool doStop = false;

        public Client(string transportLayer, string addr)
        {
            this.transportLayer = transportLayer;
            this.addr = addr;
        }

        public void stop()
        {
            doStop = true;
        }

        public void run()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(addr), Vars.epPort); // Address
            IPEndPoint udpEp = new IPEndPoint(IPAddress.Parse(addr), 1235); // Address
            TcpClient tcp = null;
            NetworkStream tcpStream = null;
            UdpClient udp = null;
            if (transportLayer == "tcp")
            {
                Console.WriteLine("Connecting with TCP...");
                tcp = new TcpClient(ep); // Create a new connection
                tcpStream = tcp.GetStream();
            }
            else
            {
                Console.WriteLine("Connecting with UDP...");
                udp = new UdpClient(udpEp);
            }
            byte[] messageBytes = fillArray(new byte[Vars.bytesize]);

            while (!doStop)
            {
                if (tcpStream != null)
                {
                    tcpStream.Write(messageBytes, 0, messageBytes.Length); // Write the bytes
                }
                else if (udp != null)
                {
                    udp.Send(messageBytes, ep);
                }
            }

            if (tcp != null)
            {
                tcp.Close();
            }
            else if (udp != null)
            {
                udp.Close();
            }
        }

        byte[] fillArray(byte[] arr)
        {
            byte[] fillval = new byte[24];
            for (int i = 0; i < 24; i++)
            {
                fillval[i] = (byte)(i + Convert.ToByte('a'));
            }

            int j;
            for (j = 0; j < arr.Length-24; j += 24)
            {
                fillval.CopyTo(arr, j);
            }
            
            int remainder = arr.Length - j;
            byte[] fillrest = fillval.Take(remainder).ToArray();
            fillrest.CopyTo(arr, j);

            return arr;
        }
    }
}
