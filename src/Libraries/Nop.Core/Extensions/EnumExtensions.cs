using System;

namespace Nop.Core
{
    public static class EnumExtensions
    {
        public static int ToInt(this Enum enumValue)
        {
            return (int)((object)enumValue);
        }
    }
}
