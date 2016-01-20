using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DroneDeliverySystem
{
    public class Path
    {
        private readonly int _witdh;

        public Path(Point origine, Point destination,int witdh)
        {
            _witdh = witdh;
            Origine = origine;
            Destination = destination;
            Distance = origine.GetDistanceTo(destination,witdh);
        }

        public Point Origine { get; }
        public Point Destination { get; }
        public int Distance { get; }
        
        public IEnumerable<Moves> GetXMoves()
        {
            // TODO : Look not good ??? 140 226 615 point(s) WTF

            var x1 = Math.Abs(Destination.X - Origine.X); // go left
            var x2 = Math.Abs(Origine.X - _witdh + Destination.X); //right
            
            return x1 <= x2
                ? Enumerable.Range(0, Math.Abs(x1)).Select(i => Moves.Left)
                : Enumerable.Range(0, Math.Abs(x2)).Select(i => Moves.Right);

            //return x > 0
            //    ? Enumerable.Range(0, Math.Abs(x)).Select(i => Moves.Right)
            //    : Enumerable.Range(0, Math.Abs(x)).Select(i => Moves.Left);
        }

        public IEnumerable<Moves> GetYMoves()
        {
            var y = Destination.Y - Origine.Y;
            return y > 0
                ? Enumerable.Range(0, Math.Abs(y)).Select(i => Moves.Down)
                : Enumerable.Range(0, Math.Abs(y)).Select(i => Moves.Up);

        }
    }
}