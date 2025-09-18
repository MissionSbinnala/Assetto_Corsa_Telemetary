using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Telemetry.Tools
{
    public class UDPReceiver
    {
        private UdpClient client;
        private Thread thread;
        private bool running = false;

        public event Action<string> OnDataReceived;

        public void Start(int port)
        {
            client = new UdpClient(port);
            running = true;
            thread = new Thread(ReceiveLoop) { IsBackground = true };
            thread.Start();
        }

        private void ReceiveLoop()
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            while (running)
            {
                try
                {
                    var data = client.Receive(ref remoteEP);
                    string message = Encoding.UTF8.GetString(data);

                    OnDataReceived?.Invoke(message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("UDP Error: " + ex.Message);
                }
            }
        }

        public void Stop()
        {
            running = false;
            client?.Close();
        }
    }

    public enum UDPType { Point, Lap, Pit }
}
