using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Limited
{
    public class Buffer
    {
        public byte[] Active = new byte[32];
        public byte[] Index = new byte[32];
        public float[] ChannAmp = new float[32];
        public float[,] Balancers = new float[32, 8];
    }
}
