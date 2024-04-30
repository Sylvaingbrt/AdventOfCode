using System.Text.RegularExpressions;
using System.Drawing;

string currLine;
string prevLine;
string nextLine;
double result = 0;

string subString;
string numString;
int nbEndIndex;
int nbLength;
int nbValue;
bool enginePart;

string debugString;

//For part 2
Dictionary<Point, List<int>> gears = new Dictionary<Point, List<int>>();
int lineIndex = 0;
double result2 = 0;

bool checkChar(char ch){
    return (ch!='.') && ((ch-48<0) || (ch-48>9)); //actually we need to only check if it's a dot or not, numbers do not touch themselves
}

//Used for part 2
void AddToDictionnary(Point key,int value){
    if(!gears.ContainsKey(key)){
        gears[key] = new List<int>();
    }
    gears[key].Add(value);
}


try
{
    //Pass the file path and file name to the StreamReader constructor
    StreamReader sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\Input.txt");

    prevLine = "";
    nextLine = "";

    //Read the first line of text
    currLine = sr.ReadLine();

    //Continue to read until you reach end of file
    while (currLine != null)
    {
        //Read the next line
        nextLine = sr.ReadLine();


        //write the lines to console window
        Console.WriteLine("Current line : {0}",currLine);

        //Prepare Loop variables
        nbEndIndex = 0;
        nbLength = 0;
        nbValue = 0;
        debugString="Add: ";

        foreach (Match m in Regex.Matches(currLine, @"(\d\D|\d$)")) {

            //Cut the string at each end of number, remove all other characters except digits: this will give the index and size of next number.
            //From here you can look all around to check if there is a special character that would indicate to take into account this number.
            
            subString = currLine.Substring(nbEndIndex, m.Index - nbEndIndex+1);
            numString = Regex.Replace(subString,@"\D","");

            nbEndIndex += subString.Length+1;
            nbLength = numString.Length;
            enginePart = false;

            if(nbLength!=0){
                nbValue = Int32.Parse(numString);

                //search for special character around this value
                if((nbEndIndex-nbLength-1>0 && currLine[nbEndIndex-nbLength-2]!='.') || (nbEndIndex<currLine.Length && currLine[nbEndIndex-1]!='.')){
                    if(nbEndIndex-nbLength-1>0 && currLine[nbEndIndex-nbLength-2]=='*'){
                        AddToDictionnary(new Point(lineIndex,nbEndIndex-nbLength-2),nbValue);
                    }
                    if(nbEndIndex<currLine.Length && currLine[nbEndIndex-1]=='*'){
                        AddToDictionnary(new Point(lineIndex,nbEndIndex-1),nbValue);
                    }
                    enginePart = true;
                }
                
                for(int i=Math.Max(nbEndIndex-nbLength-2,0); i<Math.Min(nbEndIndex,currLine.Length);i++){
                    if((prevLine.Length>0 && checkChar(prevLine[i])) || (nextLine!=null && nextLine.Length>0 && checkChar(nextLine[i]))){
                        if(prevLine.Length>0 && prevLine[i]=='*'){
                            AddToDictionnary(new Point(lineIndex-1,i),nbValue);
                        }
                        if(nextLine!=null && nextLine.Length>0 && nextLine[i]=='*'){
                            AddToDictionnary(new Point(lineIndex+1,i),nbValue);
                        }
                        enginePart = true;
                    }
                }
                if(enginePart){
                    debugString+=numString +", ";
                    result += nbValue;
                }
            }
        }
        Console.WriteLine(debugString);

        prevLine = currLine;
        currLine = nextLine;
        lineIndex++;
    }

    Console.WriteLine("End of input. Result game 1 found: {0}",result);

    foreach(var gear in gears){
        /*
        //DEBUG LOGS
        Console.WriteLine("Gear found for pos: {0},{1}",gear.Key.X,gear.Key.Y);
        string values = "";
        foreach(int val in gear.Value){
            values += val.ToString()+",";
        }
        Console.WriteLine("Gear values: " + values);
        */
        if(gear.Value.Count==2){
            result2 += gear.Value[0]*gear.Value[1];
        }
    }

    Console.WriteLine("End of input. Result game 2 found: {0}",result2);

    //close the file
    sr.Close();
}
catch(Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}
finally
{
    Console.WriteLine("END");
}