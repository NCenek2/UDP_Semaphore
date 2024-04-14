using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Listener
{
    public class Listener
    {
        private readonly int _port;
        private bool _started = true;
        public Listener(int port)
        {
            _port = port;
        }

        public void Start()
        {
            string filePath = $"{_port}.txt"; // Specify the file path to write the received data

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }


            Task.Run(() =>
            {

                try
                {
                    // Create a UdpClient listening on the specified port
                    using UdpClient udpClient = new UdpClient(_port);
                    Console.WriteLine($"Listening for UDP packets on port {_port}...");
                    int packetCount = 1;

                    // Receive UDP packets synchronously
                    while (_started)
                    {
                        // Receive a UDP packet
                        IPEndPoint remoteEP = null;
                        byte[] buffer = udpClient.Receive(ref remoteEP);
                        if (buffer == null || buffer.Length != 1216)
                        {
                            continue;
                        }

                        WritePacketToFile(buffer, filePath);

                        File.AppendAllText(filePath, $"Packet ${packetCount}: Received UDP packet from {remoteEP} on Port {_port}\n" + Environment.NewLine);
                        Console.WriteLine($"Packet ${packetCount}: Received UDP packet from {remoteEP} on Port {_port}\n");
                        packetCount++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }

        private void WritePacketToFile(byte[] buffer, string filePath)
        {
            using var memoryStream = new MemoryStream(buffer);
            using var packetReader = new PacketReader(memoryStream);

            var sb = new StringBuilder();
            File.AppendAllText(filePath, "Active" + Environment.NewLine);
            for (int i = 0; i < 32; i++)
            {
                sb.Append($" {packetReader.ReadByte().ToString()}");
            }
            File.AppendAllText(filePath, sb.ToString() + Environment.NewLine);
            sb.Clear();

            File.AppendAllText(filePath, "Index" + Environment.NewLine);
            for (int i = 0; i < 32; i++)
            {
                sb.Append($" {packetReader.ReadByte().ToString()}");
            }
            File.AppendAllText(filePath, sb.ToString() + Environment.NewLine);
            sb.Clear();

            File.AppendAllText(filePath, "Channel Amplification" + Environment.NewLine);
            for (int i = 0; i < 32; i++)
            {
                sb.Append($" {packetReader.ReadSingle().ToString()}");
            }
            File.AppendAllText(filePath, sb.ToString() + Environment.NewLine);
            sb.Clear();

            File.AppendAllText(filePath, "Balancer" + Environment.NewLine);
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    sb.Append($" {packetReader.ReadSingle().ToString()}");
                }
                File.AppendAllText(filePath, $"Channel {i + 1}: {sb.ToString()}" + Environment.NewLine);
                sb.Clear();
            }
        }

        public void Stop()
        {
            _started = false;
        }
    }
}


