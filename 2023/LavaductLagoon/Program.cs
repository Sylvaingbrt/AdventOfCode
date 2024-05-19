using System.Drawing;

string line;
double result = 0;
Dictionary<Point,Color> map = new Dictionary<Point,Color>();
List<Tuple<char,int,Color>> instructions = new List<Tuple<char,int,Color>>();
char diggedChar = '#';
char emptyChar = '.';
char fillChar = 'F';
Color fillColor= Color.White;
bool lookLeft = false; //Same logic as for Pipe Maze (day 10) problem.
bool outside = false; //Same logic as for Pipe Maze (day 10) problem.
int mapSizeX = -1;
int mapSizeY = -1;
int minSizeX = 0;
int minSizeY = 0;
int chainSize = 0;


Tuple<char, int, Color> ConvertInstruction(string input)
{
    char dir = input.Split(" ")[0][0];
    int length = int.Parse(input.Split(" ")[1]);
    Color color = ColorTranslator.FromHtml(input.Split(" ")[2].Substring(1,7));
    Tuple<char,int,Color> result = Tuple.Create(dir,length,color);
    return result;
}

void LookAtTile(Point pos){
    if(!map.ContainsKey(pos)){
        map[pos] = fillColor;
        if(pos.X==minSizeX || pos.X==mapSizeX || pos.Y==minSizeY || pos.Y==mapSizeY){
            outside = true;
        }
        LookAtTile(new Point(Math.Max(pos.X-1,minSizeX),pos.Y));
        LookAtTile(new Point(Math.Min(pos.X+1,mapSizeX),pos.Y));
        LookAtTile(new Point(pos.X,Math.Max(pos.Y-1,minSizeY)));
        LookAtTile(new Point(pos.X,Math.Min(pos.Y+1,mapSizeY)));
    }
}

try
{
    //To get the time of code execution!
    var watch = System.Diagnostics.Stopwatch.StartNew();

    //Pass the file path and file name to the StreamReader constructor
    StreamReader sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\Input.txt");

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
    Point currentPoint = new Point(0,0);
    Color digColor = instructions[0].Item3;
    map[currentPoint] = digColor;
    Point direction = new Point(0,0);
    for(int i=0; i<instructions.Count; i++){
        var instruction = instructions[i];
        digColor =instruction.Item3;
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

        for(int j = 0; j < instruction.Item2; j++){
            currentPoint.X+=direction.X;
            currentPoint.Y+=direction.Y;
            
            //Console.WriteLine("Point at : ({0},{1})", currentPoint.X,currentPoint.Y);

            if(mapSizeX<currentPoint.X){
                mapSizeX = currentPoint.X;
                //Console.WriteLine("New max : {0}", mapSizeX);
            }
            if(mapSizeY<currentPoint.Y){
                mapSizeY = currentPoint.Y;
            }

            if(minSizeX>currentPoint.X){
                minSizeX = currentPoint.X;
            }
            if(minSizeY>currentPoint.Y){
                minSizeY = currentPoint.Y;
            }

            if(map.ContainsKey(currentPoint)){
                Console.WriteLine("Point aleady digged at ({0},{1})", currentPoint.X,currentPoint.Y);
            }
            else{
                map[currentPoint] = digColor;
            }
        }
    }

    chainSize = map.Count;

    //We go through all instructions again, looking at a side of our path, and fill empty spaces. It is the same logic as day 10 probelm.
    direction = new Point(0,0);
    Point lookoutDir = new Point(0,0);
    for(int i=0; i<instructions.Count; i++){
        var instruction = instructions[i];
        switch(instruction.Item1){
            case 'L':
                direction.X = -1;
                direction.Y = 0;
                lookoutDir.X = 0;
                lookoutDir.Y = lookLeft?1:-1;
                break;
            case 'R':
                direction.X = 1;
                direction.Y = 0;
                lookoutDir.X = 0;
                lookoutDir.Y = lookLeft?-1:1;
                break;
            case 'U':
                direction.X = 0;
                direction.Y = -1;
                lookoutDir.X = lookLeft?-1:1;
                lookoutDir.Y = 0;
                break;
            case 'D':
                direction.X = 0;
                direction.Y = 1;
                lookoutDir.X = lookLeft?1:-1;
                lookoutDir.Y = 0;
                break;
            default:
                Console.WriteLine("Unknown instruction dir: {0}", instruction.Item1);
                break;
        }

        for(int j = 0; j < instruction.Item2; j++){
            currentPoint.X+=direction.X;
            currentPoint.Y+=direction.Y;
            Point lookPos = new Point(currentPoint.X + lookoutDir.X,currentPoint.Y + lookoutDir.Y); //MAX AND MIN NECESSARY
            //Console.WriteLine("At point ({0},{1}) we look at point ({2},{3}) | Dir: {4}, lookoutDir: {5},{6}", currentPoint.X,currentPoint.Y,lookPos.X,lookPos.Y,instruction.Item1, lookoutDir.X,lookoutDir.Y);
            LookAtTile(lookPos);
        }
    }

    if(outside){
        result = (mapSizeX+1-minSizeX) * (mapSizeY+1-minSizeY) + chainSize - map.Count;
    }
    else{
        result = map.Count;
    }
    




    //DEBUG LOG
    for(int i = minSizeY; i <= mapSizeY; i++){
        string mapLine = "";
        for(int j = minSizeX; j <= mapSizeX; j++){
            Point point = new Point(j, i);
            if(map.ContainsKey(point)){
                if(map[point]==Color.White){
                    Console.Write(fillChar);
                    mapLine+=fillChar;
                }
                else{
                    Console.Write(diggedChar);
                    mapLine+=diggedChar;
                }
            }
            else{
                Console.Write(emptyChar);
                mapLine+=emptyChar;
            }
        }
        Console.WriteLine();
        LogInOutput(mapLine);
    }
    LogInOutput("");
    LogInOutput("");
    
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