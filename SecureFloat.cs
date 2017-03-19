using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoObfuscation
{
    class SecureFloat
    {
        public float value1;
        public float value2;
        public float value3;

        public SecureFloat(float f)
        {
            value1 = f + (f / 2);
            value2 = f / 2;
            value3 = f * 2;
        }

        public static implicit operator float(SecureFloat sf)
        {
            return 2 * (sf.value1 + sf.value2) - sf.value3;
        }

        public static implicit operator SecureFloat(float f)
        {
            return new SecureFloat(f);
        }
    }
}
