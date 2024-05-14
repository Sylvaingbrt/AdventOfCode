
string line;
double result1 = 0;
List<string> mapString = new List<string>();

//For part 2
double result2 = 0;
List<string> mapString2 = new List<string>();
List<List<string>> seenConfigurations = new List<List<string>>();
double numberOfCycles = 1000000000;

List<string> Transpose(List<string> input){
    List<string> result = new List<string>();
    for(int i=0;i<input[0].Length; i++){
        string column = "";
        for(int j=0;j<input.Count; j++){
            column+=input[j][i];
        }
        result.Add(column);
    }
    return result;
}

List<string> RollRight(List<string> input){
    List<string> result = new List<string>();
    foreach(string inputLine in input){
        int newIndex = 0;
        string resultLine = "";
        for(int i=0; i<inputLine.Length; i++){
            resultLine+=inputLine[i];
            if(inputLine[i]=='#'){
                newIndex = i+1;
            }
            else if(inputLine[i]=='O'){
                if(newIndex!=i){
                    //Console.WriteLine("{0}, with new index {1}. So length second part of string is {2}.",resultLine,newIndex,resultLine.Length-newIndex-2);
                    resultLine = resultLine.Substring(0,newIndex) + 'O' + resultLine.Substring(newIndex+1,resultLine.Length-newIndex-2) + '.';
                }
                newIndex++;
            }
        }
        result.Add(resultLine);
    }
    return result;
}

List<string> RollLeft(List<string> input){
    List<string> result = new List<string>();
    foreach(string inputLine in input){
        int newIndex = inputLine.Length-1;
        string resultLine = "";
        for(int i=inputLine.Length-1; i>=0; i--){
            resultLine=inputLine[i]+resultLine;
            if(inputLine[i]=='#'){
                newIndex = i-1;
            }
            else if(inputLine[i]=='O'){
                if(newIndex!=i){
                    //Console.WriteLine("{0}, with new index {1}. So length first part of string is {2}.",resultLine,newIndex,newIndex-i-1);
                    resultLine = '.' + resultLine.Substring(1,newIndex-i-1) + 'O' + resultLine.Substring(newIndex-i+1);
                    //Console.WriteLine("And it gives: {0}",resultLine);
                }
                newIndex--;
            }
        }
        result.Add(resultLine);
    }
    return result;
}

double CountLoad(List<string> input)
{
    double result = 0;
    int n = input.Count;
    for(int i=0; i<n; i++){
        foreach(char c in input[i]){
            if(c=='O'){
                result += n-i;
            }
        }
    }
    return result;
}

List<string> CycleRoll(List<string> input)
{
    //RollRight north, then west, then south, then east.
    List<string> result = RollRight(Transpose(input)); //North
    result = RollRight(Transpose(result)); //West
    result = RollLeft(Transpose(result)); //South
    result = RollLeft(Transpose(result)); //East
    return result;
}

int KnownCycleIndex(List<List<string>> knownCycles, List<string> currentCylce){
    int result = -1;
    bool found = false;
    foreach(List<string> cycle in knownCycles){
        found = true;
        result++;
        for(int i=0; i<cycle.Count; i++){
            if(!cycle[i].Equals(currentCylce[i])){
                found = false;
                break;
            }
        }

        if(found){
            break;
        }
    }
    if(!found){
        return -1;
    }
    return result;
}   

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

        mapString.Add(line);
        mapString2.Add(line);

        //Read the next line
        line = sr.ReadLine();
    }

    //close the file
    sr.Close();

    mapString = Transpose(RollRight(Transpose(mapString)));

    result1 = CountLoad(mapString);

    Console.WriteLine("");
    Console.WriteLine("End of input. Result game 1 found: {0}",result1);
    Console.WriteLine("");

    /*
    //Way to long due to useless heavy work as manipulating string all around. And doing the same work again and again because of the repeating pattern.
    for(double i = 0; i < numberOfCycles; i++){
        mapString2 = CycleRoll(mapString2);
    }
    */

    //We search for the moment we get back to an already seen configuration, then we just make the left cycles to have the configuration we would have by doing the 1000000000 cycles.
    int nbCyclesBeforeLoop = 0;
    int knownIndex = KnownCycleIndex(seenConfigurations,mapString2);
    while(knownIndex<0){
        seenConfigurations.Add(mapString2);
        mapString2 = CycleRoll(mapString2);
        nbCyclesBeforeLoop++;
        knownIndex = KnownCycleIndex(seenConfigurations,mapString2);
    }
    /*
    //DEBUG LOG
    foreach(string mapLine in mapString2){
        Console.WriteLine(mapLine);
    }
    */
    Console.WriteLine("We made {0} cycles, previous known cycle at {1}",nbCyclesBeforeLoop,knownIndex);
    double cyclesLeftToDo = (numberOfCycles - knownIndex) % (nbCyclesBeforeLoop-knownIndex);
    Console.WriteLine("Cycles to do before finding answer: {0} cycles",cyclesLeftToDo);
    for(double i = 0; i < cyclesLeftToDo; i++){
        mapString2 = CycleRoll(mapString2);
    }
    result2 = CountLoad(mapString2);
    Console.WriteLine("End of input. Result game 2 found: {0}",result2);

}
catch(Exception e)
{
    Console.WriteLine("Exception: " + e.ToString());
}
finally
{
    Console.WriteLine("END");
}