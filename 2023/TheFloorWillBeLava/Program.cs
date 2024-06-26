﻿
using System.Drawing;

string line;
double result1 = 0;
List<string> mapString = new List<string>();
Dictionary<Point, List<char>> PoweredCells = new Dictionary<Point, List<char>>();

//For Part 2
double result2 = 0;


bool CoordInMap(Point coord){
    if(coord.X >= 0 && coord.X<mapString[0].Length && coord.Y >= 0 && coord.Y<mapString.Count){
        return true;
    }
    return false;
}

List<Tuple<Point,char>> Powering(Point coord, char dir){
    //Dir: 'r'=right, 'l'=left, 'u'=up, 'd'=down
    //A recursive function here leads to a stack overflow even on part 1.
    List<Tuple<Point,char>> result = new List<Tuple<Point, char>>();

    if(PoweredCells.ContainsKey(coord)){
        if(PoweredCells[coord].Contains(dir)){
            return result;
        }
        PoweredCells[coord].Add(dir);
    }
    else{
        PoweredCells[coord] = new List<char>{dir};
    }
    char cell = mapString[coord.Y][coord.X]; 
    Point coordA = new Point(-1,-1);
    char dirA = dir;
    Point coordB = new Point(-1,-1);
    char dirB = dir;
    switch(dir){
        case 'r':
            if(cell == '.' || cell == '-'){
                coordA.X = coord.X+1;
                coordA.Y = coord.Y;
            }
            else if(cell == '\\'){
                coordA.X = coord.X;
                coordA.Y = coord.Y+1;
                dirA = 'd';
            }
            else if(cell == '/'){
                coordA.X = coord.X;
                coordA.Y = coord.Y-1;
                dirA = 'u';
            }
            else if(cell == '|'){
                coordA.X = coord.X;
                coordA.Y = coord.Y-1;
                dirA = 'u';
                coordB.X = coord.X;
                coordB.Y = coord.Y+1;
                dirB = 'd';
            }
            else{
                Console.WriteLine("Unknown cell {0}",cell);
            }
            break;
        case 'l':
            if(cell == '.' || cell == '-'){
                coordA.X = coord.X-1;
                coordA.Y = coord.Y;
            }
            else if(cell == '\\'){
                coordA.X = coord.X;
                coordA.Y = coord.Y-1;
                dirA = 'u';
            }
            else if(cell == '/'){
                coordA.X = coord.X;
                coordA.Y = coord.Y+1;
                dirA = 'd';
            }
            else if(cell == '|'){
                coordA.X = coord.X;
                coordA.Y = coord.Y-1;
                dirA = 'u';
                coordB.X = coord.X;
                coordB.Y = coord.Y+1;
                dirB = 'd';
            }
            else{
                Console.WriteLine("Unknown cell {0}",cell);
            }
            break;
        case 'u':
            if(cell == '.' || cell == '|'){
                coordA.X = coord.X;
                coordA.Y = coord.Y-1;
            }
            else if(cell == '\\'){
                coordA.X = coord.X-1;
                coordA.Y = coord.Y;
                dirA = 'l';
            }
            else if(cell == '/'){
                coordA.X = coord.X+1;
                coordA.Y = coord.Y;
                dirA = 'r';
            }
            else if(cell == '-'){
                coordA.X = coord.X-1;
                coordA.Y = coord.Y;
                dirA = 'l';
                coordB.X = coord.X+1;
                coordB.Y = coord.Y;
                dirB = 'r';
            }
            else{
                Console.WriteLine("Unknown cell {0}",cell);
            }
            break;
        case 'd':
            if(cell == '.' || cell == '|'){
                coordA.X = coord.X;
                coordA.Y = coord.Y+1;
            }
            else if(cell == '\\'){
                coordA.X = coord.X+1;
                coordA.Y = coord.Y;
                dirA = 'r';
            }
            else if(cell == '/'){
                coordA.X = coord.X-1;
                coordA.Y = coord.Y;
                dirA = 'l';
            }
            else if(cell == '-'){
                coordA.X = coord.X-1;
                coordA.Y = coord.Y;
                dirA = 'l';
                coordB.X = coord.X+1;
                coordB.Y = coord.Y;
                dirB = 'r';
            }
            else{
                Console.WriteLine("Unknown cell {0}",cell);
            }
            break;
        default:
            Console.WriteLine("Unknown dir {0}",dir);
            break;
    }

    if(CoordInMap(coordA)){
        //Console.WriteLine("Gets us {0},{1} by {2}",coordA.X,coordA.Y,dirA);
        result.Add(Tuple.Create(coordA,dirA));
    }
    if(CoordInMap(coordB)){
        //Console.WriteLine("Gets us {0},{1} by {2}",coordB.X,coordB.Y,dirB);
        result.Add(Tuple.Create(coordB,dirB));
    }
    return result;
}

void NonRecursivePowering(List<Tuple<Point,char>> nextCoord){
    List<List<Tuple<Point,char>>> allCoords = new List<List<Tuple<Point,char>>>{nextCoord};
    while(allCoords.Count>0){
        List<List<Tuple<Point,char>>> newAllCoords = new List<List<Tuple<Point,char>>>();
        foreach(List<Tuple<Point,char>> coords in allCoords){
            List<Tuple<Point,char>> newCoords = new List<Tuple<Point,char>>();
            foreach (Tuple<Point,char> coord in coords){
                //Console.WriteLine("Going for {0},{1} by {2}",coord.Item1.X,coord.Item1.Y,coord.Item2);
                newCoords = Powering(coord.Item1,coord.Item2);
                //Console.WriteLine("So we have a new list of {0} coords to go through.",newCoords.Count);
                if(newCoords.Count > 0){    
                    newAllCoords.Add(newCoords);
                }
            }
        }
        allCoords = newAllCoords;
    }
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

        mapString.Add(line);

        //Read the next line
        line = sr.ReadLine();
    }

    //close the file
    sr.Close();

    Point topLeft = new Point(0, 0);
    List<Tuple<Point,char>> topLeftRight = new List<Tuple<Point,char>>{Tuple.Create(topLeft,'r')};
    NonRecursivePowering(topLeftRight);

    result1 = PoweredCells.Count;

    Console.WriteLine("");
    Console.WriteLine("End of input. Result game 1 found: {0}",result1);

    

    //For part 2
    //We will do the same as part 1 but for all possible starting points and get the best result.
    result2 = result1;
    List<List<Tuple<Point,char>>> allStartingPoints = new List<List<Tuple<Point,Char>>>();

    List<char> directions = new List<char>{'l','u','r','d'};
    foreach(char dir in directions){
        if(dir == 'l' || dir == 'r'){
            for(int i = 0; i<mapString.Count; i++){               
                int startX = dir=='r'?0:mapString[0].Length-1;
                Point startPoint = new Point(startX, i);
                List<Tuple<Point,char>> start = new List<Tuple<Point,char>>{Tuple.Create(startPoint,dir)};
                allStartingPoints.Add(start);
            }
        }
        else if(dir == 'u' || dir == 'd'){
            for(int i = 0; i<mapString[0].Length; i++){               
                int startY = dir=='d'?0:mapString.Count-1;
                Point startPoint = new Point(i, startY);
                List<Tuple<Point,char>> start = new List<Tuple<Point,char>>{Tuple.Create(startPoint,dir)};
                allStartingPoints.Add(start);
            }
        }
    }


    foreach(List<Tuple<Point,char>> start in allStartingPoints){
        PoweredCells.Clear();
        NonRecursivePowering(start);
        //Console.WriteLine("For start: {0},{1} by {2}, we get {3} powered cells",start[0].Item1.X,start[0].Item1.Y,start[0].Item2,PoweredCells.Count);
        if(PoweredCells.Count>result2){
            result2 = PoweredCells.Count;
        }
    }

    Console.WriteLine("End of input. Result game 2 found: {0}",result2);

}
catch(Exception e)
{
    Console.WriteLine("Exception: " + e.ToString());
}
finally
{
    Console.WriteLine("END");
}