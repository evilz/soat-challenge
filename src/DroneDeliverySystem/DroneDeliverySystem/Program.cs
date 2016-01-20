using System.Drawing;
using System.IO;
using System.Linq;

namespace DroneDeliverySystem
{
    class Program
    {
       // private const string sample = "5 5\n3 1 15 20\n1 2\n0 0\n2 4\n3 1";

        static void Main(string[] args)
        {
            var input = File.OpenText("BigChallengeInput.txt");
            //var input = new StringReader(sample);

            var size = input.ReadLine().Split(' ').Select(int.Parse).ToArray();

            int width = size[0];
            int height = size[1];

            var infos = input.ReadLine().Split(' ').Select(int.Parse).ToArray();

            var targetCount = infos[0];
            var droneCount = infos[1];
            var maxMove = infos[2];
            var turnCount = infos[3];

            var start = input.ReadLine().Split(' ').Select(int.Parse).ToArray();
            var startPosition = new Point(start[1], start[0]);

            var targets = Enumerable.Range(0, targetCount)
                .Select(i => input.ReadLine().Split(' ').Select(int.Parse).ToArray())
                .Select(i => new Point(i[1], i[0]))
                //.OrderBy(point => point.X)
                //.ThenBy(point => point.Y)
                //.Where(point => startPosition.GetDistanceTo(point,width) <= maxMove)
                .ToList();

           
            var drones = Enumerable.Range(0, droneCount)
                .Select(i => new Drone
                {
                    Position = startPosition,
                    Ttl = maxMove - (4*40)
                })
                .ToArray();


            var currentDrone = 0;
            while (true)
            {
                var drone = drones[currentDrone];
                
                var pathes = targets.Select(t => new Path(drone.Position, t,width)).ToArray();

                var target = drone.AcquireTarget(pathes);

                drone.IsAlive = drone.CanMoveTo(target);

                if(drone.IsAlive)
                {
                    drone.MoveToTarget(target);
                    targets.Remove(drone.Position);
                }
               
                if (drones.All(d => !d.IsAlive) || !targets.Any())
                {
                    break;
                }

                currentDrone = currentDrone == droneCount - 1 
                    ? 0 
                    : currentDrone + 1;
            }

            var droneOutput =
                drones.Select(
                    d =>
                        d.TargetReach + " " +
                        string.Join(" ", d.Moves.Select(m => (int)m).Concat(Enumerable.Range(0, (maxMove - d.Moves.Count)).Select(i=>0))));


            var result = string.Join("\n", droneOutput);
            var score = drones.Sum(d => d.TargetReach);
            File.WriteAllText("output.txt", result);
        }
    }
}
