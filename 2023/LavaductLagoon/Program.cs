using System.Drawing;

string line;
double result = 0;
Dictionary<Point,char> map = new Dictionary<Point,char>();
List<Tuple<char,int,Color>> instructions = new List<Tuple<char,int,Color>>();
char diggedChar = '#';
char emptyChar = '.';


Tuple<char, int, Color> ConvertInstruction(string input)
{
    char dir = input.Split(" ")[0][0];
    int length = int.Parse(input.Split(" ")[1]);
    Color color = ColorTranslator.FromHtml(input.Split(" ")[2].Substring(1,7));
    Tuple<char,int,Color> result = Tuple.Create(dir,length,color);
    return result;
}

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

        instructions.Add(ConvertInstruction(line));

        //Read the next line
        line = sr.ReadLine();
    }

    //close the file
    sr.Close();
    
    Console.WriteLine("");

    //Draw map:
    int mapX = -1;
    int mapY = -1;
    Point currentPoint = new Point(0,0);
    map[currentPoint] = diggedChar;
    Point direction = new Point(0,0);
    foreach(var instruction in instructions){
        switch(instruction.Item1){
            case 'L':
                direction.X = -1;
                direction.Y = 0;
                break;
            case 'R':
                direction.X = 1;
                direction.Y = 0;
                break;
            case 'U':
                direction.X = 0;
                direction.Y = -1;
                break;
            case 'D':
                direction.X = 0;
                direction.Y = 1;
                break;
            default:
                Console.WriteLine("Unknown instruction dir: {0}", instruction.Item1);
                break;
        }

        for(int i = 0; i < instruction.Item2; i++){
            currentPoint.X+=direction.X;
            currentPoint.Y+=direction.Y;

            if(mapX<currentPoint.X){
                mapX = currentPoint.X;
            }
            if(mapY<currentPoint.Y){
                mapY = currentPoint.Y;
            }

            if(map.ContainsKey(currentPoint)){
                Console.WriteLine("Point aleady digged at ({0},{1})", currentPoint.X,currentPoint.Y);
            }
            else{
                map[currentPoint] = diggedChar;
            }
        }
    }

    //DEBUG LOG
    for(int i = 0; i <= mapY; i++){
        for(int j = 0; j <= mapX; j++){
            Point point = new Point(j, i);
            if(map.ContainsKey(point)){
                Console.Write('#');
            }
            else{
                Console.Write('.');
            }
        }
        Console.WriteLine();
    }

    
    //PART 1
    Console.WriteLine("");
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

void LogInOutput(string outLine){
    using (StreamWriter outputFile = new StreamWriter(Directory.GetCurrentDirectory()+"\\..\\..\\..\\DebugLogs.txt", true))
    {
        outputFile.WriteLine(outLine);
    }
}