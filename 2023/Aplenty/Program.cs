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

try
{
    //To get the time of code execution!
    var watch = System.Diagnostics.Stopwatch.StartNew();

    //Pass the file path and file name to the StreamReader constructor
    StreamReader sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\Input.txt");

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