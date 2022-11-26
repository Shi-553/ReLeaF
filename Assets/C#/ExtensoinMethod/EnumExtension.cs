using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility{
    public static class EnumExtension
    {
        public static int ToInt32(this Enum value)
        {
            return Convert.ToInt32(value);
        }
    }
}
