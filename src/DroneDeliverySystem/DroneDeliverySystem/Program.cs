using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DroneDeliverySystem
{
    class Program
    {
        private const string sample = "5 5\n3 1 15 20\n1 2\n0 0\n2 4\n3 1";

        private int targetReach = 0;

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
                .OrderBy(point => point.X)
                .ThenBy(point => point.Y)
                .Where(point =>
                {
                    var x = point.X - startPosition.X;
                    var y = point.Y - startPosition.Y;
                    return maxMove >= Math.Abs(x) + Math.Abs(y);
                }
                )
                .ToArray();

            List<Pathes> targetGroups;

            if (!File.Exists("data.bin"))
            {
                targetGroups = GetTargetGroups(targetCount, targets);

                foreach (var group in targetGroups)
                {
                    var allTargets = targetGroups.Where(x => x != group);
                    if (allTargets.Any(x => x.Targets.Select(y => y.Target).Contains(group.Source)) ||
                        allTargets.Any(x => x.Source == group.Source))
                    {
                        Console.WriteLine("BUG");
                    }
                }

                File.WriteAllText("data.bin", JsonConvert.SerializeObject(targetGroups));
            }
            else
            {
                var fileData = File.ReadAllText("data.bin");
                targetGroups = JsonConvert.DeserializeObject<List<Pathes>>(fileData);
            }

            var drones = Enumerable.Range(0, droneCount)
                .Select(d =>
                {
                    var target = targetGroups.Skip(d % targetGroups.Count).First();

                    target.Targets.Add(new Path() {
                        Target = target.Source,
                        Distance = 0
                    });

                    return new Drone
                    {
                        Index = d,
                        Position = startPosition,
                        Targets = target.Targets.Select(x => x.Target).ToList(),
                        Ttl = maxMove - (4*40)
                    };
                });

            var result = string.Join("\n", drones.Select(drone => string.Join(" ", drone.GetMoves(turnCount))));
            var score = Drone.TargetReach;
            File.WriteAllText("output.txt", result);
        }

        private static List<Pathes> GetTargetGroups(int targetCount, Point[] targets)
        {
            var result = new List<Pathes>();

            for (int a = 0; a < targetCount; ++a)
            {
                var shortest = new List<Path>();

                var targetA = targets[a];

                for (int b = 0; b < targetCount; ++b)
                {
                    if (a == b)
                    {
                        continue;
                    }

                    var targetB = targets[b];

                    //if (b % 10 == 0)
                    //    Console.ReadLine();

                    var distance = Math.Abs(targetA.X - targetB.X) + Math.Abs(targetA.Y - targetB.Y);
                    var isTargetUsed = result
                        .SelectMany(target => target.Targets)
                        .Any(x => x.Target == targetB || x.Target == targetA);

                    isTargetUsed |= result
                        .Any(x => x.Source == targetB || x.Source == targetA);

                    if (shortest.Count != 3 && !isTargetUsed)
                    {
                        //Console.WriteLine("Add initial target > {0} ({1})", distance, targetB);

                        shortest.Add(new Path()
                        {
                            Target = targetB,
                            Distance = distance
                        });

                        continue;
                    }

                    //Console.WriteLine("targetB > {0} ({1})", distance, targetB);

                    if (!isTargetUsed && shortest.Max(x => x.Distance) > distance)
                    {
                        var path = shortest
                            .First(x => x.Distance == shortest.Max(s => s.Distance));

                        //Console.WriteLine("targetB replace > {0} ({1})", path.Distance, path.Target);

                        shortest.Remove(path);

                        shortest.Add(new Path()
                        {
                            Target = targetB,
                            Distance = distance
                        });
                    }
                }

                if (shortest.Count == 0)
                {
                    continue;
                }

                result.Add(new Pathes()
                {
                    Source = targetA,
                    Targets = shortest
                });
            }

            return result;
        }
    }

    public class Pathes
    {
        public Point Source { get; set; }
        public List<Path> Targets { get; set; }
    }

    [DebuggerDisplay("{Distance} ({Target})")]
    public class Path
    {
        public Point Target { get; set; }
        public int Distance { get; set; }
    }

    public class Drone
    {
        public int Ttl { get; set; }

        public Point Position { get; set; }

        public List<Point> Targets { get; set; }
        public int Index { get; set; }

        public static int TargetReach;

        public IEnumerable<int> GetMoves(int turnCount)
        {
            var moveList = new List<int>(Targets.Count);
            
            foreach (var target in Targets)
            {
                var x = target.X - Position.X;
                var y = target.Y - Position.Y;

                if (Ttl >= (Math.Abs(x) + Math.Abs(y)))
                {
                    Ttl -= (Math.Abs(x) + Math.Abs(y));
                    TargetReach++;
                }
                else
                {
                    break;
                }

                moveList.AddRange(x > 0
                    ? Enumerable.Range(0, Math.Abs(x)).Select(i => (int) Moves.Right)
                    : Enumerable.Range(0, Math.Abs(x)).Select(i => (int) Moves.Left));
                
                moveList.AddRange(y > 0
                    ? Enumerable.Range(0, Math.Abs(y)).Select(i => (int) Moves.Down)
                    : Enumerable.Range(0, Math.Abs(y)).Select(i => (int) Moves.Up));

                Position = target;
            }

            var complete = turnCount - moveList.Count + 1 ;
            if (complete < 0)
                return moveList.Take(turnCount+1);
            else
            {
                moveList.AddRange(Enumerable.Range(0, complete).Select(i => 0));
                return moveList;
            }
            
        }

    }

    enum Moves 
    {
        Stand = 0,
        Left = 1,
        Up = 2,
        Right = 3,
        Down = 4,
        
    }
}
