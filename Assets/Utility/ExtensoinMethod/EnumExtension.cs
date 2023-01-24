using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    public static class EnumExtension
    {
        public static bool HasAny(this Enum me, Enum other)
        {
            return (Convert.ToInt32(me) & Convert.ToInt32(other)) != 0;
        }
        public static int ToInt32(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        // https://stackoverflow.com/questions/4171140/how-to-iterate-over-values-of-an-enum-having-flags
        public static IEnumerable<T> GetUniqueFlags<T>(this T flags) where T : Enum
        {
            ulong flag = 1;
            foreach (var value in Enum.GetValues(flags.GetType()).Cast<T>())
            {
                ulong bits = Convert.ToUInt64(value);
                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits && flags.HasFlag(value as Enum))
                {
                    yield return value;
                }
            }
        }
    }
}
