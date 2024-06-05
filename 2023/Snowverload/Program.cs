public class Node : IEquatable<Node>
{
    public string name;
    public int gCost;
    public Node from;
    public Node(string n, int g, Node f){
        name = n;
        gCost = g;
        from = f;
    }

    public bool Equals(Node otherNode){
        if(otherNode != null){
            return name.Equals(otherNode.name);// && from==otherNode.from;
        }
        return false;
    }
}

class Program
{
    static string line;
    static double result = 0;
    static Dictionary<string, List<string>> nodeLinks = new Dictionary<string, List<string>>();
    static Dictionary<Tuple<string,string>, int> distances = new Dictionary<Tuple<string,string>, int>();
    static List<int> groupsNumbers = new List<int>();

    static Node GetLowestGCostNode(List<Node> nodeList)
    {
        Node lowestFCostNode = nodeList[0];
        for(int i = 1; i < nodeList.Count; i++)
        {
            if(nodeList[i].gCost < lowestFCostNode.gCost)
            {
                lowestFCostNode = nodeList[i];
            }
        }

        return lowestFCostNode;
    }

    
    static List<Node> GetNeighboursList(Node currentNode){
        List<Node> neighboursList = new List<Node>();
        
        foreach(string neighbour in nodeLinks[currentNode.name]){
            if(currentNode.from==null || neighbour!=currentNode.from.name){
                Node neighbourNode = new Node(neighbour,currentNode.gCost+1,currentNode);
                neighboursList.Add(neighbourNode);
            }
        }
        return neighboursList;
    }

    static List<Node> FindPath(string start, string end){
        Node startNode = new Node(start,0,null);
        List<Node> openedList = new List<Node>{startNode};
        List<Node> closedList = new List<Node>();

        //startNode.gCost = 0;

        while(openedList.Count > 0){
            Node currentNode = GetLowestGCostNode(openedList);
            /*
            Point fromPos = currentNode.from==null?new Point(-1,-1):currentNode.from.pos;
            string logLine = string.Format("Checking node at ({0},{1}), coming from ({4},{5}), with a heat loss of {2}. (Own heat loss is {3}, nb of jumps in same dir: max={6}, min={7})", 
            currentNode.pos.X, currentNode.pos.Y, currentNode.gCost, currentNode.weight,fromPos.X, fromPos.Y, currentNode.nbMaxBeforeTurn,currentNode.nbMinBeforeTurn);
            Console.WriteLine(logLine);
            //LogInOutput(logLine);
            */
            if(end == currentNode.name){
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

    static void FloydWarshall(List<string> allNodes){
        //List<string> allNodes = nodeLinks.Keys.ToList();
        for(int k = 0; k < allNodes.Count; k++){
            for(int i = 0; i < allNodes.Count; i++){
                Tuple<string,string> curIK = Tuple.Create(allNodes[i],allNodes[k]);
                if(distances.ContainsKey(curIK)){
                    for(int j = 0; j < allNodes.Count; j++){
                        Tuple<string,string> curIJ = Tuple.Create(allNodes[i],allNodes[j]);
                        Tuple<string,string> curJK = Tuple.Create(allNodes[j],allNodes[k]);
                        if(distances.ContainsKey(curJK)){
                            if(!distances.ContainsKey(curIJ) || distances[curIJ] > distances[curIK] + distances[curJK]){
                                distances[curIJ] = distances[curIK] + distances[curJK];
                                distances[Tuple.Create(allNodes[j],allNodes[i])] = distances[curIK] + distances[curJK];
                            }
                        }
                    }
                }
            }
        }
    }

    static List<string> GetGroupMembers(string member, Dictionary<string, List<string>> links){
        Queue<string> openedList = new Queue<string>();
        List<string> closedList = new List<string>();

        openedList.Enqueue(member);

        while(openedList.Count > 0){
            string currentNode = openedList.Dequeue();
            /*
            //DEUBG LOG
            Point fromPos = currentNode.from==null?new Point(-1,-1):currentNode.from.pos;
            string logLine = string.Format("Checking node at ({0},{1}), coming from ({4},{5}), with a heat loss of {2}. (Own heat loss is {3}, nb of jumps in same dir: max={6}, min={7})", 
            currentNode.pos.X, currentNode.pos.Y, currentNode.gCost, currentNode.weight,fromPos.X, fromPos.Y, currentNode.nbMaxBeforeTurn,currentNode.nbMinBeforeTurn);
            Console.WriteLine(logLine);
            //LogInOutput(logLine);
            */
            
            closedList.Add(currentNode);

            foreach(string neighbourNode in links[currentNode]){
                if(closedList.Contains(neighbourNode)){
                    continue;
                }
                
                if(!openedList.Contains(neighbourNode)){
                    openedList.Enqueue(neighbourNode);
                }
                
                
            }
        }

        return closedList;
    }

    static void Main(string[] args){
        try
        {
            //To get the time of code execution!
            var watch = System.Diagnostics.Stopwatch.StartNew();

            bool test = false;

            //Pass the file path and file name to the StreamReader constructor
            StreamReader sr;
            if(test){
                sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\TestInput.txt");
            }
            else{
                sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\Input.txt");
            }
            
            

            //Read the first line of text
            line = sr.ReadLine();

            //Continue to read until you reach end of file
            while (line != null)
            {
                //write the line to console window
                Console.WriteLine(line);

                string refnode = line.Split(":")[0].Trim();
                string[] nodes = line.Split(":")[1].Trim().Split(" ");

                if(!nodeLinks.ContainsKey(refnode)){
                    nodeLinks[refnode] = new List<string>();
                }
                foreach(string node in nodes){
                    nodeLinks[refnode].Add(node);
                    if(!nodeLinks.ContainsKey(node)){
                        nodeLinks[node] = new List<string>();
                    }
                    nodeLinks[node].Add(refnode);

                    distances[Tuple.Create(refnode, node)] = 1;
                    distances[Tuple.Create(node, refnode)] = 1;
                }
                distances[Tuple.Create(refnode, refnode)] = 0;

                //Read the next line
                line = sr.ReadLine();
            }

            //close the file
            sr.Close();

            /*
            //DEBUG LOG
            Console.WriteLine();
            foreach(var link in nodeLinks){
                string others = "";
                foreach(string otherNode in link.Value){
                    others += otherNode + " ";
                }
                Console.WriteLine($"{link.Key}: {others}");
            }
            */

            List<string> allNodes = nodeLinks.Keys.ToList();
            /*
            for(int i = 0; i<allNodes.Count; i++){
                for(int j = i+1;j<allNodes.Count; j++){
                    if(!distances.ContainsKey(Tuple.Create(allNodes[i], allNodes[j]))){
                        List<Node> path = FindPath(allNodes[i],allNodes[j]);
                        for(int k=0; k<path.Count; k++){
                            for(int l = k+1; l<path.Count; l++){
                                int dist = l-k;
                                distances[Tuple.Create(path[k].name, path[l].name)] = dist;
                                distances[Tuple.Create(path[l].name, path[k].name)] = dist;
                            }
                        }
                        
                    }
                }
            }
            */
            FloydWarshall(allNodes);

            /*

            int maxDist = 0;
            Tuple<string,string> maxDistNodes = Tuple.Create("","");
            //Console.WriteLine();
            foreach(var dist in distances){
                //Console.WriteLine($"{dist.Key.Item1}->{dist.Key.Item2}: {dist.Value}");
                if(maxDist<dist.Value){
                    maxDist = dist.Value;
                    maxDistNodes = dist.Key;
                }
            }
            
            Console.WriteLine();
            Console.WriteLine("Max distance at {0}, between '{1}' and '{2}'", maxDist,maxDistNodes.Item1,maxDistNodes.Item2);

            int startBridgeDist = maxDist%2==0?(maxDist/2)-1:(maxDist/2);
            int endBridgeDist = startBridgeDist+1;
            Console.WriteLine("Searching 3 nodes at distance {0} from {1} and {2} from {3}", startBridgeDist,maxDistNodes.Item1,endBridgeDist,maxDistNodes.Item2);

            //Now that we have the longest path of our graph, we can check in the middle, where do we have three "bridges" to cut to have two groups as equals as possible
            List<Tuple<string,string>> bridges = distances.Keys.Where(x => 
                x.Item1!=maxDistNodes.Item1 &&
                x.Item2!=maxDistNodes.Item1 &&
                x.Item1!=maxDistNodes.Item2 &&
                x.Item2!=maxDistNodes.Item2 &&
                distances[x]==1 &&
                distances.ContainsKey(Tuple.Create(maxDistNodes.Item1,x.Item1)) && 
                distances.ContainsKey(Tuple.Create(maxDistNodes.Item2,x.Item2)) &&
                distances[Tuple.Create(maxDistNodes.Item1,x.Item1)]==startBridgeDist &&
                distances[Tuple.Create(maxDistNodes.Item2,x.Item2)]==endBridgeDist
            ).ToList();
            */
            /*
            //DEBUG LOG
            Console.WriteLine();
            string ex1 = "hfx";
            string ex2 = "pzl";
            Console.WriteLine("Exemple {0}-{1}: {2}", ex1,ex2,distances[Tuple.Create(ex1,ex2)]);
            Console.WriteLine("Exemple {0}-{1}: {2}", ex1,maxDistNodes.Item1,distances[Tuple.Create(ex1,maxDistNodes.Item1)]);
            Console.WriteLine("Exemple {0}-{1}: {2}", ex2,maxDistNodes.Item2,distances[Tuple.Create(ex2,maxDistNodes.Item2)]);
            
            foreach(var bridge in bridges){
                Console.WriteLine("{0}-{1}", bridge.Item1,bridge.Item2);
            }
            */
            /*
            if(bridges.Count!=3){
                bridges = distances.Keys.Where(x => 
                    x.Item1!=maxDistNodes.Item1 &&
                    x.Item2!=maxDistNodes.Item1 &&
                    x.Item1!=maxDistNodes.Item2 &&
                    x.Item2!=maxDistNodes.Item2 &&
                    distances[x]==1 &&
                    distances.ContainsKey(Tuple.Create(maxDistNodes.Item2,x.Item1)) && 
                    distances.ContainsKey(Tuple.Create(maxDistNodes.Item1,x.Item2)) &&
                    distances[Tuple.Create(maxDistNodes.Item2,x.Item1)]==startBridgeDist &&
                    distances[Tuple.Create(maxDistNodes.Item1,x.Item2)]==endBridgeDist
                ).ToList();
            }
            */

            foreach(var dist in distances.Where(x => x.Value>2)){
                int startBridgeDist = dist.Value%2==0?(dist.Value/2)-1:(dist.Value/2);
                int endBridgeDist = startBridgeDist+1;
                
                //Console.WriteLine();
                //Console.WriteLine("Searching 3 nodes at distance {0} from {1} and {2} from {3}", startBridgeDist,dist.Key.Item1,endBridgeDist,dist.Key.Item2);

                List<Tuple<string,string>> bridges = distances.Keys.Where(x => 
                    x.Item1!=dist.Key.Item1 &&
                    x.Item2!=dist.Key.Item1 &&
                    x.Item1!=dist.Key.Item2 &&
                    x.Item2!=dist.Key.Item2 &&
                    distances[x]==1 &&
                    distances.ContainsKey(Tuple.Create(dist.Key.Item1,x.Item1)) && 
                    distances.ContainsKey(Tuple.Create(dist.Key.Item2,x.Item2)) &&
                    distances[Tuple.Create(dist.Key.Item1,x.Item1)]==startBridgeDist &&
                    distances[Tuple.Create(dist.Key.Item2,x.Item2)]==endBridgeDist
                ).ToList();

                if(bridges.Count==3){

                    Dictionary<string,List<string>> newLinks = new Dictionary<string,List<string>>(nodeLinks);

                    //Cut the bridges!
                    foreach(var bridge in bridges){
                        //Console.WriteLine("Cutting the ropes of bridge {0}-{1}",bridge.Item1,bridge.Item2);
                        newLinks[bridge.Item1].Remove(bridge.Item2);
                        newLinks[bridge.Item2].Remove(bridge.Item1);
                    }
                    //Console.WriteLine();
                    List<string> seenMembers = new List<string>();
                    groupsNumbers.Clear();
                    foreach(string node in allNodes){
                        if(!seenMembers.Contains(node)){
                            //Console.WriteLine("New group for key: {0}",node);
                            List<string> aGroup = GetGroupMembers(node,newLinks);
                            groupsNumbers.Add(aGroup.Count);
                            seenMembers.AddRange(aGroup);
                        }
                    }

                    int newResult = 1;
                    foreach(int i in groupsNumbers){
                        newResult *= i;
                    }

                    if(result<newResult){
                        //Console.WriteLine("New best result: from {0} to {1}",result,newResult);
                        result = newResult;
                    }
                }
            }

            /*
            Console.WriteLine();
            if(bridges.Count!=3){
                Console.WriteLine("ERROR ?? bridges: {0}",bridges.Count);
            }
            else{
                //Cut the bridges!
                foreach(var bridge in bridges){
                    Console.WriteLine("Cutting the ropes of bridge {0}-{1}",bridge.Item1,bridge.Item2);
                    nodeLinks[bridge.Item1].Remove(bridge.Item2);
                    nodeLinks[bridge.Item2].Remove(bridge.Item1);
                }
            }

            Console.WriteLine();
            result = 1;
            List<string> seenMembers = new List<string>();
            foreach(string node in allNodes){
                if(!seenMembers.Contains(node)){
                    Console.WriteLine("New group for key: {0}",node);
                    List<string> aGroup = GetGroupMembers(node);
                    groupsNumbers.Add(aGroup.Count);
                    seenMembers.AddRange(aGroup);
                }
            }

            foreach(int i in groupsNumbers){
                result *= i;
            }
            */
            
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

