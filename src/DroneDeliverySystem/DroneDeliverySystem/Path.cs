using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DroneDeliverySystem
{
    public class Path
    {
        private readonly int _witdh;
        private Moves _XDirection = Moves.Stand;
        private Moves _YDirection = Moves.Stand;

        public Path(Drone drone, Point destination, int witdh)
        {
            Drone = drone;
            _witdh = witdh;
            Origine = drone.Position;
            Destination = destination;
        }

        public Drone Drone { get; }
        public Point Origine { get; }
        public Point Destination { get; }


        private int YDistance
        {
            get
            {
                if (Origine.Y > Destination.Y)
                {
                    _YDirection = Moves.Up;
                    return Origine.Y - Destination.Y;
                }

                _YDirection = Moves.Down;
                return Destination.Y - Origine.Y;
            }
        }

        private int XDistance
        {
            get
            {
                var maxX = Math.Max(Origine.X, Destination.X);
                var minX = Math.Min(Origine.X, Destination.X);
                var diff = maxX - minX;

                if (diff < _witdh / 2)
                {
                    _XDirection = Origine.X < Destination.X ? Moves.Right : Moves.Left;
                    return diff;
                }
                _XDirection = Origine.X < Destination.X ? Moves.Left : Moves.Right;
                return _witdh - maxX + minX;
            }
        }
         
        public int Distance => XDistance + YDistance;

        public IEnumerable<Moves> GetXMoves => Enumerable.Range(0, XDistance).Select(i => _XDirection);
        

        public IEnumerable<Moves> GetYMoves => Enumerable.Range(0, YDistance).Select(i => _YDirection);

    }
}