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
        IPEndPoint remoteEP;
        public event Action<string>? OnDataReceived;
        public event Func<string, bool>? OnSessionReceived;



        public void Start(int port)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            client = new UdpClient();
            client.Connect(remoteEP);
            thread = new Thread(UDPSession) { IsBackground = true };
            thread.Start();
        }

        private void UDPSession()
        {
            Waiting();
            ReceiveSessionData();
            ReceiveLoop();
        }

        private void ReceiveSessionData()
        {
            var sessionData = client.Receive(ref remoteEP);
            running = OnSessionReceived?.Invoke(Encoding.UTF8.GetString(sessionData)) ?? false;
        }

        private void Waiting()
        {

            while (!running)
            {
                Debug.WriteLine("Waiting for start...");
                Thread.Sleep(1000);
                try
                {
                    client.Send(Encoding.UTF8.GetBytes("Can you see me?".ToArray()));
                    var data = client.Receive(ref remoteEP);
                    if (data.Length > 0 && Encoding.UTF8.GetString(data) is "I've Received!")
                    {
                        Debug.WriteLine(Encoding.UTF8.GetString(data));
                        running = true;
                    }
                }
                catch (SocketException se)
                {
                    if (se.SocketErrorCode != SocketError.ConnectionReset)
                        Debug.WriteLine("Socket Error: " + se.Message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("UDP Error: " + ex.Message);
                }
            }
        }

        private void ReceiveLoop()
        {
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
