string line;
double result1 = 0;
List<List<double>> suites = new List<List<double>>();
bool sameNum;
int index;
double diff;

//For part 2
double result2 = 0;

List<double> GetNumbersFromString(string nbString){
    List<double> result = new List<double>();
    string[] toConvert = nbString.Split(" ");
    foreach(string s in toConvert){
        if(s.Trim().Length > 0){
            result.Add(double.Parse(s.Trim()));
        }
        
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

        //Reset variables
        index = 0; 
        suites.Clear();
        suites.Add(GetNumbersFromString(line));

        //Create the table of "derivatives" until we find the const.
        do
        {
            //Console.WriteLine("DO! Size suites: {0}",suites.Count);
            sameNum = true;
            List<double> suite = new List<double>();
            diff = suites[index][1]-suites[index][0];
            for(int i = 0; i<suites[index].Count-1;i++){
                if(suites[index][i+1]-suites[index][i]!=diff){
                    sameNum=false;
                    diff=suites[index][i+1]-suites[index][i];
                }
                suite.Add(diff);
            }
            suites.Add(suite);
            index++;
        } while (!sameNum);

        for(int i = suites.Count-1; i>0;i--){
            suites[i-1].Add(suites[i-1][suites[i-1].Count-1]+suites[i][suites[i].Count-1]);
            
            //Part 2 
            suites[i-1].Insert(0,suites[i-1][0]-suites[i][0]);
        }

        result1+=suites[0][suites[0].Count-1];
        result2+=suites[0][0];

        //Read the next line
        line = sr.ReadLine();
    }

    //close the file
    sr.Close();

    Console.WriteLine("End of input. Result game 1 found: {0}",result1);
    Console.WriteLine("End of input. Result game 2 found: {0}",result2);

}
catch(Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}
finally
{
    Console.WriteLine("END");
}