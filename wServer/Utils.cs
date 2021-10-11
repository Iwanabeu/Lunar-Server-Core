#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace wServer
{
    internal static class EnumerableUtils
    {
        public static T RandomElement<T>(this IEnumerable<T> source, Random rng)
        {
            T current = default(T);
            int count = 0;
            foreach (T element in source)
            {
                count++;
                if (rng.Next(count) == 0)
                {
                    current = element;
                }
            }
            if (count == 0)
            {
                throw new InvalidOperationException("Sequence was empty");
            }
            return current;
        }
    }

    internal static class StringUtils
    {
        public static bool ContainsIgnoreCase(this string self, string val)
        {
            return self.IndexOf(val, StringComparison.InvariantCultureIgnoreCase) != -1;
        }

        public static bool EqualsIgnoreCase(this string self, string val)
        {
            return self.Equals(val, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    internal static class MathsUtils
    {
        public static double Dist(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        public static double DistSqr(double x1, double y1, double x2, double y2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        }
    }
    internal static class LogUtils
    {
        public static void LogBuffer(this log4net.ILog log, byte[] buffer, int offset,int len)
        {
            StringBuilder sb = new StringBuilder();
            buffer.Skip(offset).Take(len).All(b => { sb.AppendFormat("{0:x2}", b); return true; });
            log.Error(sb.ToString());


        }
    }
}