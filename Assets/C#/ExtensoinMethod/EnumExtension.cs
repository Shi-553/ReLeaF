using System;

namespace Utility
{
    public static class EnumExtension
    {
        public static int ToInt32(this Enum value)
        {
            return Convert.ToInt32(value);
        }
    }
}
