using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace TCP_UDP_example
{
    internal class Server
    {
        long recvCnt = 0;
        Mutex recvMutex = new Mutex();
        bool doStop = false;
        string transportLayer;
        string addr = "127.0.0.1";

        public Server(string transportLayer, string addr)
        {
            this.transportLayer = transportLayer;
            this.addr = addr;
        }

        internal void stop()
        {
            doStop = true;
        }

        public void run()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(addr), Vars.epPort); // Address
            TcpListener tcp = null;
            TcpClient tcpClient = null;
            UdpClient udp = null;
            if (transportLayer == "tcp")
            {
                tcp = new TcpListener(ep); // Instantiate the object
                tcp.Start(); // Start listening...
                Console.WriteLine("Listening with TCP...");
            }
            else
            {
                udp = new UdpClient(ep);
                Console.WriteLine("Listening with UDP...");
            }

            byte[] buffer = new byte[Vars.bytesize];
            System.Timers.Timer tick = new System.Timers.Timer(1000);
            tick.Elapsed += showSummary;
            tick.AutoReset = true;
            tick.Enabled = true;

            if (tcp != null)
            {
                tcpClient = tcp.AcceptTcpClient();
            }
            tick.Start();
            while (!doStop)
            {
                recvMutex.WaitOne();
                for (int i = 0; i < 1000; i++)
                {
                    if (tcpClient != null)
                    {
                        recvCnt += tcpClient.GetStream().Read(buffer, 0, Vars.bytesize);
                    }
                    else if (udp != null)
                    {
                        IPEndPoint tmp = null;
                        recvCnt += udp.Receive(ref tmp).Length;
                    }
                }
                recvMutex.ReleaseMutex();
            }

            if (tcp != null)
            {
                tcp.Stop();
            }
            else if (udp != null)
            {
                udp.Close();
            }
        }

        private void showSummary(object? sender, ElapsedEventArgs e)
        {
            recvMutex.WaitOne();
            if (recvCnt > 1000 * 1000 * 1000)
            {
                Console.WriteLine($"{(float)recvCnt / (1000 * 1000 * 1000)}b");
            }
            else if (recvCnt > 1000 * 1000)
            {
                Console.WriteLine($"{(float)recvCnt / (1000 * 1000)}m");
            }
            else if (recvCnt > 1000)
            {
                Console.WriteLine($"{(float)recvCnt / 1000}k");
            }
            else
            {
                Console.WriteLine(recvCnt);
            }
            recvCnt = 0;
            recvMutex.ReleaseMutex();
        }
    }
}
