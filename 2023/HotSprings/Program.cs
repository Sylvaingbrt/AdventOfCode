//I looked for hints for the first part. The recursive way seemed too heavy and I wanted to search for rules and constraints that would allow me to find the answer an other way. But knowing others had choose a recursive method made me decide to try the same.
//Part 2 had me too I confess. Memoization is a great idea, and it seems I don't use it often enough to be able to think about quickly haha.

//To use a dictionary with a complex type like a List, we need to make a custom IEqualityComparer, and that makes us use class:
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
                Console.WriteLine("{0} exist!!",part2);
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

                    double nbForLine = GetPossibilityForLine(line, 0, groups);

                    Console.WriteLine("Possibilities for this line: {0}",nbForLine);

                    result1 += nbForLine;

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
                Console.WriteLine("End of input. Result game 2 found: {0}",result2);
                Console.WriteLine("Memoization size: {0}",possibilitiesMemoization.Count);
                
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("END");
            }
    }
}