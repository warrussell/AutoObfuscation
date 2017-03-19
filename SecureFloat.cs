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
            obfuscate(f);
        }

        private void obfuscate(float f)
        {
            this.value1 = f + (f / 2);
            this.value2 = f / 2;
            this.value3 = f * 2;
        }

        private float unobfuscate()
        {
            return 2 * (this.value1 + this.value2) - this.value3;
        }

        public static implicit operator float(SecureFloat sf)
        {
            return sf.unobfuscate();
        }

        public static implicit operator SecureFloat(float f)
        {
            return new SecureFloat(f);
        }

        public static implicit operator int(SecureFloat sf)
        {
            return (int)sf.unobfuscate();
        }

        public static implicit operator SecureFloat(int i)
        {
            return new SecureFloat(i);
        }
    }
}
