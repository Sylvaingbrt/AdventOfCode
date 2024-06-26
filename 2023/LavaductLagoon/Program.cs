﻿using System.Drawing;

string line;
double result = 0;
Dictionary<Point,Color> map = new Dictionary<Point,Color>();
List<Tuple<char,int>> instructions = new List<Tuple<char,int>>();
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
double chainSize = 0;

//For part 2
Color digColor= Color.Black;
List<Tuple<char,int>> correctedInstructions = new List<Tuple<char,int>>();
Dictionary<Point,Color> mapCorrected = new Dictionary<Point,Color>();
Dictionary<int,List<int>> leftBorders = new Dictionary<int,List<int>>();
Dictionary<int,List<int>> rightBorders = new Dictionary<int,List<int>>();
HashSet<int> leftKeys = new HashSet<int>();
HashSet<int> rightKeys = new HashSet<int>();

Tuple<char, int> ConvertInstruction(string input, bool corrected = false)
{
    char dir;
    int length;
    if (corrected){
        string encryptedInstruction = input.Split(" ")[2].Substring(2,6);
        char[] dirs = new char[]{'R','D','L','U'};
        dir = dirs[encryptedInstruction[encryptedInstruction.Length-1]-48];
        length = Convert.ToInt32(encryptedInstruction.Substring(0,5),16);
    }
    else{
        dir = input.Split(" ")[0][0];
        length = int.Parse(input.Split(" ")[1]);
    }
    Tuple<char,int> result = Tuple.Create(dir,length);
    return result;
}

void FillPosAndGetNextPos(ref List<Point> posToFill, Dictionary<Point,Color> mapToFill){
    //Function taken from day 10 ! Works well on small size and is very consistent but too slow for big data
    Point pos = posToFill[0];
    posToFill.Remove(pos);
    if(!mapToFill.ContainsKey(pos)){
        mapToFill[pos] = fillColor;
        if(pos.X==minSizeX || pos.X==mapSizeX || pos.Y==minSizeY || pos.Y==mapSizeY){
            outside = true;
        }
        Point[] newPoints = new Point[]{
            new Point(Math.Max(pos.X-1,minSizeX),pos.Y),
            new Point(Math.Min(pos.X+1,mapSizeX),pos.Y),
            new Point(pos.X,Math.Max(pos.Y-1,minSizeY)),
            new Point(pos.X,Math.Min(pos.Y+1,mapSizeY))
            };
        foreach(Point newPos in newPoints){
            if(!posToFill.Contains(newPos)){
                posToFill.Add(newPos);
            }
        }
    }
}

void AddBorder(ref Dictionary<int,List<int>> bordersList, int column, int row, ref HashSet<int> keys){
    if(!keys.Contains(column)){
        List<int> borders = new List<int>{row};
        bordersList[column] = borders;
        if(!keys.Add(column)){
            Console.WriteLine("Rewrite possible at column "+column);
        }
    }
    else{
        if(column==-46){

        }
        bordersList[column].Add(row);
    }
    
}

void DrawMap(List<Tuple<char,int>> instructionsToFollow, Dictionary<Point,Color> mapToDraw, bool keepTrackOfBorders){
    Point currentPoint = new Point(0,0);
    mapToDraw[currentPoint] = digColor;
    Point direction = new Point(0,0);
    mapSizeX = -1;
    mapSizeY = -1;
    minSizeX = 0;
    minSizeY = 0;
    char prevDir = 'N';
    char dirToConsider = prevDir;
    bool addToBorder = false;
    bool addEvenFirstToBorder = false;
    if(keepTrackOfBorders){
        leftBorders = new Dictionary<int, List<int>>();
        rightBorders = new Dictionary<int, List<int>>();
        leftKeys = new HashSet<int>();
        rightKeys = new HashSet<int>();
    }
    for(int i=0; i<instructionsToFollow.Count; i++){
        addToBorder = false;
        var instruction = instructionsToFollow[i];
        switch(instruction.Item1){
            case 'L':
                direction.X = -1;
                direction.Y = 0;
                if(keepTrackOfBorders && ((!lookLeft && prevDir=='U') || (lookLeft && prevDir=='D'))){
                    addToBorder = true;
                    dirToConsider = prevDir;
                }
                break;
            case 'R':
                direction.X = 1;
                direction.Y = 0;
                if(keepTrackOfBorders && ((!lookLeft && prevDir=='D') || (lookLeft && prevDir=='U'))){
                    addToBorder = true;
                    dirToConsider = prevDir;
                }
                break;
            case 'U':
                direction.X = 0;
                direction.Y = -1;
                //addToBorder = keepTrackOfBorders;
                if(keepTrackOfBorders && (prevDir=='U' || (!lookLeft && prevDir=='R') || (lookLeft && prevDir=='L'))){
                    addToBorder = true;
                    dirToConsider = instruction.Item1;
                }
                break;
            case 'D':
                direction.X = 0;
                direction.Y = 1;
                //addToBorder = keepTrackOfBorders;
                if(keepTrackOfBorders && (prevDir=='D' || (!lookLeft && prevDir=='L') || (lookLeft && prevDir=='R'))){
                    addToBorder = true;
                    dirToConsider = instruction.Item1;
                }
                break;
            default:
                Console.WriteLine("Unknown instruction dir: {0}", instruction.Item1);
                break;
        }

        for(int j = 0; j < instruction.Item2; j++){

            if(addToBorder){
                if(dirToConsider=='U'){
                    //Console.WriteLine("Add left Border : ({0},{1})", currentPoint.X,currentPoint.Y-minSizeY);
                    AddBorder(ref leftBorders,currentPoint.Y,currentPoint.X,ref leftKeys);
                }
                else{
                    //Console.WriteLine("Add right Border : ({0},{1})", currentPoint.X,currentPoint.Y-minSizeY);
                    AddBorder(ref rightBorders,currentPoint.Y,currentPoint.X,ref rightKeys);
                }   
            }
            addToBorder = keepTrackOfBorders && (instruction.Item1=='U' || instruction.Item1=='D');
            dirToConsider = instruction.Item1;

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

            if(mapToDraw.ContainsKey(currentPoint)){
                Console.WriteLine("Point aleady digged at ({0},{1})", currentPoint.X,currentPoint.Y);
            }
            else{
                mapToDraw[currentPoint] = digColor;
            }
        }
        
        prevDir = instruction.Item1;
    }
}

void FillMap(List<Tuple<char,int>> instructionsToFollow, Dictionary<Point,Color> mapToFill){
    Point currentPoint = new Point(0,0);
    Point direction = new Point(0,0);
    Point lookoutDir = new Point(0,0);
    for(int i=0; i<instructionsToFollow.Count; i++){
        var instruction = instructionsToFollow[i];
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
            List<Point> posToFill = new List<Point>{lookPos};
            while(posToFill.Count!=0){
                FillPosAndGetNextPos(ref posToFill,mapToFill);
            }
        }
    }
}

double GetBordersAndChainSize(List<Tuple<char,int>> instructionsToFollow){
    double chainSizeResult = 0;
    Point currentPoint = new Point(0,0);
    Point direction = new Point(0,0);
    mapSizeX = -1;
    mapSizeY = -1;
    minSizeX = 0;
    minSizeY = 0;
    char prevDir = instructionsToFollow[instructionsToFollow.Count-1].Item1;
    char dirToConsider = prevDir;
    bool addToBorder = false;
    leftBorders = new Dictionary<int, List<int>>();
    rightBorders = new Dictionary<int, List<int>>();
    leftKeys = new HashSet<int>();
    rightKeys = new HashSet<int>();
    for(int i=0; i<instructionsToFollow.Count; i++){
        addToBorder = false;
        var instruction = instructionsToFollow[i];
        switch(instruction.Item1){
            case 'L':
                direction.X = -1;
                direction.Y = 0;
                if((!lookLeft && prevDir=='U') || (lookLeft && prevDir=='D')){
                    addToBorder = true;
                    dirToConsider = prevDir;
                }
                break;
            case 'R':
                direction.X = 1;
                direction.Y = 0;
                if((!lookLeft && prevDir=='D') || (lookLeft && prevDir=='U')){
                    addToBorder = true;
                    dirToConsider = prevDir;
                }
                break;
            case 'U':
                direction.X = 0;
                direction.Y = -1;
                //addToBorder = keepTrackOfBorders;
                if(prevDir=='N' || prevDir=='U' || (!lookLeft && prevDir=='R') || (lookLeft && prevDir=='L')){
                    addToBorder = true;
                    dirToConsider = instruction.Item1;
                }
                break;
            case 'D':
                direction.X = 0;
                direction.Y = 1;
                //addToBorder = keepTrackOfBorders;
                if(prevDir=='N' || prevDir=='D' || (!lookLeft && prevDir=='L') || (lookLeft && prevDir=='R')){
                    addToBorder = true;
                    dirToConsider = instruction.Item1;
                }
                break;
            default:
                Console.WriteLine("Unknown instruction dir: {0}", instruction.Item1);
                break;
        }
        if(instruction.Item1=='U'||instruction.Item1=='D'){
            for(int j = 0; j < instruction.Item2; j++){

                if(addToBorder){
                    if(dirToConsider=='U'){
                        //Console.WriteLine("Add left Border : ({0},{1}), current min: {2}", currentPoint.X,currentPoint.Y,minSizeY);
                        AddBorder(ref leftBorders,currentPoint.Y,currentPoint.X,ref leftKeys);
                    }
                    else{
                        //Console.WriteLine("Add right Border : ({0},{1}), current min: {2}", currentPoint.X,currentPoint.Y,minSizeY);
                        AddBorder(ref rightBorders,currentPoint.Y,currentPoint.X,ref rightKeys);
                    }   
                }
                addToBorder = instruction.Item1=='U' || instruction.Item1=='D';
                dirToConsider = instruction.Item1;

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
            }
        }
        else{
            if(addToBorder){
                if(dirToConsider=='U'){
                    //Console.WriteLine("Add left Border : ({0},{1}), current min: {2}", currentPoint.X,currentPoint.Y,minSizeY);
                    AddBorder(ref leftBorders,currentPoint.Y,currentPoint.X,ref leftKeys);
                }
                else{
                    //Console.WriteLine("Add right Border : ({0},{1}), current min: {2}", currentPoint.X,currentPoint.Y,minSizeY);
                    AddBorder(ref rightBorders,currentPoint.Y,currentPoint.X,ref rightKeys);
                }   
            }
            currentPoint.X+=instruction.Item2*direction.X;
        }
        chainSizeResult += instruction.Item2;
        prevDir = instruction.Item1;
    }
    //Console.WriteLine("chain size: {0}",chainSizeResult);
    return chainSizeResult;
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
        correctedInstructions.Add(ConvertInstruction(line,true));

        //Read the next line
        line = sr.ReadLine();
    }

    //close the file
    sr.Close();
    
    Console.WriteLine("");
    //PART 1
    //Draw map:
    DrawMap(instructions, map, false);

    chainSize = map.Count;

    //We go through all instructions again, looking at a side of our path, and fill empty spaces. It is the same logic as day 10 probelm.
    
    FillMap(instructions, map);

    if(outside){
        result = (mapSizeX+1-minSizeX) * (mapSizeY+1-minSizeY) + chainSize - map.Count;
    }
    else{
        result = map.Count;
        Console.WriteLine("Inside size: {0}, chain size {1}",map.Count-chainSize,chainSize);
    }
    
    


/*
    //DEBUG LOG
    for(int i = minSizeY; i <= mapSizeY; i++){
        string mapLine = "";
        for(int j = minSizeX; j <= mapSizeX; j++){
            Point point = new Point(j, i);
            if(map.ContainsKey(point)){
                if(map[point]==Color.White){
                    //Console.Write(fillChar);
                    mapLine+=fillChar;
                }
                else{
                    //Console.Write(diggedChar);
                    mapLine+=diggedChar;
                }
            }
            else{
                //Console.Write(emptyChar);
                mapLine+=emptyChar;
            }
        }
        //Console.WriteLine();
        LogInOutput(mapLine);
    }
    LogInOutput("");
    LogInOutput("");
*/

    Console.WriteLine();
    Console.WriteLine("End of input. Result game 1 found: {0}",result);
    Console.WriteLine();

    //PART 2
    result = 0;

    /*
    //DEBUG LOG
    Console.WriteLine();
    Console.WriteLine("New instructions:");
    foreach(var correctedInstruction in correctedInstructions){
        Console.WriteLine("{0} {1}",correctedInstruction.Item1, correctedInstruction.Item2);
    }
    */

    //Draw map:
    //DrawMap(correctedInstructions, mapCorrected, true);

    chainSize = GetBordersAndChainSize(correctedInstructions);

    //for(int i = 0; i < leftBorders.Count; i++){
    foreach(var element in leftBorders){
        leftBorders[element.Key].Sort();
        rightBorders[element.Key].Sort();
        //Console.WriteLine("At ligne {0}, we have {1} left borders and {2} right borders",element.Key,leftBorders[element.Key].Count,rightBorders[element.Key].Count);
        for(int j = 0; j < element.Value.Count; j++){
            int bordRight = Math.Max(leftBorders[element.Key][j],rightBorders[element.Key][j]);
            int bordLeft = Math.Min(leftBorders[element.Key][j],rightBorders[element.Key][j]);
            result+=bordRight-bordLeft-1;
        }
        
    }
    
    Console.WriteLine("Inside size: {0}, chain size {1}",result,chainSize);
    result += chainSize;

    Console.WriteLine();
    Console.WriteLine("End of input. Result game 2 found: {0}",result);
    Console.WriteLine();

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