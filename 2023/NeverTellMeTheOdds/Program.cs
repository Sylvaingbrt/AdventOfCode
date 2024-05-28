using System.Windows.Markup;

string line;
double result = 0;
List<Tuple<double,double,double>> positions = new List<Tuple<double,double,double>>();
List<Tuple<double,double,double>> speeds = new List<Tuple<double,double,double>>();

/*
bool IntersectInBound(Tuple<double,double,double> pA, Tuple<double,double,double> vA, Tuple<double,double,double> pB, Tuple<double,double,double> vB, double minP, double maxP, bool useZ){
    double zPA = pA.Item3;
    double zVA = vA.Item3;
    double zPB = pB.Item3;
    double zVB = vB.Item3;
    if(!useZ){
        zPA = 0;
        zVA = 0;
        zPB = 0;
        zVB = 0;
    }


}
*/

try
{
    //To get the time of code execution!
    var watch = System.Diagnostics.Stopwatch.StartNew();

    //Pass the file path and file name to the StreamReader constructor
    StreamReader sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\TestInput.txt");

    //Read the first line of text
    line = sr.ReadLine();

    //Continue to read until you reach end of file
    while (line != null)
    {
        //write the line to console window
        Console.WriteLine(line);
        string[] posValues = line.Split("@")[0].Trim().Split(", ");
        string[] velValues = line.Split("@")[1].Trim().Split(", ");
        positions.Add(Tuple.Create(double.Parse(posValues[0]), double.Parse(posValues[1]), double.Parse(posValues[2])));
        speeds.Add(Tuple.Create(double.Parse(velValues[0]), double.Parse(velValues[1]), double.Parse(velValues[2])));

        //Read the next line
        line = sr.ReadLine();
    }

    //close the file
    sr.Close();

    //DEBUG LOGS
    Console.WriteLine();
    Console.WriteLine("We have {0} positions and {1} speeds",positions.Count,speeds.Count);
    for(int i = 0; i < positions.Count; i++){
        Console.WriteLine("Point at ({0},{1},{2}) is goind at speed ({3},{4},{5})",positions[i].Item1,positions[i].Item2,positions[i].Item3,speeds[i].Item1,speeds[i].Item2,speeds[i].Item3);
    }

    //PART 1
    Console.WriteLine();
    Console.WriteLine("End of input. Result game 1 found: {0}",result);

    
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

void LogInOutput(string outLine,string fileName){
    using (StreamWriter outputFile = new StreamWriter(Directory.GetCurrentDirectory()+"\\..\\..\\..\\"+fileName+".txt", true))
    {
        outputFile.WriteLine(outLine);
    }
}