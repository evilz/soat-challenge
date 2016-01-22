using System.Collections.Generic;
using System.Drawing;

namespace DroneDeliverySystem
{
    public class Drone
    {
        public Drone(Point position, int ttl)
        {
            _ttl = ttl;
            Position = position;
        }

        private int _ttl;

        public Point Position { get; private set; }
        
        public int TargetReach { get; private set; }
        
        public bool CanMoveTo(Path target)
        {
            return _ttl >= target.Distance;
        }

        public List<Moves> Moves { get; } = new List<Moves>();
        public bool IsAlive { get; set; } = true;

        public void MoveToTarget(Path target)
        {
            Moves.AddRange(target.GetXMoves);
            Moves.AddRange(target.GetYMoves);
            TargetReach++;
            _ttl -= target.Distance;
            Position = target.Destination;
        }
    }
}