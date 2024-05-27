using System.Numerics;

string line;
double result=0;
Dictionary<int,List<int>> bricksByZLevel = new Dictionary<int,List<int>>();
Dictionary<int,List<int>> bricksSupportingABrick = new Dictionary<int,List<int>>();
Dictionary<int,List<int>> bricksSupportedByBrick = new Dictionary<int,List<int>>();
Dictionary<int,List<Vector3>> bricks = new Dictionary<int, List<Vector3>>();
List<int> zLevelBlocking = new List<int>();
int maxZLevel = 0;
int maxXLevel = 0;
int maxYLevel = 0;

bool Overlaping(int x1Min, int x1Max, int y1Min, int y1Max, int x2Min, int x2Max, int y2Min, int y2Max){
    return ((x1Min<=x2Min && x1Max>=x2Min) || (x1Min<=x2Max && x1Max>=x2Max) || (x1Min>x2Min && x1Max<x2Max)) && ((y1Min<=y2Min && y1Max>=y2Min) || (y1Min<=y2Max && y1Max>=y2Max) || (y1Min>y2Min && y1Max<y2Max));
}

List<int> IntersectAtLevelWith(int newLevel, int brickNumber)
{
    List<int> resultList = new List<int>();
    int xMin = (int) Math.Min(bricks[brickNumber][0].X,bricks[brickNumber][1].X);
    int xMax = (int) Math.Max(bricks[brickNumber][0].X,bricks[brickNumber][1].X);
    int yMin = (int) Math.Min(bricks[brickNumber][0].Y,bricks[brickNumber][1].Y);
    int yMax = (int) Math.Max(bricks[brickNumber][0].Y,bricks[brickNumber][1].Y);
    //Check if X axis or Y axis overlaps, for all bricks at level and for zLevelBlocking
    if(bricksByZLevel.ContainsKey(newLevel)){
        foreach(int brickNum in bricksByZLevel[newLevel]){
            int xOtherMin = (int) Math.Min(bricks[brickNum][0].X,bricks[brickNum][1].X);
            int xOtherMax = (int) Math.Max(bricks[brickNum][0].X,bricks[brickNum][1].X);
            int yOtherMin = (int) Math.Min(bricks[brickNum][0].Y,bricks[brickNum][1].Y);
            int yOtherMax = (int) Math.Max(bricks[brickNum][0].Y,bricks[brickNum][1].Y);
            if(Overlaping(xMin,xMax,yMin,yMax,xOtherMin,xOtherMax,yOtherMin,yOtherMax)){
                resultList.Add(brickNum);
            }
        }
    }
    foreach(int brickNum in zLevelBlocking){
        //Console.WriteLine("At level {0}, we have brick {1} that may block the path (from {2} to {3}) <- whatching brick {4}",newLevel,brickNum,Math.Min(bricks[brickNum][0].Z, bricks[brickNum][1].Z),Math.Max(bricks[brickNum][0].Z, bricks[brickNum][1].Z),brickNumber);
        if(Math.Min(bricks[brickNum][0].Z, bricks[brickNum][1].Z)<newLevel && Math.Max(bricks[brickNum][0].Z, bricks[brickNum][1].Z)>=newLevel){
            //Console.WriteLine("Intersection possible");
            int xOtherMin = (int) Math.Min(bricks[brickNum][0].X,bricks[brickNum][1].X);
            int xOtherMax = (int) Math.Max(bricks[brickNum][0].X,bricks[brickNum][1].X);
            int yOtherMin = (int) Math.Min(bricks[brickNum][0].Y,bricks[brickNum][1].Y);
            int yOtherMax = (int) Math.Max(bricks[brickNum][0].Y,bricks[brickNum][1].Y);
            if(Overlaping(xMin,xMax,yMin,yMax,xOtherMin,xOtherMax,yOtherMin,yOtherMax)){
                //Console.WriteLine("Intersection at level {0}, between brick {1} and brick {2}",newLevel,brickNum,brickNumber);
                resultList.Add(brickNum);
            }
        }
    }
    return resultList;
}

void DrawBricks(bool Xaxis = true){
    Console.WriteLine();
    int maxIndex = Xaxis?maxXLevel:maxYLevel;
    for(int i = maxZLevel; i>=1; i--){
        List<int> bricksAtThisLevel = new List<int>();
        if(bricksByZLevel.ContainsKey(i)){
            bricksAtThisLevel=bricksByZLevel[i];
        }
        for(int j = 0; j <= maxIndex;j++){
            int numbOfBricks = 0;
            foreach(int brickNum in bricksAtThisLevel){
                int minVal = (int) (Xaxis?Math.Min(bricks[brickNum][0].X,bricks[brickNum][1].X):Math.Min(bricks[brickNum][0].Y,bricks[brickNum][1].Y));
                int maxVal = (int) (Xaxis?Math.Max(bricks[brickNum][0].X,bricks[brickNum][1].X):Math.Max(bricks[brickNum][0].Y,bricks[brickNum][1].Y));
                if(j>=minVal && j<=maxVal){
                    numbOfBricks++;
                }
            }
            Console.Write(numbOfBricks);
        }
        Console.WriteLine();
    }
    for(int j = 0; j <= maxIndex;j++){
        Console.Write("-");
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

    int brickNum = 0;

    //Continue to read until you reach end of file
    while (line != null)
    {
        //write the line to console window
        Console.WriteLine(line);

        string[] firstCoords = line.Split("~")[0].Split(",");
        string[] secondCoords = line.Split("~")[1].Split(",");
        Vector3 first = new Vector3
        {
            X = int.Parse(firstCoords[0]),
            Y = int.Parse(firstCoords[1]),
            Z = int.Parse(firstCoords[2])
        };
        Vector3 second = new Vector3
        {
            X = int.Parse(secondCoords[0]),
            Y = int.Parse(secondCoords[1]),
            Z = int.Parse(secondCoords[2])
        };

        bricks[brickNum] = new List<Vector3>{first, second};

        int ZLevel = (int)Math.Min(first.Z, second.Z);
        if(maxZLevel<ZLevel){
            maxZLevel=ZLevel;
        }
        if(bricksByZLevel.ContainsKey(ZLevel)){
            bricksByZLevel[ZLevel].Add(brickNum);
        }
        else{
            bricksByZLevel[ZLevel] = new List<int>{brickNum};
        }

        int XLevel = (int)Math.Min(first.X, second.X);
        if(maxXLevel<XLevel){
            maxXLevel=XLevel;
        }

        int YLevel = (int)Math.Min(first.Y, second.Y);
        if(maxYLevel<YLevel){
            maxYLevel=YLevel;
        }

        //Read the next line
        line = sr.ReadLine();
        brickNum++;
    }

    //close the file
    sr.Close();
    
    //PART 1

    /*
    //DEBUG LOG
    Console.WriteLine();
    for (int level=0; level<=maxZLevel; level++){
        if(bricksByZLevel.ContainsKey(level)){
            foreach (int brickNumber in bricksByZLevel[level]){
                List<Vector3> brick = bricks[brickNumber];
                Console.WriteLine("Brick {0} go from {1},{2},{3} to {4},{5},{6}",brickNumber,brick[0].X,brick[0].Y,brick[0].Z,brick[1].X,brick[1].Y,brick[1].Z);
            }
        }
    }
    */

    //Go by croissant level, and make it lose a level as long as it can, then place it, populate bricksSupportingABrick, bricksSupportedByBrick and zLevelBlocked and move to next.
    //When finished we count the number of bricks that supports no bricks (bricksSupportedByBrick[abricknum].Count==0) or that for each bricks supported, the list of supporting bricks is more than 1 (bricksSupportingABrick[supportedBrickNum].Count>1)

    
    //DrawBricks();
    //DrawBricks(false);
    //Console.WriteLine();
    for (int level=2; level<=maxZLevel; level++){
        if(bricksByZLevel.ContainsKey(level)){
            for (int j = bricksByZLevel[level].Count-1; j>=0; j--){
                int brickNumber = bricksByZLevel[level][j];
                int newLevel = level-1;
                List<int> intersects = IntersectAtLevelWith(newLevel, brickNumber);
                while(newLevel > 0 && intersects.Count==0){
                    newLevel--;
                    intersects = IntersectAtLevelWith(newLevel, brickNumber);
                }
                /*
                foreach(int i in intersects){
                    Console.WriteLine("Brick {0} intersect with brick {1}",brickNumber,i);
                }
                */
                newLevel++;
                if(newLevel!=level){
                    bricks[brickNumber][0] = new Vector3(bricks[brickNumber][0].X,bricks[brickNumber][0].Y,bricks[brickNumber][0].Z-(level-newLevel));
                    bricks[brickNumber][1] = new Vector3(bricks[brickNumber][1].X,bricks[brickNumber][1].Y,bricks[brickNumber][1].Z-(level-newLevel));
                    bricksByZLevel[level].Remove(brickNumber);
                    if(!bricksByZLevel.ContainsKey(newLevel)){
                        bricksByZLevel[newLevel] = new List<int>(); 
                    }
                    bricksByZLevel[newLevel].Add(brickNumber);
                }
                if(intersects.Count!=0){
                    bricksSupportingABrick[brickNumber] = new List<int>();
                    for(int i=0;i<intersects.Count;i++){
                        bricksSupportingABrick[brickNumber].Add(intersects[i]);
                        if(!bricksSupportedByBrick.ContainsKey(intersects[i])){
                            bricksSupportedByBrick[intersects[i]] = new List<int>();
                        }
                        bricksSupportedByBrick[intersects[i]].Add(brickNumber);
                    }
                }
                //Add brick to list to check from this point
                if(bricks[brickNumber][0].Z-bricks[brickNumber][1].Z!=0){
                    zLevelBlocking.Add(brickNumber);
                }
            }
        }
    }

    //Console.WriteLine();
    foreach(var brick in bricks){
        if(!bricksSupportedByBrick.ContainsKey(brick.Key) || bricksSupportedByBrick[brick.Key].Count==0){
            result++;
            //Console.WriteLine("Brick {0} does not support any other brick and can be removed",brick.Key);
        }
        else{
            bool isOkToRemove = true;
            foreach(int supportedBrickNum in bricksSupportedByBrick[brick.Key]){
                //Console.WriteLine("Brick {0} supports brick {1}",brick.Key,supportedBrickNum);
                if(bricksSupportingABrick[supportedBrickNum].Count==1){
                    //Console.WriteLine("Which is not supported by any other bricks unfortunately");
                    isOkToRemove = false;
                    break;
                }
            }
            if(isOkToRemove){
                //Console.WriteLine("Brick {0} can be removed",brick.Key);
                result++;
            }
        }
    }

    //DrawBricks();
    //DrawBricks(false);

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