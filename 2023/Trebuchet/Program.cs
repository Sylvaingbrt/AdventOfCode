using System.IO;
using System.Xml.XPath;

bool checkForStrings = true; //Used to get result for 1st exercice if false, 2nd if true
String line;
int i;
int j;
int nInDigits1;
int nInDigits2;
int lineResult;
double result = 0;
List<String> digitsStrings =
[
    "one",
    "two",
    "three",
    "four",
    "five",
    "six",
    "seven",
    "eight",
    "nine"
];

bool CheckStringAtIndex(ref int index, ref int res, string line){
    res = -1;
    if(index+3<=line.Length && checkForStrings){
        int k=1;
        foreach(string num in digitsStrings){
            if(index+num.Length<=line.Length){
                string candidat = line.Substring(index,num.Length);
                if(candidat.Equals(num)){
                    res=k;
                    //index=index+num.Length-1;
                    return true;
                }
            }
            k++;
        }
    }
    return false;
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

        //reset variables
        i = 0;
        nInDigits1 = -1; 
        nInDigits2 = -1;

        while(i<line.Length && ((line[i]-48)<0 || (line[i]-48)>9) && !CheckStringAtIndex(ref i,ref nInDigits1,line)){
            i++;
        }
        if(i<line.Length){
            if(nInDigits1>0){
                Console.WriteLine("Found first digit: {0}, which begins at index: {1}",nInDigits1,i);
                lineResult = nInDigits1*10;
            }
            else{
                Console.WriteLine("Found first digit: {0}, at index: {1}",line[i],i);
                lineResult = (line[i]-48)*10;
            }
            
            for(j=line.Length-1;j>i-1;j--){
                if(((line[j]-48)>=0 && (line[j]-48)<=9) || CheckStringAtIndex(ref j,ref nInDigits2,line)){
                    if(nInDigits2>0){
                        lineResult += nInDigits2;
                        Console.WriteLine("Found last digit: {0}, which begins at index: {1}",nInDigits2,j);
                        break;
                    }
                    else{
                        lineResult += line[j]-48;
                        Console.WriteLine("Found last digit: {0}, at index: {1}",line[j],j);
                        break;
                    }
                }
            }
            result+=lineResult;
        }
        else{
            Console.WriteLine("-----------NO DIGIT HERE-----------");
        }

        //Read the next line
        line = sr.ReadLine();
    }

    Console.WriteLine("End of input. Result found: {0}",result);

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
