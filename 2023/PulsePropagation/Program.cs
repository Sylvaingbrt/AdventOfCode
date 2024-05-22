using System.Diagnostics;

string line;
double lowPulseSent = 0;
double highPulseSent = 0;
double result = 0;
Dictionary<string,bool> flipFlops = new Dictionary<string,bool>(); //Switch state when receiving a LOW PULSE, send back a HIGH PULSE when ON, LOW PULSE when OFF after change. OFF by default.
Dictionary<string,Dictionary<string,bool>> conjunctions = new Dictionary<string,Dictionary<string,bool>>(); //Save last state received for each input, send back a LOW PULSE when all saved state are HIGH, a HIGH PULSE otherwise. (all low at first)
Dictionary<string,List<string>> targets = new Dictionary<string,List<string>>(); //Send to list the pulse. 
Queue<Tuple<string,bool,string>> pulses = new Queue<Tuple<string,bool,string>>();

//Broadcaster send back to its targets the input it receives.
//Start by sending a LOW PULSE on broadcaster.
//Pulse are treated in ORDER, treat the pulse, put more pulse if any to queue, and then proceed with next.

//We will use the rule: LOW = FALSE | HIGH = true for the following, to switch between states and pulses.

bool CheckIfInitialState()
{
    foreach(var flipFlop in flipFlops){
        if(flipFlop.Value){
            return false;
        }
    }
    foreach(var conjunction in conjunctions){
        foreach(var memoryValue in conjunction.Value){
            if(memoryValue.Value){
                return false;
            }
        }
    }
    return true;
}

void Sendbroadcast()
{
    //Initialize broadcast
    lowPulseSent++; //First low pulse sent to broadcaster
    foreach(string target in targets["broadcaster"]){
        pulses.Enqueue(new Tuple<string,bool,string>(target,false,"broadcaster"));
    }

    //Solve pulses course
    bool needNewLine = false;
    while(pulses.Count > 0){
        Tuple<string,bool,string> pulse = pulses.Dequeue();
        if(pulse.Item2){
            highPulseSent++;
        }
        else{
            lowPulseSent++;
        }
        if(flipFlops.ContainsKey(pulse.Item1)){
            if(!pulse.Item2){
                flipFlops[pulse.Item1] = !flipFlops[pulse.Item1];
                foreach(string target in targets[pulse.Item1]){
                    pulses.Enqueue(new Tuple<string,bool,string>(target,flipFlops[pulse.Item1],pulse.Item1));
                }
            }
        }
        else if(conjunctions.ContainsKey(pulse.Item1)){
            conjunctions[pulse.Item1][pulse.Item3] = pulse.Item2;
            bool newPulse = !pulse.Item2;
            if(!newPulse){
                foreach(var memoryValue in conjunctions[pulse.Item1]){
                    if(!memoryValue.Value){
                        newPulse = true;
                        break;
                    }
                }
            }
            foreach(string target in targets[pulse.Item1]){
                pulses.Enqueue(new Tuple<string,bool,string>(target,newPulse,pulse.Item1));
            }
        }
        else{
            //A module that has no targets, can be seen as output? (cf. "more interesting example")
            //Console.WriteLine("{0} received pulse {1}",pulse.Item1,pulse.Item2);
            //needNewLine = true;
        }
    }
    if(needNewLine){
        Console.WriteLine();
    }
}


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

        string moduleName = line.Split("->")[0].Trim();
        List<string> targetsName = line.Split("->")[1].Replace(" ","").Split(",").ToList();

        if(moduleName.Contains("%")){
            //Flip-Flop
            moduleName = moduleName.Replace("%","");
            flipFlops[moduleName] = false;
        }
        else if(moduleName.Contains("&")){
            moduleName = moduleName.Replace("&","");
            conjunctions[moduleName] = new Dictionary<string, bool>();
        }
        else{
            //Broadcaster, nothing to do
        }

        targets[moduleName] = targetsName;

        //Read the next line
        line = sr.ReadLine();
    }

    //close the file
    sr.Close();

    //Finish conjuctions links
    foreach(var conjunction in conjunctions){
        foreach(var target in targets){
            if(target.Value.Contains(conjunction.Key)){
                //This module (target.Key) will send something to this conjunction module (conjunction.Key) 
                conjunction.Value[target.Key] = false; 
            }
        }
    }

    /*
    //DEBUG LOG
    Console.WriteLine();
    Console.WriteLine("Flip-Flops:");
    foreach(var flipFlop in flipFlops){
        string targetsNames = "";
        for(int i = 0; i<targets[flipFlop.Key].Count; i++){
            targetsNames += targets[flipFlop.Key][i];
            if(i!=targets[flipFlop.Key].Count-1){
                targetsNames += ", ";
            }
        }
        Console.WriteLine("flipFlop {0} will send pulse to: {1}",flipFlop.Key,targetsNames);
    }
    Console.WriteLine();
    Console.WriteLine("Conjunctions:");
    foreach(var conjunction in conjunctions){
        string targetsNames = "";
        for(int i = 0; i<targets[conjunction.Key].Count; i++){
            targetsNames += targets[conjunction.Key][i];
            if(i!=targets[conjunction.Key].Count-1){
                targetsNames += ", ";
            }
        }
        string connectedInputsNames = "";
        List<string> keyList = new List<string>(conjunction.Value.Keys);
        for(int i = 0; i<keyList.Count; i++){
            connectedInputsNames += keyList[i];
            if(i!=keyList.Count-1){
                connectedInputsNames += ", ";
            }
        }
        Console.WriteLine("Conjunction {0} will send pulse to: {1}. And receive pulse from: {2}",conjunction.Key,targetsNames,connectedInputsNames);
    }
    if(targets.ContainsKey("broadcaster")){
        Console.WriteLine();
        Console.WriteLine("Broadcaster:");
        string targetsNames = "";
        for(int i = 0; i<targets["broadcaster"].Count; i++){
            targetsNames += targets["broadcaster"][i];
            if(i!=targets["broadcaster"].Count-1){
                targetsNames += ", ";
            }
        }
        Console.WriteLine("Broadcaster will send pulse to: {0}",targetsNames);
    }
    */
    
    
    Console.WriteLine("");

    //PART 1
    int nbLoop = 1000;
    bool backToInitialState = false;
    while(nbLoop>0 && !backToInitialState){
        //Do the pulse
        Sendbroadcast();
        backToInitialState = CheckIfInitialState(); //If we are back to initial state, we can optimize
        nbLoop--;
    }
    if(backToInitialState && nbLoop > 0){
        int fullLoops = nbLoop / (1000-nbLoop);
        nbLoop = nbLoop % (1000-nbLoop);
        lowPulseSent*= fullLoops+1;
        highPulseSent*= fullLoops+1;
        while(nbLoop>0){
            //Do the pulse
            Sendbroadcast();
            backToInitialState = CheckIfInitialState();
            nbLoop--;
        }
    }
    result = lowPulseSent * highPulseSent;
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