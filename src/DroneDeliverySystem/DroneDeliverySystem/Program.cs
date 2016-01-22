using System;
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
                .ToList();

           
            var drones = Enumerable.Range(0, droneCount)
                .Select(i => new Drone(startPosition,maxMove - (4*40)))
                .ToArray();
            
            var currentTargetTurn = 0;

            var pathes = (
                    from d in drones
                    from t in targets
                    where d.TargetReach <= currentTargetTurn
                    where d.IsAlive
                    select new Path(d, t, width))
                    .ToList();


            while (true)
            {
                if (drones.All(d => d.TargetReach > currentTargetTurn))
                {
                    currentTargetTurn++;
                     pathes = (
                   from d in drones
                   from t in targets
                   where d.TargetReach <= currentTargetTurn
                   where d.IsAlive
                   select new Path(d, t, width))
                   .ToList();
                }

                
                Console.Clear();
                Console.WriteLine($"Pathes : {pathes.Count}");       
                
                if(!pathes.Any()) break;

                var bestPath = pathes.OrderBy(path => path.Distance).First();

                bestPath.Drone.IsAlive = bestPath.Drone.CanMoveTo(bestPath);
                if (bestPath.Drone.IsAlive)
                {
                    bestPath.Drone.MoveToTarget(bestPath);
                    targets.Remove(bestPath.Destination);

                    pathes.RemoveAll(path => path.Drone == bestPath.Drone || path.Destination == bestPath.Destination);
                }
                   
                if (drones.All(d => !d.IsAlive) || !targets.Any())
                {
                    break;
                }
                
            }

            var droneOutput =
                drones.Select(
                    d =>
                        d.TargetReach + " " +
                        string.Join(" ", d.Moves.Select(m => (int)m).Concat(Enumerable.Range(0, (maxMove - d.Moves.Count)).Select(i=>0))));


            var result = string.Join("\n", droneOutput);
            var targetHit = drones.Sum(d => d.TargetReach);

            var score = targetHit*((maxMove*droneCount) - drones.Sum(d => d.Moves.Count));
            Console.WriteLine($"Score = {score}");
            File.WriteAllText("output.txt", result);
            Console.ReadLine();
        }
    }
}
