using System.Diagnostics;

class Program
{
    public static Point Target;
    public static char TargetSkin = '*';
    public static string MapFile = "map.txt";
    public static char Wall = '#';
    public static char StartSkin = 'x';
    public static int Delay = 0;
    public static Stopwatch timer = new Stopwatch();
    public static bool ShouldPrint = true;



    class Maze
    {
        public static List<char[]> Map = new List<char[]> { };
        private static Point CurrentLocation;
        public static List<Node> ListOfNodes = new List<Node> { };
        public static Node LastNode;

        private static Point StartingPoint = new Point(1,1);
        public static bool WasPathFound = false;

        public static List<Direction> PathToTarget = new List<Direction> { };

        public Maze(string mapfile)
        {

            string[] save = File.ReadAllLines(mapfile);

            foreach (string s in save)
            {
                Map.Add(s.ToCharArray());
            }

            for (int i = 0; i < Map.Count(); i++)
            {
                for (int j = 0; j < Map[i].Count(); j++)
                {
                    if (Map[i][j] == TargetSkin) Target = new Point(j, i);
                }
            }

            for (int i = 0; i < Map.Count(); i++)
            {
                for (int j = 0; j < Map[i].Count(); j++)
                {
                    if (Map[i][j] == StartSkin) 
                    {
                        LastNode = new Node(j, i, new List<Direction>{});
                        CurrentLocation = new Point(j, i);
                        ListOfNodes.Add(LastNode);
                        StartingPoint = new Point(j, i);
                    }
                }
            }
            PreviousDirection = ListOfNodes[0].WillMoveTo;
        }

        public static void KillNode(Node x)
        {
            for (int i = 0; i < ListOfNodes.Count(); i++)
            {
                if (ListOfNodes[i].Location.x == x.Location.x && ListOfNodes[i].Location.y == x.Location.y)
                {
                    ListOfNodes[i].IsAlive = false;
                }
            }
        }

        public static void KillShouldBeDeadNodes()
        {
            foreach (Node n in ListOfNodes)
            {
                if (GetPossibleDirections(n.Location).Count() == 0) KillNode(n);
                else if (n.Visits > 3) KillNode(n);
                else n.IsAlive = true;
            }
        }

        public static int FindIndexOFBestNode()
        {
            int CurrentBest = ListOfNodes.Count() - 1;

            while (true)
            {
                if (ListOfNodes[CurrentBest].IsAlive) break;
                else
                {
                    CurrentBest -= 1;
                    if (CurrentBest < 0)
                    {
                        KillShouldBeDeadNodes();
                        CurrentBest = ListOfNodes.Count() - 1;
                    }
                }
            }

            for (int i = ListOfNodes.Count() - 1; i >= 0; i--)
            {
                if (ListOfNodes[i].IsAlive && i != CurrentBest)
                {
                    if (ListOfNodes[i].DistanceToTarget < ListOfNodes[CurrentBest].DistanceToTarget)
                    {
                        CurrentBest = i;
                    }
                }
            }

            return CurrentBest;
        }

        public static void MoveTo(Direction x)
        {
            switch (x)
            {
                case Direction.North:
                    {
                        CurrentLocation = new Point(CurrentLocation.x, CurrentLocation.y - 1); 
                        break;
                    }
                case Direction.South:
                    {
                        CurrentLocation = new Point(CurrentLocation.x, CurrentLocation.y + 1); 
                        break;
                    }
                case Direction.West:
                    {
                        CurrentLocation = new Point(CurrentLocation.x - 1, CurrentLocation.y); 
                        break;
                    }
                case Direction.East:
                    {
                        CurrentLocation = new Point(CurrentLocation.x + 1, CurrentLocation.y); 
                        break;
                    }
            }
        }

        public static Direction InverseOfDirection(Direction x)
        {
            switch (x)
            {
                case Direction.North: return Direction.South;
                case Direction.South: return Direction.North;
                case Direction.West: return Direction.East;
                case Direction.East: return Direction.West;
            }
            return Direction.None;
        }

        public static void PrintTrack()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.CursorTop = CurrentLocation.y;
            Console.CursorLeft = CurrentLocation.x;
            Console.Write("O");
            Thread.Sleep(Delay);
        }

        public static void AddNodes()
        {
            for (int i = 0; i < Map.Count(); i++)
            {
                for (int j = 0; j < Map[i].Count(); j++)
                {
                    if (Map[i][j] != Wall)
                    {
                        if (GetPossibleDirections(new Point(j, i)).Count() > 1)
                        {
                            ListOfNodes.Add(new Node(j, i, new List<Direction>{}));
                            break;
                        }
                    }
                }
            }
        }

        public static void PrintInfoInPathToTarget()
        {
            Console.Clear();
            int i = 1;
            foreach (Direction d in GetPathToTarget())
            {
                switch (d)
                {
                    case Direction.North:
                        {
                            Console.WriteLine(i + ": Move to North");
                            i++; break;
                        }
                    case Direction.South:
                        {
                            Console.WriteLine(i + ": Move to South");
                            i++; break;
                        }
                    case Direction.West:
                        {
                            Console.WriteLine(i + ": Move to West");
                            i++; break;
                        }
                    case Direction.East:
                        {
                            Console.WriteLine(i + ": Move to East");
                            i++; break;
                        }
                }
            }
        }
        public static int CurrentCursorX = 1;
        public static int CurrentCursorY = 1;

        public static void PrintPathToTarget()
        {
            CurrentCursorX = StartingPoint.x;
            CurrentCursorY = StartingPoint.y;
            Console.CursorTop = CurrentCursorY;
            Console.CursorLeft = CurrentCursorX;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write('O');
            Console.CursorTop = CurrentCursorY;
            Console.CursorLeft = CurrentCursorX;
            
            foreach (Direction d in GetPathToTarget())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Thread.Sleep(Delay);
                MoverCursorTo(d);
                Console.Write('O');
                Console.CursorTop = CurrentCursorY;
                Console.CursorLeft = CurrentCursorX;
            }
        }

        public static void MoverCursorTo(Direction d)
        {
            switch (d)
            {
                case Direction.North:
                    {
                        CurrentCursorY -= 1;
                        Console.CursorTop = CurrentCursorY;
                        break;
                    }
                case Direction.South:
                    {
                        CurrentCursorY += 1;
                        Console.CursorTop = CurrentCursorY;
                        break;
                    }
                case Direction.West:
                    {
                        CurrentCursorX -= 1;
                        Console.CursorLeft = CurrentCursorX;
                        break;
                    }
                case Direction.East:
                    {
                        CurrentCursorX += 1;
                        Console.CursorLeft = CurrentCursorX;
                        break;
                    }
            }

        }

        public static bool IsAtNode = true;
        public static Direction PreviousDirection = Direction.None;
        public static List<Direction> GetPathToTarget()
        {
            List<Direction> output = new List<Direction> { };

            timer.Start();
            while(!WasPathFound)
            {

                List<Direction> Possibilities = RemoveDirection(GetPossibleDirections(CurrentLocation), InverseOfDirection(PreviousDirection));
                int NumberOfPossibleDirections = Possibilities.Count();
                //ListOfNodes[0].PathToNode = new List<Direction> { };

                Map[CurrentLocation.y][CurrentLocation.x] = Wall;

                if (NumberOfPossibleDirections == 0)
                {
                    Map[CurrentLocation.y][CurrentLocation.x] = Wall; //  KillShouldBeDeadNodes();

                    int index = FindIndexOFBestNode();

                    CurrentLocation = new Point(ListOfNodes[index].Location.x, ListOfNodes[index].Location.y);
                    
                    output = new List<Direction> {};

                    foreach (Direction d in ListOfNodes[index].PathToNode)
                    {
                        output.Add(d);
                    }

                    IsAtNode = true;

                    PreviousDirection = Direction.None;
                    ListOfNodes[index].Visits += 1;
                    

                    if (ListOfNodes[index].Visits > 4) KillNode(ListOfNodes[index]);

                    // if (ListOfNodes[index].Location.x == LastNode.Location.x && ListOfNodes[index].Location.y == LastNode.Location.y) KillNode(LastNode);
                }

                if (NumberOfPossibleDirections == 1)
                {
                    Map[CurrentLocation.y][CurrentLocation.x] = Wall; // KillShouldBeDeadNodes();


                    if (IsAtNode) KillNode(LastNode);

                    PreviousDirection = BestDirection(CurrentLocation, Possibilities);
                    MoveTo(PreviousDirection);
                    output.Add(PreviousDirection);

                    IsAtNode = false;

                    Debug.Assert(Map[CurrentLocation.y][CurrentLocation.x] != Wall);
                }

                if (NumberOfPossibleDirections > 1 && !IsAtNode)
                {
                    Map[CurrentLocation.y][CurrentLocation.x] = Wall;
                    
                    ListOfNodes.Add(new Node(CurrentLocation.x, CurrentLocation.y, output));
                
                    //ListOfNodes[ListOfNodes.Count()-1].PathToNode = output;

                    PreviousDirection = BestDirection(CurrentLocation, Possibilities);
                    MoveTo(PreviousDirection);
                    output.Add(PreviousDirection);

                    LastNode = ListOfNodes[ListOfNodes.Count() - 1];

                    Debug.Assert(Map[CurrentLocation.y][CurrentLocation.x] != Wall);

                    IsAtNode = false;
                }

                if (NumberOfPossibleDirections > 1 && IsAtNode)
                {
                    PreviousDirection = BestDirection(CurrentLocation, Possibilities);
                    MoveTo(PreviousDirection);
                    output.Add(PreviousDirection);
                    IsAtNode = false;

                    Debug.Assert(Map[CurrentLocation.y][CurrentLocation.x] != Wall);
                }

                if (Map[CurrentLocation.y][CurrentLocation.x] == TargetSkin)
                {
                    WasPathFound = true;
                }

                //KillShouldBeDeadNodes();
                if (ShouldPrint) PrintTrack();
            }
            // Debug.Assert(false);
            timer.Stop();
            return output;
        }
    }



    public enum Direction { North, South, West, East, None };
    public static List<Direction> AllDirections = new List<Direction> { Direction.North, Direction.South, Direction.West, Direction.East };

    class Node
    {
        public bool IsAlive;
        public readonly Point Location;
        public List<Direction> PossibleDirections;
        public readonly double DistanceToTarget;
        public readonly Direction WillMoveTo;
        public int Visits;

        public readonly List<Direction> PathToNode;


        private static int it = 0;
        public Node(int x, int y, List<Direction> _PathToNode)
        {
            Visits = 1;
            IsAlive = true;
            Location = new Point(x, y);
            PossibleDirections = GetPossibleDirections(Location);

            if (PossibleDirections.Count() < 1) IsAlive = false;

            DistanceToTarget = DistanceBetween(Location, Target);
            WillMoveTo = BestDirection(Location, PossibleDirections);

            PathToNode = new List<Direction>{};
            foreach (Direction d in _PathToNode)
            {
                PathToNode.Add(d);
            }

            //PathToNode =  _PathToNode;
            //Debug.Assert(it < 6);
            it++;
        }
    }


    // requires testing
    public static Direction BestDirection(Point Location, List<Direction> Possibilities)
    {
        List<object[]> array = new List<object[]> { };

        foreach (Direction d in Possibilities)
        {
            if (d == Direction.North) array.Add(new object[] { Direction.North, DistanceBetween(new Point(Location.x, Location.y - 1), Target) });
            if (d == Direction.South) array.Add(new object[] { Direction.South, DistanceBetween(new Point(Location.x, Location.y + 1), Target) });
            if (d == Direction.West) array.Add(new object[] { Direction.West, DistanceBetween(new Point(Location.x - 1, Location.y), Target) });
            if (d == Direction.East) array.Add(new object[] { Direction.East, DistanceBetween(new Point(Location.x + 1, Location.y), Target) });
        }
        int CurrentBest = 0;

        for (int i = 0; i < Possibilities.Count(); i++)
        {
            for (int j = i + 1; j < Possibilities.Count() - i; j++)
            {
                if ((double)array[i][1] > (double)array[j][1]) CurrentBest = j;
            }
        }

        return (Direction)array[CurrentBest][0];
    }

    public static List<Direction> GetPossibleDirections(Point point)
    {
        List<Direction> output = new List<Direction> { };

        if (Maze.Map[point.y - 1][point.x] != Wall) output.Add(Direction.North);
        if (Maze.Map[point.y + 1][point.x] != Wall) output.Add(Direction.South);
        if (Maze.Map[point.y][point.x - 1] != Wall) output.Add(Direction.West);
        if (Maze.Map[point.y][point.x + 1] != Wall) output.Add(Direction.East);

        return output;
    }


    public static List<Direction> RemoveDirection(List<Direction> list, Direction remove)
    {
        List<Direction> output = new List<Direction> { };

        foreach (Direction d in list)
        {
            if (d != remove && d != Direction.None) output.Add(d);
        }
        return output;
    }


    public record Point(int x, int y);

    public static double DistanceBetween(Point one, Point two)
    {
        int HorizontalLength = Math.Abs(one.x - two.x);
        int VerticalLenght = Math.Abs(one.y - two.y);
        return (Math.Sqrt(Math.Pow(HorizontalLength, 2) + Math.Pow(VerticalLenght, 2)));
    }

    public static void TestDistanceBetween()
    {
        Point point1 = new Point(1, 3);
        Point point2 = new Point(3, 4);

        Debug.Assert(DistanceBetween(point1, point2) == Math.Sqrt(5));
    }

    public static void PrintMap()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;
        foreach (char[] charArray in Maze.Map)
        {
            foreach (char c in charArray)
            {
                Console.Write(c);
            }
            Console.WriteLine();
        }
        Console.ForegroundColor = ConsoleColor.Green;
    }

    static void Main()
    {
        Console.Clear();
        TestDistanceBetween();
        Maze maze = new Maze("map.txt");
        Delay = 0;
        PrintMap();

        ShouldPrint = false;

        Maze.PrintPathToTarget();
 
        
        Console.CursorTop = Maze.Map.Count();
        Console.CursorLeft = 0;


        Console.WriteLine($"Path found in {timer.Elapsed}!");
    }
}
