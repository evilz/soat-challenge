using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DroneDeliverySystem
{
    static class Helpers
    {
        public static IEnumerable<T> Suffle<T>(this IEnumerable<T> source)
        {
            Random rand = new Random();
            return source
                .Select(i => new { i, key = rand.Next() })
                .OrderBy(p => p.key)
                .Select(p => p.i)
                .ToArray();
        } 
    }
}