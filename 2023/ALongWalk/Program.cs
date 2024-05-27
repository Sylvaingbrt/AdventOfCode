﻿

using System.Drawing;


public class Node : IEquatable<Node>
{
    public Point pos;
    public int gCost;
    public int hCost;
    public Node from = null;
    public Point fromPos = new Point(-1,-1);

    public int GetFCost(){
        return gCost+hCost;
    }

    public Node(Point p, int g, int h){
        pos = p;
        gCost = g;
        hCost = h;
    }

    public bool Equals(Node otherNode){
        if(otherNode != null){
            return pos==otherNode.pos && fromPos==otherNode.fromPos && gCost==otherNode.gCost;
        }
        return false;
    }
}


class Program
{
    static string line;
    static double result = 0;
    static List<string> map = new List<string>();

    static Node GetHighestFCostNode(List<Node> nodeList)
    {
        Node lowestFCostNode = nodeList[0];
        for(int i = 1; i < nodeList.Count; i++)
        {
            if(nodeList[i].GetFCost() > lowestFCostNode.GetFCost())
            {
                lowestFCostNode = nodeList[i];
            }
        }

        return lowestFCostNode;
    }

    static bool InBound(int X, int Y){
        return X >= 0 && X < map[0].Length && Y >= 0 && Y < map.Count;
    }

    static bool CanWalk(int X, int Y, int fromX, int fromY){
        return InBound(X,Y) && (map[Y][X]=='.' || (map[Y][X]!='#' && ((X>fromX && map[Y][X]!='<') || (X<fromX && map[Y][X]!='>') || (Y>fromY && map[Y][X]!='^') || (Y<fromY && map[Y][X]!='v'))));
    }

    static List<Node> GetNeighboursList(Node currentNode){
        List<Node> neighboursList = new List<Node>();
        List<Point> neighbourPos = new List<Point>();

        int X = currentNode.pos.X;
        int Y = currentNode.pos.Y;
        int newX;
        int newY;
        
        if(map[Y][X]!='.'){
            switch(map[Y][X]){
                case '<':
                    neighbourPos.Add(new Point(X-1,Y));
                    break;
                case '>':
                    neighbourPos.Add(new Point(X+1,Y));
                    break;
                case '^':
                    neighbourPos.Add(new Point(X,Y-1));
                    break;
                case 'v':
                    neighbourPos.Add(new Point(X,Y+1));
                    break;
                default:
                    break;
            }
        }
        else{
            neighbourPos.Add(new Point(X-1,Y));
            neighbourPos.Add(new Point(X+1,Y));
            neighbourPos.Add(new Point(X,Y-1));
            neighbourPos.Add(new Point(X,Y+1));
        }


        for(int i = 0; i < neighbourPos.Count; i++){
            Point p = neighbourPos[i];
            newX = p.X;
            newY = p.Y;
            
            if(CanWalk(newX,newY,X,Y) && (currentNode.from==null || p!=currentNode.fromPos)){
                Node neighbourNode = new Node(new Point(newX,newY),currentNode.gCost-1,(map.Count-1)-newY+(map[0].Length-1)-newX);
                //neighbourNode.gCost = currentNode.gCost-1;
                neighbourNode.from = currentNode;
                neighbourNode.fromPos = currentNode.pos;//new Point(newX + Math.Sign(X-newX),newY + Math.Sign(Y-newY));
                neighboursList.Add(neighbourNode);
            }
        }
        return neighboursList;
    }

    static List<List<Node>> FindAllPath(Node startNode, Point endPos){
        List<List<Node>> allPaths = new List<List<Node>>();
        
        List<Node> openedList = new List<Node>{startNode};
        List<Node> closedList = new List<Node>();

        startNode.gCost = 0;

        while(openedList.Count > 0){
            Node currentNode = GetHighestFCostNode(openedList);
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
                //Console.WriteLine("Checked {1} nodes and {0} nodes waiting",openedList.Count-1,closedList.Count+1);
                while(pathNode.from != null)
                {
                    path.Add(pathNode.from);
                    pathNode = pathNode.from;
                    //Console.WriteLine("Previous node at {0},{1}, with a heat loss of {2}. (Own heat loss is {3})", pathNode.pos.X, pathNode.pos.Y, pathNode.gCost, pathNode.weight);
                }
                path.Reverse();
                allPaths.Add(path);
                openedList.Remove(currentNode);
            }
            else{
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
        }

        //No more nodes in opened list... all path founded?
        return allPaths;
    }

    static List<List<Node>> FindAllPath(Point startPos, Point endPos){
        Node startNode = new Node(startPos,0,(map.Count-1)+(map[0].Length-1));
        
        return FindAllPath(startNode,endPos);
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

                map.Add(line);

                //Read the next line
                line = sr.ReadLine();
            }

            //close the file
            sr.Close();
            
            //PART 1
            Point startPos = new Point(1,0);
            Point endPos = new Point(map[0].Length-2,map.Count-1);

            List<List<Node>> paths = FindAllPath(startPos, endPos);
            List<Node> path = new List<Node>();

            Console.WriteLine();
            Console.WriteLine("We found paths with following steps:");
            foreach(List<Node> aPath in paths){
                Console.WriteLine(aPath.Count-1);
                if(aPath.Count>path.Count){
                    path = aPath;
                }
            }

            /*
            //DEBUG LOG
            Console.WriteLine();
            List<Point> pathPoints = GetPointsFromPath(path);
            for(int i=0; i<map.Count;i++){
                string wLine = map[i];
                for(int j=0; j<wLine.Length; j++){
                    int indx = pathPoints.IndexOf(new Point(j,i));
                    if(indx>=0){
                        Console.Write('0');
                    }
                    else{
                        Console.Write(wLine[j]);
                    }
                }
                Console.WriteLine();
            }
            */
            
            if(path!=null && path.Count>0){
                //Console.WriteLine("At the end we have: {0}",path[path.Count-1].gCost);
                result = path.Count-1;
            }
            

            Console.WriteLine();
            Console.WriteLine("End of input. Result game 1 found: {0}",result);

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

    static void LogInOutput(string outLine,string fileName){
        using (StreamWriter outputFile = new StreamWriter(Directory.GetCurrentDirectory()+"\\..\\..\\..\\"+fileName+".txt", true))
        {
            outputFile.WriteLine(outLine);
        }
    }
}