using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DroneDeliverySystem
{
    static class Helpers
    {
        public static int GetDistanceTo(this Point current, Point destination,int width)
        {
            // TODO : Look not good ??? 140 226 615 point(s) WTF

            var x1 = Math.Abs(destination.X - current.X); // go left
            var x2 = Math.Abs(current.X - width + destination.X); //right

            var x = Math.Min(x1, x2);
            var y = destination.Y - current.Y;
            return Math.Abs(x) + Math.Abs(y);
        }

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