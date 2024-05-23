using System.Drawing;
using System.Globalization;
using System.Runtime;

string line;
double result = 0;
List<string> stringMap = new List<string>();
Dictionary<Point,char> map = new Dictionary<Point,char>();
List<Point> dirs = new List<Point>{new Point(1,0),new Point(-1,0),new Point(0,1),new Point(0,-1)}; //East, West, South, North
List<Point> currentPos = new List<Point>();
int nbSteps = 64;

//For part 2
Point startPos = new Point(0,0);
int nbSteps2 = 26501365;
List<Point> fullCoveredMapPoints = new List<Point>();

bool InBound(Point nPos)
{
    return nPos.X >= 0 && nPos.X < stringMap[0].Length && nPos.Y >= 0 && nPos.Y < stringMap.Count;
}

Point GetInBoundEquivalent(Point nPos,bool infinit)
{ 
    if(!infinit){
        return nPos;
    }
    else{
        Point inBoundPoint = new Point(nPos.X%stringMap[0].Length,nPos.Y%stringMap.Count);
        inBoundPoint.X = inBoundPoint.X<0?inBoundPoint.X+stringMap[0].Length:inBoundPoint.X;
        inBoundPoint.Y = inBoundPoint.Y<0?inBoundPoint.Y+stringMap.Count:inBoundPoint.Y;
        return inBoundPoint;
    }
}

void MakeAStep(bool infinit = false)
{
    List<Point> nextPos = new List<Point>();
    foreach(Point pos in currentPos){
        foreach(Point dir in dirs){
            Point nPos = new Point(pos.X+dir.X,pos.Y+dir.Y);
            if((infinit || InBound(nPos)) && !nextPos.Contains(nPos)){
                Point toLook = GetInBoundEquivalent(nPos,infinit);
                if(map[toLook]!='#'){
                    nextPos.Add(nPos);
                }
            }
        }
    }
    currentPos = nextPos;
}

bool RightNbOfSteps(int X, int Y, bool evenStepsFromStart, bool evenStartX, bool evenStartY){
    bool currentXEven = X%2==0;
    bool sameXSign = (currentXEven && evenStartX) || (!currentXEven && !evenStartX);
    bool currentYEven = Y%2==0;
    bool sameYSign = (currentYEven && evenStartY) || (!currentYEven && !evenStartY);
    bool sameSign = (sameXSign && sameYSign) || (!sameXSign && !sameYSign);
    return (evenStepsFromStart && sameSign) || (!evenStepsFromStart && !sameSign);
}

try
{
    //To get the time of code execution!
    var watch = System.Diagnostics.Stopwatch.StartNew();

    //Pass the file path and file name to the StreamReader constructor
    StreamReader sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\Input.txt");

    //Read the first line of text
    line = sr.ReadLine();

    int yCoord = 0;

    //Continue to read until you reach end of file
    while (line != null)
    {
        //write the line to console window
        Console.WriteLine(line);

        stringMap.Add(line);

        int xCoord = 0;
        foreach(char c in line){
            map[new Point(xCoord,yCoord)] = c;
            if(c=='S'){
                startPos = new Point(xCoord,yCoord);
                currentPos.Add(startPos); //Start Pos
            }
            
            xCoord++;
        }

        //Read the next line
        line = sr.ReadLine();
        yCoord++;
    }

    //close the file
    sr.Close();
    
    //PART 1
    
    for(int i=0; i<nbSteps; i++){
        MakeAStep(true);
    }
    result = currentPos.Count;
    Console.WriteLine();
    Console.WriteLine("End of input. Result game 1 found: {0}",result);

    /*
    //DEBUG LOG
    Console.WriteLine();
    for(int j=0;j<stringMap.Count;j++){
        for(int i=0;i<stringMap[0].Length;i++){
            Point coord = new Point(i,j);
            if(currentPos.Contains(coord)){
                Console.Write('O');
            }
            else{
                Console.Write(map[coord]);
            }
        }
        Console.WriteLine();
    }
    */

    //Part 2
    //For part 2, doing the regulat way will take way too much time, and naive function we use is losing too much time on useless calculation.
    //By looking a bit closer at each step state, we can see that for each cells that are reachable, we have two states at each step. Either we step on the cell at each even step and not for odd steps, or vice versa.
    //With this we could check for a full board the reached steps, multiply by the number of complete board reached and solve the borders.

    //An idea would be to to the serach for steps that does not cover a full 3x3 square where each cell is the given map. For everything else we add the full map we got.
    //And here I got stuck by the borders growing each time we move. I will search clues from now on.

    /*
    //Naive solution, won't give you the answer after a loooooooong time, if any.
    currentPos.Clear();
    currentPos.Add(startPos);
    for(int i=0; i<nbSteps2; i++){
        MakeAStep(true);
    }
    result = currentPos.Count;
    */

    /*
    //This does not take into account blocked cells !!!
    result = 0;
    bool evenStepsNb = nbSteps%2==0;
    bool evenY = startPos.Y%2==0;
    bool evenX = startPos.X%2==0;
    for(int j=0;j<stringMap.Count;j++){
        for(int i=0;i<stringMap[0].Length;i++){
            if(RightNbOfSteps(i,j,evenStepsNb,evenX,evenY) && stringMap[j][i]!='#'){
                fullCoveredMapPoints.Add(new Point(i,j));
            }
        }
    }
    */

    //Previous steps were leading me closer and closer to the solution. The last bit I had missing was taking into account the borders. 
    //Also the fact that we may have full maps that are even filled and others odd filled.

    result = 0;
    double previousCount = -1;
    int step = 0;
    currentPos.Clear();
    currentPos.Add(startPos);
    while(result!=previousCount || step%2!=0){
        if(step%2==0){
            previousCount=result;
        }
        MakeAStep();
        result = currentPos.Count;
        step++;
    }
    double evenFilledCount = result;

    result = 0;
    previousCount = -1;
    step = 0;
    currentPos.Clear();
    currentPos.Add(startPos);
    while(result!=previousCount || step%2==0){
        if(step%2!=0){
            previousCount=result;
        }
        MakeAStep();
        result = currentPos.Count;
        step++;
    }
    double oddFilledCount = result;

    Console.WriteLine();
    Console.WriteLine("We get: even={0} and odd={1}",evenFilledCount,oddFilledCount);


    Console.WriteLine();
    Console.WriteLine("Map size: {0},{1}",stringMap[0].Length,stringMap.Count);
    int farthestBorderDistance = Math.Max(startPos.X,Math.Max(startPos.Y,Math.Max(stringMap[0].Length-startPos.X,stringMap.Count-startPos.Y)))-1;
    //We assume that maps are squared...
    int remainingSteps = (nbSteps2-farthestBorderDistance)%stringMap.Count;
    int fullSquared = (nbSteps2-farthestBorderDistance)/stringMap.Count;
    Console.WriteLine("We got: steps={3}, farthestBorderDistance={0}, remainingSteps={1}, fullSquared={2}",farthestBorderDistance,remainingSteps,fullSquared,nbSteps2);

    


    //Again, this solution is too simple and.. false. I assumed corners of our diamonds would gently match other side corners but... that is a false assumption.
    /*
    if(fullSquared>0){
        if(nbSteps2%2==0){
            result+=(fullSquared+1)*(fullSquared+1)*oddFilledCount + fullSquared*(fullSquared+3)*evenFilledCount;
        }
        else{  
            result+=(fullSquared+1)*(fullSquared+1)*evenFilledCount + fullSquared*(fullSquared+3)*oddFilledCount;
        }
    }
    */

    //We need to calculate the filled part for each one of them, and use that to calculate more efficiently
    //We can actually make use of our 3x3 square, since we have corners and cropped tiles in it.

    /*
    currentPos.Clear();
    currentPos.Add(startPos);
    for(int i=0; i<remainingSteps+farthestBorderDistance; i++){
        MakeAStep(true);
    }
    result = currentPos.Count;
    */

    //Based on the site https://work.njae.me.uk/2023/12/29/advent-of-code-2023-day-21/ we would even need, in order to be more precise, to check on a 5x5 square.
    //Let's do that. (If fullSquared is odd, we would need a 6x6 square...)
    //It is based on the fact that the given number of steps leads us to the edge of the last seen map if we go on a straight line horizontally or vertically...

    currentPos.Clear();
    currentPos.Add(startPos);
    int nbMapToGoThrough = 2;
    if(fullSquared%2!=0){
        nbMapToGoThrough++;
    }
    int checkSteps = farthestBorderDistance + 2*stringMap.Count;
    for(int i=0; i<checkSteps; i++){
        MakeAStep(true);
    }

    int topRightCornersBit = 0;
    int botRightCornersBit = 0;
    int topLeftCornersBit = 0;
    int botLeftCornersBit = 0;
    int topHat = 0;
    int botHat = 0;
    int rightHat = 0;
    int leftHat = 0;

    for(int i=0; i<nbMapToGoThrough-1; i++){
        foreach(var point in currentPos){
            if(point.X>stringMap.Count+i*stringMap.Count && point.X<(nbMapToGoThrough-i)*stringMap.Count && point.Y<(nbMapToGoThrough-1-(i+1))*stringMap.Count){
                topRightCornersBit++;
            }
            else if(point.X>stringMap.Count+i*stringMap.Count && point.X<(nbMapToGoThrough-i)*stringMap.Count && point.Y>stringMap.Count+((nbMapToGoThrough-1-(i+1))*stringMap.Count)){
                botRightCornersBit++;
            }
            else if(point.X>-(i+1)*stringMap.Count && point.X<-i*stringMap.Count && point.Y<(nbMapToGoThrough-1-(i+1))*stringMap.Count){
                topRightCornersBit++;
            }
            else if(point.X>-(i+1)*stringMap.Count && point.X<-i*stringMap.Count && point.Y>stringMap.Count+((nbMapToGoThrough-1-(i+1))*stringMap.Count)){
                botRightCornersBit++;
            }
            else if(i==0){
                //On ne passe qu'une fois dans cette boucle
                if(point.X>0 && point.X<stringMap.Count && point.Y<-(nbMapToGoThrough-1)*stringMap.Count){
                    topHat++;
                }
                else if(point.X>0 && point.X<stringMap.Count && point.Y>nbMapToGoThrough*stringMap.Count){
                    botHat++;
                }
                else if(point.X>nbMapToGoThrough*stringMap.Count){
                    rightHat++;
                }
                else if(point.X<-(nbMapToGoThrough-1)*stringMap.Count){
                    leftHat++;
                }
            }
        }
    }


    if(fullSquared>0){
        if(fullSquared%2==0){
            result=fullSquared*fullSquared*oddFilledCount + (fullSquared-1)*(fullSquared-1)*evenFilledCount + (fullSquared-1)*(topRightCornersBit+botRightCornersBit+topLeftCornersBit+botLeftCornersBit) + (topHat+botHat+leftHat+rightHat);
        }
        else{  
            result=fullSquared*fullSquared*evenFilledCount + (fullSquared-1)*(fullSquared-1)*oddFilledCount + ((fullSquared-1)/2)*(topRightCornersBit+botRightCornersBit+topLeftCornersBit+botLeftCornersBit) + (topHat+botHat+leftHat+rightHat);
        }
    }

    /*
    //Output log DEBUG
    string filename1 = "Test1DebugLogs";
    string filename2 = "Test2DebugLogs";
    for(int j = 0; j<stringMap.Count; j++){
        string mapLine1 = "";
        string mapLine2 = "";
        for(int i = 0; i<stringMap[0].Length; i++){
            Point point = new Point(j, i);
            if(currentPos.Contains(point)){
                mapLine1+='O';
                
            }
            else{
                mapLine1+=map[point];
            }
            if(test2Points.Contains(point)){
                mapLine2+='O';
                
            }
            else{
                mapLine2+=map[point];
            }
        }
        //Console.WriteLine();
        LogInOutput(mapLine1,filename1);
        LogInOutput(mapLine2,filename2);
    }
    */

    Console.WriteLine();
    Console.WriteLine("End of input. Result game 2 found: {0}",result);

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