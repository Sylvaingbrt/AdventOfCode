string line;
double result = 0;
Dictionary<string,List<Tuple<char,char,int,string>>> workflows = new Dictionary<string,List<Tuple<char,char,int,string>>>();
List<List<int>> parts = new List<List<int>>();
Dictionary<char,int> categories = new Dictionary<char,int>{
    {'x',0},
    {'m',1},
    {'a',2},
    {'s',3}
    };


//For part 2
Dictionary<string,List<int[]>> possibleValueForWorkflows = new Dictionary<string,List<int[]>>();
Dictionary<string,double> possibleCountForWorkflows = new Dictionary<string,double>();
Dictionary<string,List<List<int[]>>> possibleIntervallsForWorkflows = new Dictionary<string,List<List<int[]>>>();


double ResultFromWorkflows(List<int> part,string workflowKey){
    if(workflowKey=="R"){
        return 0;
    }
    else if(workflowKey=="A"){
        double result = 0;
        foreach(int val in part){
            result += val;
        }
        return result;
    }
    else{
        List<Tuple<char,char,int,string>> workflow = workflows[workflowKey];
        string nextKey = "";
        foreach(Tuple<char,char,int,string> rule in workflow){
            if(rule.Item1 == '0'){
                nextKey = rule.Item4;
                break;
            }
            else{
                if(rule.Item2 == '>'){
                    if(part[categories[rule.Item1]]>rule.Item3){
                        nextKey = rule.Item4;
                        break;
                    }
                }
                else{
                    if(part[categories[rule.Item1]]<rule.Item3){
                        nextKey = rule.Item4;
                        break;
                    }
                }
            }
        }
        return ResultFromWorkflows(part,nextKey);
    }
    //Console.WriteLine("Nothing worked... weird.");
    //return 0;
}

List<int[]> RecursiveFindPossibilitiesInWorkflow(string workflowKey){
    if(!possibleValueForWorkflows.ContainsKey(workflowKey)){
        List<int[]> possibilities = new List<int[]>{new int[2]{0,4000},new int[2]{0,4000},new int[2]{0,4000},new int[2]{0,4000}};
        List<Tuple<char,char,int,string>> workflow = workflows[workflowKey];
        for(int i = workflow.Count-1; i>=0; i--){
            //We go thorugh rule from last to first to always get the rules that will override the ones we saw before
            Tuple<char,char,int,string> rule = workflow[i];
            if(rule.Item4=="R"){
                if(rule.Item1!='0'){
                    if(rule.Item2=='>' && possibilities[categories[rule.Item1]][1]>rule.Item3){
                        possibilities[categories[rule.Item1]][1] = rule.Item3; //Max is reduced
                    }
                    else if(rule.Item2=='<' && possibilities[categories[rule.Item1]][0]<rule.Item3-1){
                        possibilities[categories[rule.Item1]][0] = rule.Item3-1; //Min is augmented
                    }
                }
                else{
                    //Last rule is rejected...
                    foreach(int[] pos in possibilities){
                        pos[0] = 4000;
                        pos[1] = 0;
                    }
                }
            }
            else if(rule.Item4=="A"){
                if(rule.Item1!='0'){
                    if(rule.Item2=='>'){
                        possibilities[categories[rule.Item1]][1] = 4000; //Everything above trigg is accepted
                        if(possibilities[categories[rule.Item1]][0]>rule.Item3){
                            possibilities[categories[rule.Item1]][0] = rule.Item3;
                        }
                    }
                    else if(rule.Item2=='<'){
                        possibilities[categories[rule.Item1]][0] = 0; //Everything under trigg is accepted
                        if(possibilities[categories[rule.Item1]][1]<rule.Item3){
                            possibilities[categories[rule.Item1]][1] = rule.Item3;
                        }
                    }
                }
                else{
                    //Last rule is accepted, since we started with full possibilities, nothing to do.
                }
            }
            else{
                if(rule.Item1!='0'){
                    List<int[]> otherPossibilities = RecursiveFindPossibilitiesInWorkflow(rule.Item4);

                }
                else{
                    possibilities = RecursiveFindPossibilitiesInWorkflow(rule.Item4);
                }
            }
        }

        possibleValueForWorkflows[workflowKey] = possibilities;
    }
    return possibleValueForWorkflows[workflowKey];
}

double CountPossibilities(List<int[]> input){
    double result = -1;
    foreach(var possibleValue in input){
        int nbPosForCatg = possibleValue[1]-possibleValue[0];
        if(nbPosForCatg >= 0){
            if(result==-1){
                //At least some possibilities
                result = 1;
            }
            result *= nbPosForCatg;
        }
    }
    return result==-1?0:result;
}

void ResultPossibilities(ref List<int[]> source, List<int[]> constraints){
    
}

double RecursiveCountPossibilitiesInWorkflow(string workflowKey){//,List<int[]> possibilities){
    double result = 0;
    if(!possibleCountForWorkflows.ContainsKey(workflowKey)){
        possibleIntervallsForWorkflows[workflowKey] = new List<List<int[]>>();
        List<int[]> possibilities = new List<int[]>{new int[2]{0,4000},new int[2]{0,4000},new int[2]{0,4000},new int[2]{0,4000}};
        List<Tuple<char,char,int,string>> workflow = workflows[workflowKey];
        for(int i = 0; i<workflow.Count-1; i++){
            //We go thorugh rules, calcul the possibilites for this rule and proceed with the rest of the list
            Tuple<char,char,int,string> rule = workflow[i];

            if(rule.Item4=="R"){
                if(rule.Item1!='0'){
                    if(rule.Item2=='>' && possibilities[categories[rule.Item1]][1]>rule.Item3){
                        possibilities[categories[rule.Item1]][1] = rule.Item3; //Max is reduced
                    }
                    else if(rule.Item2=='<' && possibilities[categories[rule.Item1]][0]<rule.Item3-1){
                        possibilities[categories[rule.Item1]][0] = rule.Item3-1; //Min is augmented
                    }
                    //Nothing to calculate, reject is only reducing possibilities
                }
                else{
                    //Last rule is rejected... no result added when here
                }
            }
            else if(rule.Item4=="A"){
                if(rule.Item1!='0'){
                    List<int[]> currentPoss = possibilities.ToList();
                    if(rule.Item2=='>'){
                        currentPoss[categories[rule.Item1]][1] = 4000; //Everything above trigg is accepted
                        if(currentPoss[categories[rule.Item1]][0]>rule.Item3){
                            currentPoss[categories[rule.Item1]][0] = rule.Item3;
                        }
                    }
                    else if(rule.Item2=='<'){
                        currentPoss[categories[rule.Item1]][0] = 0; //Everything under trigg is accepted
                        if(currentPoss[categories[rule.Item1]][1]<rule.Item3){
                            currentPoss[categories[rule.Item1]][1] = rule.Item3;
                        }
                    }
                    possibleIntervallsForWorkflows[workflowKey].Add(currentPoss);
                    result += CountPossibilities(currentPoss);

                    //Calcul to proceed for next loop
                    if(rule.Item2=='>' && possibilities[categories[rule.Item1]][1]>rule.Item3){
                        possibilities[categories[rule.Item1]][1] = rule.Item3; //Max is reduced
                    }
                    else if(rule.Item2=='<' && possibilities[categories[rule.Item1]][0]<rule.Item3-1){
                        possibilities[categories[rule.Item1]][0] = rule.Item3-1; //Min is augmented
                    }

                }
                else{
                    possibleIntervallsForWorkflows[workflowKey].Add(possibilities);
                    result += CountPossibilities(possibilities);
                }
            }
            else{
                if(!possibleIntervallsForWorkflows.ContainsKey(rule.Item4)){
                    RecursiveCountPossibilitiesInWorkflow(rule.Item4);
                }
                List<List<int[]>> otherPossibilities = possibleIntervallsForWorkflows[rule.Item4];

                if(rule.Item1!='0'){

                }
                else{

                    result += RecursiveCountPossibilitiesInWorkflow(rule.Item4);
                }
            }
        }

        possibleCountForWorkflows[workflowKey] = result;
    }

    return  possibleCountForWorkflows[workflowKey];
    //return result;
}

try
{
    //To get the time of code execution!
    var watch = System.Diagnostics.Stopwatch.StartNew();

    //Pass the file path and file name to the StreamReader constructor
    StreamReader sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\TestInput.txt");

    //Read the first line of text
    line = sr.ReadLine();

    bool isAworkflow = true;

    //Continue to read until you reach end of file
    while (line != null)
    {
        //write the line to console window
        Console.WriteLine(line);

        if(line.Length==0){
            isAworkflow = false;
        }
        else{
            if(isAworkflow){
                string key = line.Split('{')[0];
                List<Tuple<char,char,int,string>> rules = new List<Tuple<char,char,int,string>>();
                string[] rulesString = line.Split('{')[1].Replace("}","").Split(',');
                foreach(string rule in rulesString){
                    Tuple<char,char,int,string> desc;
                    if(rule.Contains(":")){
                        string cond = rule.Split(':')[0];
                        int triggVal;
                        char cat = cond[0];
                        char trigg;
                        if(cond.Contains("<")){
                            triggVal = int.Parse(cond.Split('<')[1]);
                            trigg = '<';
                        }
                        else if(cond.Contains(">")){
                            triggVal = int.Parse(cond.Split('>')[1]);
                            trigg = '>';
                        }
                        else{
                            Console.WriteLine("Unknown tigger rule : {0}", cond);
                            triggVal = 0;
                            cat = '0';
                            trigg = '0';
                        }
                        desc=Tuple.Create(cat,trigg,triggVal,rule.Split(':')[1]);
                    }
                    else{
                        desc=Tuple.Create('0','0',0,rule);
                    }
                    rules.Add(desc);
                }
                workflows[key] = rules;
            }
            else{
                string[] catValues = line.Substring(1,line.Length-2).Split(',');
                List<int> values = new List<int>();
                foreach(string catVal in catValues){
                    values.Add(int.Parse(catVal.Split('=')[1]));
                }
                parts.Add(values);
            }
        }

        //Read the next line
        line = sr.ReadLine();
    }

    //close the file
    sr.Close();

    /*
    //DEBUG LOG
    */
    
    Console.WriteLine("");

    //PART 1
    foreach(var part in parts){
        result += ResultFromWorkflows(part,"in");
    }
    
    Console.WriteLine();
    Console.WriteLine("End of input. Result game 1 found: {0}",result);
    Console.WriteLine();

    //PART 2
    List<int[]> possibleValues = RecursiveFindPossibilitiesInWorkflow("in");
    result = -1;
    foreach(var possibleValue in possibleValues){
        int nbPosForCatg = possibleValue[1]-possibleValue[0];
        if(nbPosForCatg >= 0){
            if(result==-1){
                //At least some possibilities
                result = 1;
            }
            result *= nbPosForCatg;
        }
    }
    result = result==-1?0:result; //If result equals -1 here, that means we never got a categories with possible values, so 0 pieces are accepted
    Console.WriteLine();
    Console.WriteLine("End of input. Result game 2 found: {0}",result);
    Console.WriteLine();

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

void LogInOutput(string outLine){
    using (StreamWriter outputFile = new StreamWriter(Directory.GetCurrentDirectory()+"\\..\\..\\..\\DebugLogs.txt", true))
    {
        outputFile.WriteLine(outLine);
    }
}