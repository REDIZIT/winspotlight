using System;

namespace Winspotlight.Extensions
{
    public static class MathUtils
    {
        public static int Clamp(int value, int min, int max)
        {
            if (max < min) max = min;

            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }
    }
}
