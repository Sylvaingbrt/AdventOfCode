//IT'S FINALLY TIME TO USE A* ALGORITHM! (Wait is it Dijkstra in the end?)
//Classic but never disappoint.

using System.Drawing;

public class Node : IEquatable<Node>
{
    //We use Node here as a class and not a struct to check for null nodes
    public Point pos;
    public int gCost;
    public int hCost;
    public int weight;
    public int nbMaxBeforeTurn;
    public Node from = null;
    public Point fromPos = new Point(-1,-1);
    public string ch;

    public int nbMinBeforeTurn;

    public int GetFCost(){
        return gCost+hCost;
    }

    public Node(Point p, int g, int h, int w, string c, int max, int min){
        pos = p;
        gCost = g;
        hCost = h;
        weight = w;
        ch=c;
        nbMaxBeforeTurn = max;
        nbMinBeforeTurn = min;
    }

    public bool Equals(Node otherNode){
        if(otherNode != null){
            return pos==otherNode.pos && nbMaxBeforeTurn==otherNode.nbMaxBeforeTurn && fromPos==otherNode.fromPos;
        }
        return false;
    }

    /*
    public int Compare(Node aNode, Node other){
        return aNode.GetFCost().CompareTo(other.GetFCost());
    }

    public bool Equals(Node? aNode, Node? otherNode)
    {
        if(aNode!=null && otherNode != null){
            return aNode.pos==otherNode.pos && aNode.nbMaxBeforeTurn==otherNode.nbMaxBeforeTurn && aNode.fromPos==otherNode.fromPos;
        }
        return false;
    }
    */  
    public int CompareTo(Node other)
    {
        return GetFCost().CompareTo(other.GetFCost());
    }
}
/*
public class Node : IEquatable<Node>, IComparable<Node>
{
    public Point pos;
    public int gCost;
    public int hCost;
    public int weight;
    public int nbMaxBeforeTurn;
    public Node from = null;
    public Point fromPos = new Point(-1, -1);
    public string ch;
    public int nbMinBeforeTurn;

    public int GetFCost() => gCost + hCost;

    public Node(Point p, int g, int h, int w, string c, int max, int min)
    {
        pos = p;
        gCost = g;
        hCost = h;
        weight = w;
        ch = c;
        nbMaxBeforeTurn = max;
        nbMinBeforeTurn = min;
    }

    public bool Equals(Node otherNode) =>
        otherNode != null &&
        pos == otherNode.pos &&
        nbMaxBeforeTurn == otherNode.nbMaxBeforeTurn &&
        fromPos == otherNode.fromPos;

    public int CompareTo(Node other) => GetFCost().CompareTo(other.GetFCost());
}
*/


class Program
{
    static string line;
    static double result = 0;
    static List<List<int>> weightedMap = new List<List<int>>();
    static int nbMaxBeforeTurnBase = 2;
    static int nbMinBeforeTurnBase = 0;
    static int nbMaxBeforeTurnUpgraded = 9;
    static int nbMinBeforeTurnUpgraded = 3;

    static Node GetLowestFCostNode(List<Node> nodeList)
    {
        Node lowestFCostNode = nodeList[0];
        for(int i = 1; i < nodeList.Count; i++)
        {
            if(nodeList[i].GetFCost() < lowestFCostNode.GetFCost())
            {
                lowestFCostNode = nodeList[i];
            }
        }

        return lowestFCostNode;
    }

    static bool InBound(int X, int Y){
        return X >= 0 && X < weightedMap[0].Count && Y >= 0 && Y < weightedMap.Count;
    }

    static bool SameDir(Point start, Point pos1, Point pos2)
    {
        if(pos1.X==pos2.X){
            return Math.Sign(start.Y - pos1.Y)==Math.Sign(start.Y - pos2.Y);
        }
        else if(pos1.Y==pos2.Y){
            return Math.Sign(start.X - pos1.X)==Math.Sign(start.X - pos2.X);
        }
        else{
            return false;
        }
    }

    static List<Node> GetNeighboursList(Node currentNode, bool upgraded){
        List<Node> neighboursList = new List<Node>();
        bool canMoveHorizontal = true;
        bool canMoveVertical = true;
        List<Point> neighbourPos = new List<Point>();
        //Check if we can or need to turn
        if(currentNode.nbMaxBeforeTurn==0){
            if(currentNode.from!=null && currentNode.pos.X != currentNode.from.pos.X){
                canMoveHorizontal = false;
            }
            if(currentNode.from!=null && currentNode.pos.Y != currentNode.from.pos.Y){
                canMoveVertical = false;
            }
        }
        if(currentNode.nbMinBeforeTurn>0){
            if(canMoveHorizontal && currentNode.from!=null && currentNode.pos.X == currentNode.from.pos.X){
                canMoveHorizontal = false;
            }
            if(canMoveVertical && currentNode.from!=null && currentNode.pos.Y == currentNode.from.pos.Y){
                canMoveVertical = false;
            }
        }

        int X = currentNode.pos.X;
        int Y = currentNode.pos.Y;
        int newX;
        int newY;
        int maxTurn = nbMaxBeforeTurnBase;
        int minTurn = nbMinBeforeTurnBase;
        if(upgraded){
            maxTurn = nbMaxBeforeTurnUpgraded;
            minTurn = nbMinBeforeTurnUpgraded;
        }
        int move;
        int nbminBTurn = 0;
        List<string> movesDir = new List<string>();
        if(canMoveHorizontal){
            move = 1;//(currentNode.from!=null && currentNode.pos.X != currentNode.from.pos.X)?1:1+minTurn;
            //nbminBTurn = (currentNode.from!=null && currentNode.pos.X != currentNode.from.pos.X)?Math.Max(currentNode.nbMinBeforeTurn-1,0):minTurn;
            neighbourPos.Add(new Point(X-move,Y));
            movesDir.Add("<");
            neighbourPos.Add(new Point(X+move,Y));
            movesDir.Add(">");
        }
        if(canMoveVertical){
            move = 1;//(currentNode.from!=null && currentNode.pos.Y != currentNode.from.pos.Y)?1:1+minTurn;
            //nbminBTurn = (currentNode.from!=null && currentNode.pos.Y != currentNode.from.pos.Y)?Math.Max(currentNode.nbMinBeforeTurn-1,0):minTurn;
            neighbourPos.Add(new Point(X,Y-move));
            movesDir.Add("^");
            neighbourPos.Add(new Point(X,Y+move));
            movesDir.Add("v");
        }
        for(int i = 0; i < neighbourPos.Count; i++){
            Point p = neighbourPos[i];
            newX = p.X;
            newY = p.Y;
            if(newX==currentNode.pos.X){
                move = Math.Abs(newY-currentNode.pos.Y);
            }
            else{
                move = Math.Abs(newX-currentNode.pos.X);
            }
            if(InBound(newX,newY) && (currentNode.from==null || !SameDir(currentNode.pos,currentNode.from.pos,p))){
                Node neighbourNode = new Node(new Point(newX,newY),int.MaxValue,(weightedMap.Count-1)-newY+(weightedMap[0].Count-1)-newX,weightedMap[newY][newX],movesDir[i],maxTurn,minTurn);
                neighbourNode.nbMaxBeforeTurn = SameLine(neighbourNode,currentNode.from)?currentNode.nbMaxBeforeTurn-move:maxTurn;
                neighbourNode.nbMinBeforeTurn = SameLine(neighbourNode,currentNode.from)?Math.Max(currentNode.nbMinBeforeTurn-1,0):minTurn;//nbminBTurn;//Math.Max(currentNode.nbMinBeforeTurn-1,0);
                neighbourNode.gCost = currentNode.gCost+neighbourNode.weight;//SumWeights(currentNode.pos, p);//neighbourNode.weight;
                neighbourNode.from = currentNode;
                neighbourNode.fromPos = currentNode.pos;//new Point(newX + Math.Sign(X-newX),newY + Math.Sign(Y-newY));
                neighboursList.Add(neighbourNode);
            }
        }
        return neighboursList;
    }

    

    static int SumWeights(Point pos, Point p)
    {
        int result = 0;
        for(int i=Math.Min(pos.X,p.X);i<=Math.Max(pos.X,p.X);i++){
            for(int j=Math.Min(pos.Y,p.Y);j<=Math.Max(pos.Y,p.Y);j++){
                if(i!=pos.X || j!=pos.Y){
                    result+=weightedMap[j][i];
                }
            }
        }
        return result;
    }

    static bool SameLine(Node neighbourNode, Node previousNode)
    {
        if(previousNode == null){
            return true;
        }

        return (neighbourNode.pos.X==previousNode.pos.X) || (neighbourNode.pos.Y==previousNode.pos.Y);
    }

    /*
    //SLOOOOOOOOOOOW as hell.
    //Even though part 1 is already slow without this
    static int IndexNodeInsideList(Node node, List<Node> list){
        if(node != null){
            for(int i=0; i<list.Count;i++){
                Node listNode = list[i];
                if(listNode.pos==node.pos){
                    ;
                }
                if(node.Equals(listNode)){
                    return i;
                }
            }
        }
        return -1;
    }
    */

    static List<Node> FindPath(Node startNode, Point endPos, bool upgraded){
        //SortedSet<Node> openedList = new SortedSet<Node>{startNode};
        List<Node> openedList = new List<Node>{startNode};
        HashSet<Node> closedList = new HashSet<Node>();
        //Dictionary<Tuple<Point,int,Point>, Node> openSetLookup = new Dictionary<Tuple<Point,int,Point>, Node>();

        startNode.gCost = 0;

        //openSetLookup[Tuple.Create(startNode.pos,startNode.nbMaxBeforeTurn,startNode.fromPos)] = startNode;

        while(openedList.Count > 0){
            Node currentNode = GetLowestFCostNode(openedList);
            //Tuple<Point,int,Point> keyLookup = Tuple.Create(currentNode.pos,currentNode.nbMaxBeforeTurn,currentNode.fromPos);
            /*
            Point fromPos = currentNode.from==null?new Point(-1,-1):currentNode.from.pos;
            string logLine = string.Format("Checking node at ({0},{1}), coming from ({4},{5}), with a heat loss of {2}. (Own heat loss is {3}, nb of jumps in same dir: max={6}, min={7})", 
            currentNode.pos.X, currentNode.pos.Y, currentNode.gCost, currentNode.weight,fromPos.X, fromPos.Y, currentNode.nbMaxBeforeTurn,currentNode.nbMinBeforeTurn);
            Console.WriteLine(logLine);
            //LogInOutput(logLine);
            */
            if(endPos == currentNode.pos){
                //Found the end, get the full path.
                //Console.WriteLine("Is end node !!");
                //Console.WriteLine("");
                List<Node> path = new List<Node>{currentNode};
                Node pathNode = currentNode;
                while(pathNode.from != null)
                {
                    path.Add(pathNode.from);
                    pathNode = pathNode.from;
                    //Console.WriteLine("Previous node at {0},{1}, with a heat loss of {2}. (Own heat loss is {3})", pathNode.pos.X, pathNode.pos.Y, pathNode.gCost, pathNode.weight);
                }
                path.Reverse();
                return path;
            }

            openedList.Remove(currentNode);
            closedList.Add(currentNode);
            //openSetLookup.Remove(keyLookup);

            foreach(Node neighbourNode in GetNeighboursList(currentNode,upgraded)){
                if(closedList.Contains(neighbourNode)){
                    continue;
                }
                /*
                Tuple<Point,int,Point> neighbourKeyLookup = Tuple.Create(neighbourNode.pos,neighbourNode.nbMaxBeforeTurn,neighbourNode.fromPos);
                if (!openSetLookup.ContainsKey(neighbourKeyLookup))
                {
                    openedList.Add(neighbourNode);
                    openSetLookup[neighbourKeyLookup] = neighbourNode;
                }*/
                
                
                if(!openedList.Contains(neighbourNode)){
                    openedList.Add(neighbourNode);
                }
                
            }
        }

        //No more nodes in opened list and no path found...
        return null;
    }

    static List<Node> FindPath(Point startPos, Point endPos, bool upgraded){
        int maxTurn = nbMaxBeforeTurnBase;
        int minTurn = nbMinBeforeTurnBase;
        if(upgraded){
            maxTurn = nbMaxBeforeTurnUpgraded;
            minTurn = nbMinBeforeTurnUpgraded;
        }
        Node startNode = new Node(startPos,int.MaxValue,(weightedMap.Count-1)+(weightedMap[0].Count-1),weightedMap[0][0],weightedMap[0][0].ToString(),maxTurn,minTurn);
        
        //return FindPathIA(startNode,endPos,upgraded);
        return FindPath(startNode,endPos,upgraded);
    }

    static List<Point> GetPointsFromPath(List<Node> path)
    {
        List<Point> points = new List<Point>();
        foreach(Node node in path){
            points.Add(node.pos);
        }
        return points;
    }

    static void Main(string[] args){
        try
        {
            //To get the time of code execution!
            var watch = System.Diagnostics.Stopwatch.StartNew();

            //Pass the file path and file name to the StreamReader constructor
            StreamReader sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\Input.txt");

            //Read the first line of text
            line = sr.ReadLine();

            //Continue to read until you reach end of file
            while (line != null)
            {
                //write the line to console window
                Console.WriteLine(line);

                List<int> weightedLine = new List<int>();
                foreach(char c in line){
                    weightedLine.Add(c-48);
                }
                weightedMap.Add(weightedLine);

                //Read the next line
                line = sr.ReadLine();
            }

            //close the file
            sr.Close();
            //Console.WriteLine("Grid size (X,Y): ({0},{1})",weightedMap[0].Count,weightedMap.Count);
            /*
            //DEBUG LOG
            Console.WriteLine("");
            Console.WriteLine("Weighted Map");
            foreach(List<int> wLine in weightedMap){
                foreach(int w in wLine){
                    Console.Write(w);
                }
                Console.WriteLine("");
            }
            */
            
            Console.WriteLine("");
            
            //PART 1
            //Get full path
            List<Node> path1 = FindPath(new Point(0,0), new Point(weightedMap[0].Count-1,weightedMap.Count-1),false);
            if(path1!=null && path1.Count>0){
                result = path1[path1.Count-1].gCost;
            }
            

            Console.WriteLine("");
            Console.WriteLine("End of input. Result game 1 found: {0}",result);
            
            /*
            //PART 2
            //Get full path
            List<Node> path2 = FindPath(new Point(0,0), new Point(weightedMap[0].Count-1,weightedMap.Count-1),true);
            if(path2!=null && path2.Count>0){
                result = path2[path2.Count-1].gCost;
            }

            Console.WriteLine("");
            Console.WriteLine("End of input. Result game 2 found: {0}",result);
            */
            /*
            //DEBUG LOG
            //foreach(Node node in path2){
            //    Console.WriteLine("We are at {0},{1}, with a heat loss of {2}. (Own heat loss is {3})", node.pos.X, node.pos.Y, node.gCost, node.weight);
            //}
            Console.WriteLine("");
            List<Point> pathPoints = GetPointsFromPath(path1);
            Console.WriteLine("Path Map");
            for(int i=0; i<weightedMap.Count;i++){
                List<int> wLine = weightedMap[i];
                for(int j=0; j<wLine.Count; j++){
                    int indx = pathPoints.IndexOf(new Point(j,i));
                    if(indx>=0){
                        Console.Write(path1[indx].ch);
                    }
                    else{
                        Console.Write(wLine[j]);
                    }
                }
                Console.WriteLine("");
            }
            */

            
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine();
            Console.WriteLine("Time of execution: {0} milliseconds",elapsedMs);

        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: " + e.ToString());
        }
        finally
        {
            Console.WriteLine("END");
        }
    }

    static void LogInOutput(string outLine){
        using (StreamWriter outputFile = new StreamWriter(Directory.GetCurrentDirectory()+"\\..\\..\\..\\DebugLogs.txt", true))
        {
            outputFile.WriteLine(outLine);
        }
    }

    
}