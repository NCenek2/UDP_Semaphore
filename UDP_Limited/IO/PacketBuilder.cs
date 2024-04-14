using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Limited
{
    public class PacketBuilder
    {
        private readonly MemoryStream _ms;
        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }

        public void WriteByte(byte by)
        {
            _ms.WriteByte(by);
        }

        public void WriteFloat(float fl)
        {
            byte[] flArr = BitConverter.GetBytes(fl);
            _ms.Write(flArr);
        }

        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }

        public void Clear()
        {
            _ms.SetLength(0);
        }
    }
}
