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
    public int nbBeforeTurn = 2;
    public Node from = null;
    public Point fromPos = new Point(-1,-1);
    public string ch;

    public int GetFCost(){
        return gCost+hCost;
    }

    public Node(Point p, int g, int h, int w, string c){
        pos = p;
        gCost = g;
        hCost = h;
        weight = w;
        ch=c;
    }

    public bool Equals(Node otherNode){
        if(otherNode != null){
            return pos==otherNode.pos && nbBeforeTurn==otherNode.nbBeforeTurn && fromPos==otherNode.fromPos;
        }
        return false;
    }
}


class Program
{
    static string line;
    static double result1 = 0;
    static List<List<int>> weightedMap = new List<List<int>>();

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

    static List<Node> GetNeighboursList(Node currentNode){
        List<Node> neighboursList = new List<Node>();
        bool canMoveHorizontal = true;
        bool canMoveVertical = true;
        //Check if we need to turn
        if(currentNode.nbBeforeTurn==0){
            if(currentNode.from!=null && currentNode.pos.X != currentNode.from.pos.X){
                canMoveHorizontal = false;
            }
            if(currentNode.from!=null && currentNode.pos.Y != currentNode.from.pos.Y){
                canMoveVertical = false;
            }
        }

        int X = currentNode.pos.X;
        int Y = currentNode.pos.Y;
        int newX;
        int newY;
        if(canMoveHorizontal && currentNode.pos.X -1 >= 0 && (currentNode.from==null || currentNode.pos.X-currentNode.from.pos.X!=1)){
            //Left
            newX = X-1;
            newY = Y;
            Node leftNeighbour = new Node(new Point(newX,newY),int.MaxValue,(weightedMap.Count-1)-newY+(weightedMap[0].Count-1)-newX,weightedMap[newY][newX],"<");
            leftNeighbour.nbBeforeTurn = SameLine(leftNeighbour,currentNode.from)?currentNode.nbBeforeTurn-1:2;
            leftNeighbour.gCost = currentNode.gCost+leftNeighbour.weight;
            leftNeighbour.from = currentNode;
            leftNeighbour.fromPos = currentNode.pos;
            neighboursList.Add(leftNeighbour);
        }
        if(canMoveHorizontal && currentNode.pos.X+1 < weightedMap[0].Count && (currentNode.from==null || currentNode.pos.X-currentNode.from.pos.X!=-1)){
            //Right
            newX = X+1;
            newY = Y;
            Node rightNeighbour = new Node(new Point(newX,newY),int.MaxValue,(weightedMap.Count-1)-newY+(weightedMap[0].Count-1)-newX,weightedMap[newY][newX],">");
            rightNeighbour.nbBeforeTurn = SameLine(rightNeighbour,currentNode.from)?currentNode.nbBeforeTurn-1:2;
            rightNeighbour.gCost = currentNode.gCost+rightNeighbour.weight;
            rightNeighbour.from = currentNode;
            rightNeighbour.fromPos = currentNode.pos;
            neighboursList.Add(rightNeighbour);
        }
        if(canMoveVertical && currentNode.pos.Y -1 >= 0 && (currentNode.from==null || currentNode.pos.Y-currentNode.from.pos.Y!=1)){
            //Up
            newX = X;
            newY = Y-1;
            Node upNeighbour = new Node(new Point(newX,newY),int.MaxValue,(weightedMap.Count-1)-newY+(weightedMap[0].Count-1)-newX,weightedMap[newY][newX],"^");
            upNeighbour.nbBeforeTurn = SameLine(upNeighbour,currentNode.from)?currentNode.nbBeforeTurn-1:2;
            upNeighbour.gCost = currentNode.gCost+upNeighbour.weight;
            upNeighbour.from = currentNode;
            upNeighbour.fromPos = currentNode.pos;
            neighboursList.Add(upNeighbour);
        }
        if(canMoveVertical && currentNode.pos.Y+1 < weightedMap.Count && (currentNode.from==null || currentNode.pos.Y-currentNode.from.pos.Y!=-1)){
            //Down
            newX = X;
            newY = Y+1;
            Node downNeighbour = new Node(new Point(newX,newY),int.MaxValue,(weightedMap.Count-1)-newY+(weightedMap[0].Count-1)-newX,weightedMap[newY][newX],"v");
            downNeighbour.nbBeforeTurn = SameLine(downNeighbour,currentNode.from)?currentNode.nbBeforeTurn-1:2;
            downNeighbour.gCost = currentNode.gCost+downNeighbour.weight;
            downNeighbour.from = currentNode;
            downNeighbour.fromPos = currentNode.pos;
            neighboursList.Add(downNeighbour);
        }

        return neighboursList;
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
    //Even though part 1 is slow without this
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

    static List<Node> FindPath(Node startNode, Point endPos){
        List<Node> openedList = new List<Node>{startNode};
        List<Node> closedList = new List<Node>();

        startNode.gCost = 0;

        while(openedList.Count > 0){
            Node currentNode = GetLowestFCostNode(openedList);
            /*
            Point fromPos = currentNode.from==null?new Point(-1,-1):currentNode.from.pos;
            string logLine = string.Format("Checking node at ({0},{1}), coming from ({4},{5}), with a heat loss of {2}. (Own heat loss is {3}, nb of jumps in same dir: {6})", 
            currentNode.pos.X, currentNode.pos.Y, currentNode.gCost, currentNode.weight,fromPos.X, fromPos.Y, currentNode.nbBeforeTurn);
            Console.WriteLine(logLine);
            LogInOutput(logLine);
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

            foreach(Node neighbourNode in GetNeighboursList(currentNode)){
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

    static List<Node> FindPath(Point startPos, Point endPos){
        Node startNode = new Node(startPos,int.MaxValue,(weightedMap.Count-1)+(weightedMap[0].Count-1),weightedMap[0][0],weightedMap[0][0].ToString());

        return FindPath(startNode,endPos);
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
            
            //Console.WriteLine("");
            //Get full path
            List<Node> path = FindPath(new Point(0,0), new Point(weightedMap[0].Count-1,weightedMap.Count-1));
            result1 = path[path.Count-1].gCost;

            Console.WriteLine("");
            Console.WriteLine("End of input. Result game 1 found: {0}",result1);

            /*
            //DEBUG LOG
            foreach(Node node in path){
                Console.WriteLine("We are at {0},{1}, with a heat loss of {2}. (Own heat loss is {3})", node.pos.X, node.pos.Y, node.gCost, node.weight);
            }
            Console.WriteLine("");
            List<Point> pathPoints = GetPointsFromPath(path);
            Console.WriteLine("Path Map");
            for(int i=0; i<weightedMap.Count;i++){
                List<int> wLine = weightedMap[i];
                for(int j=0; j<wLine.Count; j++){
                    int indx = pathPoints.IndexOf(new Point(j,i));
                    if(indx>=0){
                        Console.Write(path[indx].ch);
                    }
                    else{
                        Console.Write(wLine[j]);
                    }
                }
                Console.WriteLine("");
            }
            */

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