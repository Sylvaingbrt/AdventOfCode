string line;
double result1 = 0;
int goodNum;
string cardValues;
string winningNums;
List<string> wNbList = new List<string>();


//Varaibles for part 2
double result2 = 0;
int[] copies = new int[202];
Array.Fill(copies, 0);
int cardIdx=0;


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

        //Reset Variables
        goodNum = 0;
        wNbList.Clear();
        winningNums="";

        //Parse
        cardValues = line.Split(':')[1];

        //Keep winning numbers on a list, while remove space artefacts
        foreach(string winingNum in cardValues.Split('|')[0].Split(" ")){
            if(winingNum.Trim().Length > 0){
                wNbList.Add(winingNum.Trim());
            }
        }

        //Check for each number if inside winning numbers list, and keep track of how many good numbers we have
        foreach(string num in cardValues.Split('|')[1].Split(" ")){
            if(wNbList.Contains(num.Trim())){
                goodNum++;
                winningNums += num.Trim() + ",";
            }
        }

        Console.WriteLine("Winning nums: {0} WHICH means {1} winning numbers. {2} points. Also we have {3} instance of this card.",winningNums,goodNum,Math.Floor(Math.Pow(2,goodNum-1)),1+copies[cardIdx]);

        //Get the result for this card
        if(goodNum > 0){
            result1+=Math.Pow(2,goodNum-1);
            for(int i=1;i<=goodNum;i++){
                copies[cardIdx+i] += 1+copies[cardIdx];
            }
        }
        result2+=1+copies[cardIdx];
        Console.WriteLine("");

        //Read the next line
        line = sr.ReadLine();
        cardIdx++;
    }

    Console.WriteLine("End of input. Result game 1 found: {0}",result1);
    Console.WriteLine("End of input. Result game 1 found: {0}",result2);

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