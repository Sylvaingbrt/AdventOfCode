using System.Text.RegularExpressions;

string currLine;
string prevLine;
string nextLine;
double result = 0;

string subString;
string numString;
int nbEndIndex;
int nbLength;
int nbValue;

string debugString;

bool checkChar(char ch){
    return (ch!='.') && ((ch-48<0) || (ch-48>9)); //actually we need to only check if it's a dot or not, numbers do not touch themselves
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

            if(nbLength!=0){
                nbValue = Int32.Parse(numString);

                //search for special character around this value
                if((nbEndIndex-nbLength-1>0 && currLine[nbEndIndex-nbLength-2]!='.') || (nbEndIndex<currLine.Length && currLine[nbEndIndex-1]!='.')){
                    debugString+=numString +", ";
                    result += nbValue;
                }
                else{
                    for(int i=Math.Max(nbEndIndex-nbLength-2,0); i<Math.Min(nbEndIndex,currLine.Length);i++){
                        if((prevLine.Length>0 && checkChar(prevLine[i])) || (nextLine!=null && nextLine.Length>0 && checkChar(nextLine[i]))){
                            debugString+=numString +", ";
                            result += nbValue;
                            break;
                        }
                    }
                }
            }
        }
        Console.WriteLine(debugString);

        prevLine = currLine;
        currLine = nextLine;
    }

    Console.WriteLine("End of input. Result game found: {0}",result);

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