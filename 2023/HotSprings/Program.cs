using System.Collections.Immutable;

string line;
double result1 = 0;
List<int> groups = new List<int>();

List<int> GetGroups(string input){
    List<int> result = new List<int>();
    foreach(string s in input.Split(",")){
        if(s.Trim().Length > 0){
            result.Add(Int32.Parse(s.Trim()));
        }
    }
    return result;
}

bool ValidString(string input, int maxSize, List<int> listOfSize){
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

    if(input.Length==maxSize){
        //full length, need to match perfectly the list
        if(listOfSize.Count-1!=indexList || sizeInList>0){
            return false;
        }
    }
    else{
        //Check if possible for current input and if possible for reste of size
        int rest = sizeInList-1;

        for(int i=indexList+1;i<listOfSize.Count;i++){
            rest+=listOfSize[i] + 1;
        }
        
        if(maxSize-input.Length<rest){
            return false;
        }
    }
    return true;
}

double GetPossibilityForLine(string fullLine, string part, List<int> listOfSize){
    if(ValidString(part, fullLine.Length, listOfSize)){
        
        if(part.Length==fullLine.Length){
            //Console.WriteLine("{0} is valid and complete",part);
            return 1;
        }
        //Console.WriteLine("{0} is valid",part);
        char c = fullLine[part.Length];
        if(c=='?'){
            return GetPossibilityForLine(fullLine, part+".", listOfSize) + GetPossibilityForLine(fullLine, part+"#", listOfSize);
        }
        else{
            return GetPossibilityForLine(fullLine, part+c, listOfSize);
        }
    }
    //Console.WriteLine("{0} is not valid",part);
    return 0;
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
        
        groups = GetGroups(line.Split(" ")[1]);
        line = line.Split(" ")[0];

        //Console.WriteLine(line);
        string group = "";
        foreach(int i in groups){
            group += i.ToString() + ",";
        }
        //Console.WriteLine(group);

        double nbForLine = GetPossibilityForLine(line, "", groups);

        Console.WriteLine("Possibilities for this line: {0}",nbForLine);

        result1 += nbForLine;

        
        Console.WriteLine("");

        //Read the next line
        line = sr.ReadLine();
    }
    

    //close the file
    sr.Close();

    Console.WriteLine("");

    Console.WriteLine("End of input. Result game 1 found: {0}",result1);
}
catch(Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}
finally
{
    Console.WriteLine("END");
}