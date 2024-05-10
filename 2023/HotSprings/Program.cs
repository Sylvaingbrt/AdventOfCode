//I looked for hints for the first part. The recursive way seemed too heavy and I wanted to search for rules and constraints that would allow me to find the answer an other way. But knowing others had choose a recursive method made me decide to try the same.
//Part 2 had me too I confess. Memoization is a great idea, and it seems I don't use it often enough to be able to think about quickly haha.

//To use a dictionary with a complex type like a List, we need to make a custom IEqualityComparer, and that makes us use class:
using System.Diagnostics;

public class MyEqualityComparer : IEqualityComparer<Tuple<string,List<int>>>
{
    public bool Equals(Tuple<string,List<int>> x, Tuple<string,List<int>> y)
    {
        if (!x.Item1.Equals(y.Item1))
        {
            return false;
        }
        if(x.Item2.Count != y.Item2.Count){
            return false;
        }
        for (int i = 0; i < x.Item2.Count; i++)
        {
            if (x.Item2[i] != y.Item2[i])
            {
                return false;
            }
        }
        return true;
    }

    public int GetHashCode(Tuple<string,List<int>> obj)
    {
        int result = 17;
        unchecked
        {
            result = result * 23 + obj.Item1.GetHashCode();
        }
        for (int i = 0; i < obj.Item2.Count; i++)
        {
            unchecked
            {
                result = result * 23 + obj.Item2[i];
            }
        }
        return result;
    }
}

class Program
{
    static string line;
    static double result1 = 0;
    static double resultBis = 0;
    static List<int> groups = new List<int>();

    //For part 2
    static string extendedLine;
    static double result2 = 0;
    static List<int> extendedGroups = new List<int>();
    
    static Dictionary<Tuple<string,List<int>>,double> possibilitiesMemoization = new Dictionary<Tuple<string, List<int>>,double>(new MyEqualityComparer());

    static List<int> GetGroups(string input){
        List<int> result = new List<int>();
        foreach(string s in input.Split(",")){
            if(s.Trim().Length > 0){
                result.Add(Int32.Parse(s.Trim()));
            }
        }
        return result;
    }

    static bool ValidString(string input, string restString, List<int> listOfSize, ref List<int> restOfSize){
        bool needToGetNewSize = true;
        int indexList = -1;
        int sizeInList = 0;
        foreach(char c in input){
            if(c=='#'){
                if(needToGetNewSize){
                    needToGetNewSize = false;
                    indexList++;
                    if(indexList>=listOfSize.Count){
                        return false;
                    }
                    sizeInList = listOfSize[indexList];
                }
                sizeInList--;
                if(sizeInList < 0){
                    return false;
                }
            }
            else if(c=='.'){
                if(sizeInList > 0){
                    return false;
                }
                needToGetNewSize = true;
            }
        }

        //Console.WriteLine("Input:{0}, maxSize:{1}, listCount:{2}, index:{3}, leftSize:{4}",input.Length,maxSize,listOfSize.Count,indexList,sizeInList);

        if(restString.Length==0){
            //full length, need to match perfectly the list
            if(listOfSize.Count-1!=indexList || sizeInList>0){
                return false;
            }
        }
        else{
            //Check if possible for current input and if possible for reste of size
            int rest = sizeInList-1;
            int currentRest = 0;

            for(int i=indexList+1;i<listOfSize.Count;i++){
                rest+=listOfSize[i] + 1;
            }

            bool newGroup = false;
            foreach(char c in restString){
                if(c!='.'){
                    currentRest++;
                    if(newGroup){
                        currentRest++;
                    }
                }
                else{
                    newGroup = true;
                }
            }
            
            if(currentRest<rest){
                return false;
            }
        }

        if(sizeInList>0){
            restOfSize.Add(sizeInList);
        }
        for(int i=indexList+1;i<listOfSize.Count;i++){
            restOfSize.Add(listOfSize[i]);
        }

        return true;
    }

    static double GetPossibilityForLine(string fullLine, int partIdx, List<int> listOfSize){
        string part1 = fullLine.Substring(0,partIdx);
        string part2 = fullLine.Substring(partIdx,fullLine.Length-partIdx);
        List<int> restOfSize = new List<int>();
        bool isValid = ValidString(part1, part2, listOfSize, ref restOfSize);
        
        if(isValid){
            Tuple<string,List<int>> key = Tuple.Create(part2,restOfSize);
            if(!possibilitiesMemoization.ContainsKey(key)){

                if(partIdx==fullLine.Length-1){
                    //Console.WriteLine("{0} is valid and complete. (Part 2: {1})",part1,part2);
                    possibilitiesMemoization[key] = 1;
                }
                else{
                    //Console.WriteLine("{0} is valid. (Part 2: {1})",part1,part2);
                    char c = fullLine[partIdx];
                    if(c=='?'){
                        possibilitiesMemoization[key] = GetPossibilityForLine(part1 + "." + part2.Substring(1,part2.Length-1), partIdx+1, listOfSize) + GetPossibilityForLine(part1 + "#" + part2.Substring(1,part2.Length-1), partIdx+1, listOfSize);
                    }
                    else{
                        possibilitiesMemoization[key] =  GetPossibilityForLine(fullLine, partIdx+1, listOfSize);
                    }
                }

            }
            else{
                Console.WriteLine("{0} exist!! valeur: {1}",part2, possibilitiesMemoization[key]);
            }
            return possibilitiesMemoization[key];
        }
        else{
            Tuple<string,List<int>> key = Tuple.Create(part1+part2,listOfSize);
            //Console.WriteLine("{0} is not valid. (Part 2: {1})",part1,part2);
            possibilitiesMemoization[key] = 0;
            return possibilitiesMemoization[key];
        }
    }

    static int GetNbSpot(string input){
        int nbSpot = 0;
        bool group = false;
        bool newGroup = false;
        foreach(char c in input){
            if(c!='.'){
                group = true;
                nbSpot++;
                if(newGroup){
                    nbSpot++;
                    newGroup = false;
                }
            }
            else{
                if(group){
                    newGroup = true;
                }
            }
        }
        return nbSpot;
    }

    static int GetNbConstraints(List<int> input){
        int nbConstraints = 0;

        for(int i=0;i<input.Count;i++){
            nbConstraints+=input[i];
            if(i<input.Count-1){
                nbConstraints+=1;
            }
        }

        return nbConstraints;
    }

    static double GetNbPossibility(string input, List<int> constraints, int depth = 0){
        string debugConstraints = "";
        for(int i = 0; i < constraints.Count; i++){
            debugConstraints+=constraints[i].ToString();
            if(i<constraints.Count-1){
                debugConstraints+=", ";
            }
        }
        double result = 0;
        //Console.WriteLine("{1}. Get nb for : {0}",input,depth);
        if(input.Length==0){
            result += (constraints.Count==0) ? 1 : 0;
        }
        else{
            int nbSpots = GetNbSpot(input);
            int nbConstraints = GetNbConstraints(constraints);
            //Console.WriteLine("{2}. The input: {3} {4} has nbSpots : {0} | nbConstraints: {1}",nbSpots,nbConstraints,depth,input,debugConstraints);
            
            //if(nbSpots==nbConstraints){
            //    result = 1;
            //}
            //else{
                if(nbSpots<nbConstraints){
                    result = 0;
                }
                else{
                    if(input[0]=='.'){
                        result = GetNbPossibility(input.Substring(1,input.Length-1), constraints.ToList(),depth);
                    }
                    else if(input[0]=='#'){
                        if(nbConstraints==0){
                            result = 0;
                        }
                        else{
                            int offSet = constraints[0];
                            constraints.RemoveAt(0);
                            if(input.Length<=offSet+1){
                                result = (constraints.Count==0) ? 1 : 0;
                            }
                            else{
                                if(input[offSet]=='#'){
                                    result = 0;
                                }
                                else{
                                    result = GetNbPossibility(input.Substring(offSet+1,input.Length-(offSet+1)), constraints.ToList(),depth);
                                }
                            }
                        }
                        
                    }
                    else{
                        //Console.WriteLine("{1}. Maybe two possibilities for : {0}",input,depth);
                        double option1 = GetNbPossibility(input.Substring(1,input.Length-1), constraints.ToList(),depth);
                        //Console.WriteLine("{1}. Option 1 : {0}",option1,depth);
                        double option2 = 0;
                        if(nbConstraints!=0){
                            //Console.WriteLine("{1}. Constraint : {0}",constraints.Count,depth);
                            int offSet = constraints[0];
                            constraints.RemoveAt(0);
                            if((input.Length<=offSet+1) && (constraints.Count==0)){
                                option2 = 1;
                            }
                            else if(input.Length>1){
                                if(input[1]!='#'){
                                    option2 = GetNbPossibility(input.Substring(offSet+1,input.Length-(offSet+1)), constraints.ToList(),depth+1);
                                }
                            }
                        }
                        //Console.WriteLine("{1}. Option 1 : {0}",option1,depth);
                        //Console.WriteLine("{1}. Option 2 : {0}",option2,depth);
                        result = option1 + option2;
                    }
                }
            //}
            
        }

        
        //Console.WriteLine("{3}. The input: {0} {1} has a result of {2}",input,debugConstraints,result,depth);
        return result;
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
                    
                    groups = GetGroups(line.Split(" ")[1]);
                    line = line.Split(" ")[0];

                    /*
                    Console.WriteLine(line);
                    string group = "";
                    foreach(int i in groups){
                        group += i.ToString() + ",";
                    }
                    Console.WriteLine(group);
                    */

                    //double nbForLine = GetPossibilityForLine(line, 0, groups);
                    double nbForLineBis = GetNbPossibility(line, groups.ToList());
                    //Console.WriteLine("Possibilities for this line: {0}",nbForLine);
                    Console.WriteLine("Possibilities bis for this line: {0}",nbForLineBis);

                    //result1 += nbForLine;
                    resultBis += nbForLineBis;

                    /*
                    extendedLine =line;
                    extendedGroups.Clear();  
                    foreach(int j in groups){
                        extendedGroups.Add(j);
                    }

                    for(int i = 0; i < 4;i++){
                        extendedLine+="?"+line;
                        foreach(int j in groups){
                            extendedGroups.Add(j);
                        }
                    }
                    
                    Console.WriteLine(extendedLine);
                    string extendedGroup = "";
                    foreach(int i in extendedGroups){
                        extendedGroup += i.ToString() + ",";
                    }
                    Console.WriteLine(extendedGroup);
                    
                    nbForLine = GetPossibilityForLine(extendedLine, 0, extendedGroups);
                    Console.WriteLine("Extended possibilities for this line: {0}",nbForLine);
                    result2 += nbForLine;
                    */
                    
                    Console.WriteLine("");

                    //Read the next line
                    line = sr.ReadLine();
                }
                

                //close the file
                sr.Close();

                Console.WriteLine("");

                Console.WriteLine("End of input. Result game 1 found: {0}",result1);
                Console.WriteLine("End of input. Result bis game 1 found: {0}",resultBis);
                Console.WriteLine("End of input. Result game 2 found: {0}",result2);
                Console.WriteLine("Memoization size: {0}",possibilitiesMemoization.Count);
                
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: {0}",e.ToString());
            }
            finally
            {
                Console.WriteLine("END");
            }
    }
}