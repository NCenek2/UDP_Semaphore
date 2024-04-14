using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDP_Limited
{
    public class Socket
    {
        private readonly int _port;
        private readonly string _ipAddress;
        private bool _connected = false;
        private readonly PacketBuilder _packetBuilder;
        private readonly Timer _packetSenderTimer;
        private readonly SemaphoreSlim _semaphore;
        private readonly UdpClient _udpClient;
        private readonly Buffer _buffer;

        public Socket(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
            _packetBuilder = new PacketBuilder();
            _semaphore = new SemaphoreSlim(1, 1);
            _udpClient = new UdpClient();
            _packetSenderTimer = new Timer(SendPacket, null, Timeout.Infinite, Timeout.Infinite);
            _buffer = new Buffer();
        }



        public void StartSending()
        {
            _connected = true;
            int packetsPerSecond = 40;
            // Calculate the interval between packets based on desired packets per second
            int packetIntervalMilliseconds = 1000 / packetsPerSecond;

            // Start the packet sender timer
            _packetSenderTimer.Change(0, packetIntervalMilliseconds);
        }

        public void StopSending()
        {
            // Stop the packet sender timer
            _packetSenderTimer.Change(Timeout.Infinite, Timeout.Infinite);
            DisconnectSocket();
        }

        private async void SendPacket(object state)
        {
            if (_connected)
            {
                try
                {
                    // Wait for semaphore to ensure only one thread accesses the UDP client at a time
                    await _semaphore.WaitAsync();

                    // Construct and send UDP packet

                    ClearBuffer();
                    BuildAstiBuffer();
                    _packetBuilder.Clear();
                    byte[] buffer = BuildPacket();
                    await _udpClient.SendAsync(buffer, buffer.Length, _ipAddress, _port); // Example destination IP and port
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending UDP packet: {ex.Message}");
                }
                finally
                {
                    // Release semaphore
                    _semaphore.Release();
                }
            }
        }
        private void DisconnectSocket()
        {
            if (_connected)
            {
                _connected = false;
                _udpClient.Close();
                _udpClient.Dispose();
                _semaphore.Dispose();
            }
        }

        private void BuildAstiBuffer()
        {
            for (var soundIndex = 0; soundIndex < 32; soundIndex++)
            {
                _buffer.Active[soundIndex] = 1;
                _buffer.Index[soundIndex] = 2;
                _buffer.ChannAmp[soundIndex] = 1.25f;

                for (var balancerIndex = 0; balancerIndex < 8; balancerIndex++)
                {
                    _buffer.Balancers[soundIndex, balancerIndex] = 0.25f;
                }
            }
        }

        private void ClearBuffer()
        {
            _buffer.Active = new byte[32];
            _buffer.Index = new byte[32];
            _buffer.ChannAmp = new float[32];
            _buffer.Balancers = new float[32, 8];
        }

        private byte[] BuildPacket()
        {

            foreach (var active in _buffer.Active)
            {
                _packetBuilder.WriteByte(active);
            }

            foreach (var index in _buffer.Index)
            {
                _packetBuilder.WriteByte(index);
            }

            foreach (var channelAmp in _buffer.ChannAmp)
            {
                _packetBuilder.WriteFloat(channelAmp);
            }

            foreach (var balancer in _buffer.Balancers)
            {
                _packetBuilder.WriteFloat(balancer);
            }

            return _packetBuilder.GetPacketBytes();
        }
    }
}
