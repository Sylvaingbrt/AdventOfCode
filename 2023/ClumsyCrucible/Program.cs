//IT'S FINALLY TIME TO USE A* ALGORITHM! (Wait is it Dijkstra in the end?)
//Classic but never disappoint.

using System.Drawing;

public class Node{
    //We use Node here as a class and not a struct to check for null nodes
    public Point pos;
    public int gCost;
    public int hCost;
    public int weight;
    public int nbBeforeTurn = 2;
    public Point fromPos; //If we had a Node here to put the previous Node instead of the Position, we could not have "Node" as a struct instead of a class.
    public Node fromNode = null;
    public string ch;

    public int GetFCost(){
        return gCost+hCost;
    }

    public Node(Point p, int g, int h, int w, Point f, string c){
        pos = p;
        gCost = g;
        hCost = h;
        weight = w;
        fromPos = f;
        ch=c;
    }
}


class Program
{
    static string line;
    static double result1 = 0;
    static List<List<int>> weightedMap = new List<List<int>>();
    static List<List<List<Node>>> nodeMap = new List<List<List<Node>>>();

    static Node GetNode(Point pos, Point from){
        if(pos.X >= 0 && pos.X < nodeMap[0].Count && pos.Y >= 0 && pos.Y < nodeMap.Count){
            List<Node> nodesAtPoint = nodeMap[pos.Y][pos.X];
            foreach(Node node in nodesAtPoint){
                if(node.fromPos == from){
                    return node;
                }
            }
        }
        return null;
    }

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
            if(currentNode.pos.X != currentNode.fromPos.X){
                canMoveHorizontal = false;
            }
            if(currentNode.pos.Y != currentNode.fromPos.Y){
                canMoveVertical = false;
            }
        }

        if(canMoveHorizontal && currentNode.pos.X -1 >= 0 && currentNode.pos.X-currentNode.fromPos.X!=1){
            //Left
            neighboursList.Add(GetNode(new Point(currentNode.pos.X-1,currentNode.pos.Y),currentNode.pos));
        }
        if(canMoveHorizontal && currentNode.pos.X+1 < nodeMap[0].Count && currentNode.pos.X-currentNode.fromPos.X!=-1){
            //Right
            neighboursList.Add(GetNode(new Point(currentNode.pos.X+1,currentNode.pos.Y),currentNode.pos));
        }
        if(canMoveVertical && currentNode.pos.Y -1 >= 0 && currentNode.pos.Y-currentNode.fromPos.Y!=1){
            //Up
            neighboursList.Add(GetNode(new Point(currentNode.pos.X,currentNode.pos.Y-1),currentNode.pos));
        }
        if(canMoveVertical && currentNode.pos.Y+1 < nodeMap.Count && currentNode.pos.Y-currentNode.fromPos.Y!=-1){
            //Down
            neighboursList.Add(GetNode(new Point(currentNode.pos.X,currentNode.pos.Y+1),currentNode.pos));
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


    static List<Node> FindPath(Node startNode, List<Node> endNodes){
        List<Node> openedList = new List<Node>{startNode};
        List<Node> closedList = new List<Node>();

        startNode.gCost = 0;

        while(openedList.Count > 0){
            Node currentNode = GetLowestFCostNode(openedList);
            string logLine = string.Format("Checking node at ({0},{1}), coming from ({4},{5}), with a heat loss of {2}. (Own heat loss is {3}, nb of jumps in same dir: {6})", 
            currentNode.pos.X, currentNode.pos.Y, currentNode.gCost, currentNode.weight,currentNode.fromPos.X, currentNode.fromPos.Y, currentNode.nbBeforeTurn);
            Console.WriteLine(logLine);
            LogInOutput(logLine);
            if(endNodes.Contains(currentNode)){
                //Found the end, get the full path.
                Console.WriteLine("Is end node !!");
                Console.WriteLine("");
                List<Node> path = new List<Node>{currentNode};
                Node pathNode = currentNode;
                while(pathNode.fromNode != null)
                {
                    path.Add(pathNode.fromNode);
                    pathNode = pathNode.fromNode;
                    Console.WriteLine("Previous node at {0},{1}, with a heat loss of {2}. (Own heat loss is {3})", pathNode.pos.X, pathNode.pos.Y, pathNode.gCost, pathNode.weight);
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
                int potentialGCost = currentNode.gCost + neighbourNode.weight;
                int potentialNbBeforeTurn = SameLine(neighbourNode,currentNode.fromNode)?currentNode.nbBeforeTurn-1:2;
                if(potentialGCost < neighbourNode.gCost || (potentialGCost==neighbourNode.gCost && potentialNbBeforeTurn > neighbourNode.nbBeforeTurn)){
                    neighbourNode.fromNode = currentNode;
                    neighbourNode.gCost = potentialGCost;
                    neighbourNode.nbBeforeTurn = potentialNbBeforeTurn;
                    if(!openedList.Contains(neighbourNode)){
                        openedList.Add(neighbourNode);
                    }
                }
            }
        }

        //No more nodes in opened list and no path found...
        return null;
    }

    static List<Node> FindPath(Point startPos, Point endPos){
        Node startNode = GetNode(startPos,new Point(-1,-1));
        List<Node> endNodes = nodeMap[endPos.Y][endPos.X];

        return FindPath(startNode,endNodes);
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

            for(int i = 0 ; i < weightedMap.Count; i++){
                List<List<Node>> nodeLine = new List<List<Node>>();
                for(int j = 0 ; j < weightedMap[i].Count; j++){
                    List<Node> nodesAtPoint = new List<Node>();
                    if(i==0 && j==0){
                        //Start Node, with theoric previous node at -1,-1
                        nodesAtPoint.Add(new Node(new Point(j,i),int.MaxValue,weightedMap.Count-i+weightedMap[i].Count-j-2,weightedMap[i][j],new Point(-1,-1),weightedMap[0][0].ToString()));
                    }
                    if(i-1>=0){
                        nodesAtPoint.Add(new Node(new Point(j,i),int.MaxValue,weightedMap.Count-i+weightedMap[i].Count-j-2,weightedMap[i][j],new Point(j,i-1),"v"));
                    }
                    if(i+1<weightedMap.Count){
                        nodesAtPoint.Add(new Node(new Point(j,i),int.MaxValue,weightedMap.Count-i+weightedMap[i].Count-j-2,weightedMap[i][j],new Point(j,i+1),"^"));
                    }
                    if(j-1>=0){
                        nodesAtPoint.Add(new Node(new Point(j,i),int.MaxValue,weightedMap.Count-i+weightedMap[i].Count-j-2,weightedMap[i][j],new Point(j-1,i),">"));
                    }   
                    if(j+1<weightedMap[i].Count){
                        nodesAtPoint.Add(new Node(new Point(j,i),int.MaxValue,weightedMap.Count-i+weightedMap[i].Count-j-2,weightedMap[i][j],new Point(j+1,i),"<"));
                    }
                    nodeLine.Add(nodesAtPoint);
                }
                nodeMap.Add(nodeLine);
            }
            
            Console.WriteLine("");

            //Get full path
            List<Node> path = FindPath(new Point(0,0), new Point(weightedMap[0].Count-1,weightedMap.Count-1));
            result1 = path[path.Count-1].gCost;

            Console.WriteLine("");
            Console.WriteLine("End of input. Result game 1 found: {0}",result1);

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