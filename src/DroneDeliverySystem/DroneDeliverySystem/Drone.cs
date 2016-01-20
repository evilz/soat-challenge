using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DroneDeliverySystem
{
    public class Drone
    {
        public int Ttl { private get; set; }

        public Point Position { get; set; }
        
        public int TargetReach { get; private set; }

        public Path AcquireTarget(Path[] availableTargets)
        {
            var minDistance = availableTargets.Min(arg => arg.Distance);
            var selectedTarget = availableTargets.Where(arg => arg.Distance == minDistance).Suffle().First();

            return selectedTarget;
        }


        public bool CanMoveTo(Path target)
        {
            return Ttl >= target.Distance;
        }

        public List<Moves> Moves { get; } = new List<Moves>();
        public bool IsAlive  { get; set; }

        public void MoveToTarget(Path target)
        {
            Moves.AddRange(target.GetXMoves());
            Moves.AddRange(target.GetYMoves());
            TargetReach++;
            Ttl -= target.Distance;
            Position = target.Destination;
        }
    }
}