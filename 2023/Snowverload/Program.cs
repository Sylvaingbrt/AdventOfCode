
string line;
double result = 0;
Dictionary<string, List<string>> nodeLinks = new Dictionary<string, List<string>>();
Dictionary<Tuple<string,string>, int> distances = new Dictionary<Tuple<string,string>, int>();
List<Tuple<string,string>> neighboursCouples = new List<Tuple<string,string>>();
List<int> groupsNumbers = new List<int>();


void FloydWarshall(List<string> allNodes){
    //List<string> allNodes = nodeLinks.Keys.ToList();
    for(int k = 0; k < allNodes.Count; k++){
        for(int i = 0; i < allNodes.Count; i++){
            Tuple<string,string> curIK = Tuple.Create(allNodes[i],allNodes[k]);
            if(distances.ContainsKey(curIK)){
                for(int j = i+1; j < allNodes.Count; j++){
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

List<string> GetGroupMembers(string member, Dictionary<string, List<string>> links){
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
            neighboursCouples.Add(Tuple.Create(refnode, node));
            neighboursCouples.Add(Tuple.Create(node, refnode));
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


    FloydWarshall(allNodes);

    //Now that we have the longest path of our graph, we can check in the middle, where do we have three "bridges" to cut to have two groups as equals as possible
    
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

    var pathsThroughBridges = distances.Where(x => x.Value>2);
    Dictionary<int, List<Tuple<string,string>>> coupleForLength = new Dictionary<int, List<Tuple<string, string>>>();
    int maxDist = 0;
    foreach(var dist in pathsThroughBridges){
        if(dist.Value>maxDist){
            maxDist = dist.Value;
        }
        if(!coupleForLength.ContainsKey(dist.Value)){
            coupleForLength[dist.Value] = new List<Tuple<string,string>>();
        }
        coupleForLength[dist.Value].Add(dist.Key);
    }

    //foreach(var dist in pathsThroughBridges){
    for(int i=maxDist; i>1; i--){
        if(!coupleForLength.ContainsKey(i)){
            continue;
        }
        int startBridgeDist = i%2==0?(i/2)-1:(i/2);
        int endBridgeDist = startBridgeDist+1;
        
        foreach(var couple in coupleForLength[i]){
            //Console.WriteLine();
            //Console.WriteLine("Searching 3 nodes at distance {0} from {1} and {2} from {3}", startBridgeDist,dist.Key.Item1,endBridgeDist,dist.Key.Item2);

            List<Tuple<string,string>> bridges = neighboursCouples.Where(x => 
                x.Item1!=couple.Item1 &&
                x.Item2!=couple.Item1 &&
                x.Item1!=couple.Item2 &&
                x.Item2!=couple.Item2 &&
                //distances[x]==1 &&
                distances.ContainsKey(Tuple.Create(couple.Item1,x.Item1)) && 
                distances.ContainsKey(Tuple.Create(couple.Item2,x.Item2)) &&
                distances[Tuple.Create(couple.Item1,x.Item1)]==startBridgeDist &&
                distances[Tuple.Create(couple.Item2,x.Item2)]==endBridgeDist
            ).ToList();

            if(bridges.Count==3){

                Dictionary<string,List<string>> newLinks = nodeLinks.ToDictionary(entry => entry.Key, entry => new List<string>(entry.Value));

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
            }

            if(groupsNumbers.Count>1){
                break;
            }
        }
        if(groupsNumbers.Count>1){
            break;
        }
    }

    result=1;
    foreach(int d in groupsNumbers){
        result *= d;

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


void LogInOutput(string outLine,string fileName){
    using (StreamWriter outputFile = new StreamWriter(Directory.GetCurrentDirectory()+"\\..\\..\\..\\"+fileName+".txt", true))
    {
        outputFile.WriteLine(outLine);
    }
}

