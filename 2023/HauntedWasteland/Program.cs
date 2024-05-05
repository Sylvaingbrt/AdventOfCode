using System.Globalization;

string line;
string lrSequence;
string currentNode = "AAA";
double result1 = 0;
Dictionary<string, List<string>> nodes = new Dictionary<string, List<string>>();


//Part 2
List<string> currentNodes = new List<string>();
List<double> currentNodesSteps = new List<double>();
Dictionary<string, List<int>> nodesOccurenceInSequence = new Dictionary<string, List<int>>();
Dictionary<string, List<int>> nodesOccurenceSteps = new Dictionary<string, List<int>>();
List<int> loopValues = new List<int>();
double result2 = 1;

List<string> GetListNodes(string input){
    List<string> list = new List<string>
    {
        input.Split(',')[0].Trim().Replace("(", ""),
        input.Split(',')[1].Trim().Replace(")", "")
    };
    return list;
}

double LCM(double numA, double numB){
    //We will use the Euclide algorithm. We know that LCM(a,b) = a*b/GCD(a,b) with GCD the greatest common divisor.
    double a = Math.Max(numA, numB);
    double b = Math.Min(numA,numB);
    double c = a%b;
    while(c!=0){
        a=b;
        b=c;
        c = a%b;
    }
    return numA*numB/b;
}

try
{
    //Pass the file path and file name to the StreamReader constructor
    StreamReader sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\Input.txt");

    //Get first line with left-right sequence
    lrSequence = sr.ReadLine();
    Console.WriteLine(lrSequence);


    //Get line
    line = sr.ReadLine();


    //Continue to read until you reach end of file
    while (line != null)
    {
        //write the line to console window
        Console.WriteLine(line);

        if(line.Length>0){
            //Get nodes
            string node = line.Split("=")[0].Trim();
            if(node[node.Length-1] == 'A'){
                currentNodes.Add(node);
                currentNodesSteps.Add(0);
            }
            nodes[node] = GetListNodes(line.Split("=")[1]);
        }

        //Read the next line
        line = sr.ReadLine();
    }

    //close the file
    sr.Close();

    while(currentNode!="ZZZ"){
        for(int i = 0; i < lrSequence.Length; i++){
            //Console.WriteLine("Currently at {0}, moving {1} in ({2},{3})",currentNode,lrSequence[i],nodes[currentNode][0],nodes[currentNode][1]);
            result1+=1;
            if(lrSequence[i]=='L'){
                currentNode = nodes[currentNode][0];
            }
            else{
                currentNode = nodes[currentNode][1];
            }
            if(currentNode=="ZZZ"){
                break;
            }
        }
    }
    

    Console.WriteLine("End of input. Result game 1 found: {0}",result1);


    //For part 2, after having try this "naive" solution by looping for all nodes until we have a mach for all, which took hours, I decided to search for the "least common multiple" (lcm) after finding the first occurence of each Z nodes and the loop length.
    //Then we take the lowest lcm of all.
    //(Post solution edit: it was a good call, the number found tells that it would have taken me at least DAYS with the naive solution)

    int lastIndexString = currentNodes[0].Length-1;
    int loopSize = 0;

    for(int j = 0;j < currentNodes.Count; j++){
        //Console.WriteLine("Starting at {0} = ({1},{2})",currentNodes[j],nodes[currentNodes[j]][0],nodes[currentNodes[j]][1]);
        bool loopFound = false;
        int steps = 0;
        nodesOccurenceInSequence.Clear();
        nodesOccurenceSteps.Clear();
        while(!loopFound){
            for(int i = 0; i < lrSequence.Length; i++){
                steps++;
                if(lrSequence[i]=='L'){
                    currentNodes[j] = nodes[currentNodes[j]][0];
                }
                else{
                    currentNodes[j] = nodes[currentNodes[j]][1];
                }

                if(currentNodes[j][lastIndexString] == 'Z'){
                    //indexInLR = steps % lrSequence.Length;
                    if(nodesOccurenceInSequence.ContainsKey(currentNodes[j])){
                        int id = nodesOccurenceInSequence[currentNodes[j]].FindIndex(a => a==i);
                        if(id>-1){
                            loopSize = steps-nodesOccurenceSteps[currentNodes[j]][id];
                            loopFound = true;
                        }
                        else{
                            nodesOccurenceInSequence[currentNodes[j]].Add(i);
                            nodesOccurenceSteps[currentNodes[j]].Add(steps);
                        }
                    }
                    else{
                        nodesOccurenceInSequence[currentNodes[j]] = new List<int>{i};
                        nodesOccurenceSteps[currentNodes[j]] = new List<int>{steps};
                    }
                }
                


                if(loopFound){
                    /*
                    //We find here that the given nodes loop perfectly well. It is an easier case than the general one were we could have "loop = initialStep + k*loopSize". It simplifies the following calculs
                    Console.WriteLine("Found loop for node {0}, after {1} steps. It's at index {2} in the lr sequence of total size {3}",j,steps,i,lrSequence.Length);
                    foreach(var occurence in nodesOccurenceInSequence){
                        string ids = "";
                        foreach(int k in occurence.Value){
                            ids += k.ToString() + ",";
                        }
                        Console.WriteLine("We get {0} at index: {1}. First occurence after {3} steps, with a loop of size {2}",occurence.Key ,ids, loopSize, nodesOccurenceSteps[occurence.Key][0]);
                    }
                    */
                    loopValues.Add(loopSize);
                    break;
                }
            }
        }
    }

    //We now have to search the lcm of our list of loopValues
    foreach(int i in loopValues){
        result2 = LCM(i,result2);
    }

    result2+=currentNodesSteps[0];
    Console.WriteLine("End of input. Result game 2 found: {0}",result2);
}
catch(Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}
finally
{
    Console.WriteLine("END");
}