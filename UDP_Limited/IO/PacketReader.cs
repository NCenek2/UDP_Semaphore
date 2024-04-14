using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Limited
{
    public class PacketReader : BinaryReader
    {
        private readonly MemoryStream _ms;
        public PacketReader(MemoryStream ms) : base(ms)
        {
            _ms = ms;
        }
    }
}
