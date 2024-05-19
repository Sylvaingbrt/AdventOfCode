//IT'S FINALLY TIME TO USE A* ALGORITHM! (Wait is it Dijkstra in the end?)
//Classic but never disappoint.

//This problem shows the limit of A* logic. It works but takes a long time: ~4min for part 1 / ~45min for part 2

using System.Drawing;

public class Node : IEquatable<Node>
{
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
}


class Program
{
    static string line;
    static double result = 0;
    static List<List<int>> weightedMap = new List<List<int>>();
    static int nbMaxBeforeTurnBase = 3;
    static int nbMinBeforeTurnBase = 0;
    static int nbMaxBeforeTurnUpgraded = 10;
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
        int move = 1;
        int nbminBTurn = 0;
        List<string> movesDir = new List<string>();
        if(canMoveHorizontal){
            move = (currentNode.from!=null && currentNode.pos.X != currentNode.from.pos.X)?1:1+minTurn;
            neighbourPos.Add(new Point(X-move,Y));
            movesDir.Add("<");
            neighbourPos.Add(new Point(X+move,Y));
            movesDir.Add(">");
        }
        if(canMoveVertical){
            move = (currentNode.from!=null && currentNode.pos.Y != currentNode.from.pos.Y)?1:1+minTurn;
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
            
            if(InBound(newX,newY) && (currentNode.from==null || p!=currentNode.fromPos)){
                Node neighbourNode = new Node(new Point(newX,newY),int.MaxValue,(weightedMap.Count-1)-newY+(weightedMap[0].Count-1)-newX,weightedMap[newY][newX],movesDir[i],maxTurn,minTurn);
                neighbourNode.nbMaxBeforeTurn = SameLine(neighbourNode,currentNode.from)?currentNode.nbMaxBeforeTurn-move:maxTurn-move;
                neighbourNode.nbMinBeforeTurn = SameLine(neighbourNode,currentNode.from)?Math.Max(currentNode.nbMinBeforeTurn-move,0):Math.Max(minTurn-move,0);
                neighbourNode.gCost = currentNode.gCost+SumWeights(currentNode.pos, p);
                neighbourNode.from = currentNode;
                neighbourNode.fromPos = new Point(newX + Math.Sign(X-newX),newY + Math.Sign(Y-newY));
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

    static List<Node> FindPath(Node startNode, Point endPos, bool upgraded){
        List<Node> openedList = new List<Node>{startNode};
        List<Node> closedList = new List<Node>();

        startNode.gCost = 0;

        while(openedList.Count > 0){
            Node currentNode = GetLowestFCostNode(openedList);
            /*
            Point fromPos = currentNode.from==null?new Point(-1,-1):currentNode.from.pos;
            string logLine = string.Format("Checking node at ({0},{1}), coming from ({4},{5}), with a heat loss of {2}. (Own heat loss is {3}, nb of jumps in same dir: max={6}, min={7})", 
            currentNode.pos.X, currentNode.pos.Y, currentNode.gCost, currentNode.weight,fromPos.X, fromPos.Y, currentNode.nbMaxBeforeTurn,currentNode.nbMinBeforeTurn);
            Console.WriteLine(logLine);
            //LogInOutput(logLine);
            */
            if(endPos == currentNode.pos){
                //Found the end, get the full path.
                List<Node> path = new List<Node>{currentNode};
                Node pathNode = currentNode;
                Console.WriteLine("Checked {1} nodes and {0} nodes waiting",openedList.Count-1,closedList.Count+1);
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

            foreach(Node neighbourNode in GetNeighboursList(currentNode,upgraded)){
                if(closedList.Contains(neighbourNode)){
                    continue;
                }
                
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
            
            Console.WriteLine("");
            /*
            //PART 1
            //Get full path
            List<Node> path1 = FindPath(new Point(0,0), new Point(weightedMap[0].Count-1,weightedMap.Count-1),false);
            if(path1!=null && path1.Count>0){
                result = path1[path1.Count-1].gCost;
            }
            

            Console.WriteLine("");
            Console.WriteLine("End of input. Result game 1 found: {0}",result);
            */
            
            //PART 2
            //Get full path
            List<Node> path2 = FindPath(new Point(0,0), new Point(weightedMap[0].Count-1,weightedMap.Count-1),true);
            if(path2!=null && path2.Count>0){
                result = path2[path2.Count-1].gCost;
            }
            
            Console.WriteLine("");
            Console.WriteLine("End of input. Result game 2 found: {0}",result);
            
            /*
            //DEBUG LOG
            //foreach(Node node in path2){
            //    Console.WriteLine("We are at {0},{1}, with a heat loss of {2}. (Own heat loss is {3})", node.pos.X, node.pos.Y, node.gCost, node.weight);
            //}
            Console.WriteLine("");
            List<Point> pathPoints = GetPointsFromPath(path2);
            Console.WriteLine("Path Map");
            for(int i=0; i<weightedMap.Count;i++){
                List<int> wLine = weightedMap[i];
                for(int j=0; j<wLine.Count; j++){
                    int indx = pathPoints.IndexOf(new Point(j,i));
                    if(indx>=0){
                        Console.Write(path2[indx].ch);
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