using System.Drawing;

string line;
int result1 = 1;
List<string> map = new List<string>();
Point start = new Point(0, 0);
Point point1 = new Point(-1, -1);
Point prevPoint1;
Point point2 = new Point(-1, -1);
Point prevPoint2;
List<Point> chain = new List<Point>();
bool startFound = false;

//For part 2
int result2 = 0;
bool lookLeft = false; //CHANGE THIS VARIABLE TO LOOK AT LEFT SIDE OR RIGHT SIDE TILES WHILE MOVING ALONG THE CHAIN
bool outside = false;
List<Point> seenTiles = new List<Point>();
Point lookOffSet = new Point(0, 0);
Point prevPointChain = new Point(0,0);

void Move(ref Point current, ref Point prev)
{
    char c = map[current.Y][current.X];
    int xChange = 0;
    int yChange = 0;
    switch (c){
        case '|':
            if (prev.Y>current.Y){
                yChange=-1;
            }
            else{
                yChange=1;
            }
            break;
        case '-':
            if (prev.X>current.X){
                xChange=-1;
            }
            else{
                xChange=1;
            }
            break;
        case 'L':
            if (prev.Y<current.Y){
                xChange=1;
            }
            else{
                yChange=-1;
            }
            break;
        case 'J':
            if (prev.Y<current.Y){
                xChange=-1;
            }
            else{
                yChange=-1;
            }
            break;
        case '7':
            if (prev.Y>current.Y){
                xChange=-1;
            }
            else{
                yChange=1;
            }
            break;
        case 'F':
            if (prev.Y>current.Y){
                xChange=1;
            }
            else{
                yChange=1;
            }
            break;
        default:
            break;
    }
    prev = current;
    current.X+=xChange;
    current.Y+=yChange;
}

void LookAtTile(Point pos){
    if(!seenTiles.Contains(pos) && !chain.Contains(pos)){
        seenTiles.Add(pos);
        if(pos.X==0 || pos.X==map[0].Length-1 || pos.Y==0 || pos.Y==map.Count-1){
            outside = true;
        }
        LookAtTile(new Point(Math.Max(pos.X-1,0),pos.Y));
        LookAtTile(new Point(Math.Min(pos.X+1,map[0].Length-1),pos.Y));
        LookAtTile(new Point(pos.X,Math.Max(pos.Y-1,0)));
        LookAtTile(new Point(pos.X,Math.Min(pos.Y+1,map.Count-1)));
    }
}

bool UpdateOffset(ref Point offSet, Point pos, Point prevPos)
{
    if(map[pos.Y][pos.X] == 'L'){
        if(prevPos.X>pos.X){
            offSet.X = lookLeft? -1:1;
            offSet.Y = 0;
            return lookLeft;
        }
        else{
            offSet.X = 0;
            offSet.Y = lookLeft? -1:1;
            return !lookLeft;
        }
    }
    else if(map[pos.Y][pos.X] == 'J'){
        if(prevPos.X<pos.X){
            offSet.X = lookLeft? -1:1;
            offSet.Y = 0;
            return !lookLeft;
        }
        else{
            offSet.X = 0;
            offSet.Y = lookLeft? 1:-1;
            return lookLeft;
        }
    }
    else if(map[pos.Y][pos.X] == '7'){
        if(prevPos.X<pos.X){
            offSet.X = lookLeft? 1:-1;
            offSet.Y = 0;
            return lookLeft;
        }
        else{
            offSet.X = 0;
            offSet.Y = lookLeft? 1:-1;
            return !lookLeft;
        }
    }
    else if(map[pos.Y][pos.X] == 'F'){
        if(prevPos.X>pos.X){
            offSet.X = lookLeft? 1:-1;
            offSet.Y = 0;
            return !lookLeft;
        }
        else{
            offSet.X = 0;
            offSet.Y = lookLeft? -1:1;
            return lookLeft;
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

        map.Add(line);

        if(!startFound){
            for(int i=0;i<line.Length;i++){
                if (line[i] == 'S'){
                    start.X=i;
                    startFound = true;
                }
            }
        }

        if(!startFound){
            start.Y++;
        }

        //Read the next line
        line = sr.ReadLine();
    }

    //close the file
    sr.Close();

    //Find the two directions to go through for the loop
    Console.WriteLine("At pos {0},{1} we have {2}",start.X,start.Y,map[start.Y][start.X]);
    for(int i=0;i<2;i++){
        for(int j=0;j<2;j++){
            int offSet = (int)Math.Pow(-1,j);
            if(i%2==0){
                if(start.X+offSet>=0 && start.X+offSet<map[start.Y].Length){
                    char c = map[start.Y][start.X+offSet];
                    if(c=='-' || (offSet<0 && (c=='L' || c=='F')) || (offSet>0 && (c=='J' || c=='7'))){
                        Console.WriteLine("Can move at pos {0},{1} by {2}",start.X+offSet,start.Y,map[start.Y][start.X+offSet]);
                        if(point1.X<0){
                            point1.X = start.X+offSet;
                            point1.Y = start.Y;

                            //For part 2
                            lookOffSet.Y = lookLeft?-offSet:offSet;
                        }
                        else{
                            point2.X = start.X+offSet;
                            point2.Y = start.Y;
                        }
                        
                    }
                }
            }
            else{
                if(start.Y+offSet>=0 && start.Y+offSet<map.Count){
                    char c = map[start.Y+offSet][start.X];
                    if(c=='|' || (offSet>0 && (c=='L' || c=='J')) || (offSet<0 && (c=='F' || c=='7'))){
                        Console.WriteLine("Can move at pos {0},{1} by {2}",start.X,start.Y+offSet,map[start.Y+offSet][start.X]);
                        if(point1.X<0){
                            point1.X = start.X;
                            point1.Y = start.Y+offSet;

                            //For part 2
                            lookOffSet.X = lookLeft?offSet:-offSet;
                        }
                        else{
                            point2.X = start.X;
                            point2.Y = start.Y+offSet;
                        }
                    }
                }
            }
            
        }
    }

    /*
    //PREVIOUS PART 1 CODE
    //Navigate through the loop until we arrive at the same cell, or cross each other (meaning we get the farthest point or points) or both ways leads to a dead-end, but we know it is a loop so that won't happen.
    prevPoint1 = start;
    prevPoint2 = start;
    while((point1!=point2) && (point1!=prevPoint2) && result1<(map.Count*map[0].Length)/2){
        result1++;
        //Console.WriteLine("Following point 2 at pos {0},{1} where we have {2}",X2,Y2,map[Y2][X2]);
        //Console.Read();
        Move(ref point1,ref prevPoint1);
        Move(ref point2,ref prevPoint2);
    }
    //Console.WriteLine("We end up with point 1 at pos {0},{1} where we have {2}",X1,Y1,map[Y1][X1]);
    //Console.WriteLine("We end up with point 2 at pos {0},{1} where we have {2}",X2,Y2,map[Y2][X2]);
    Console.WriteLine("End of input. Result game 1 found: {0}",result1);
    */
    

    //To make part 2 easier, we will go through the loop once completly and filling a list of cells from this. Also I changed everything to work with Point structure, wich is better for readability
    prevPoint1 = start;
    chain.Add(start);
    while(point1!=start){
        chain.Add(point1);
        Move(ref point1,ref prevPoint1);
    }

    result1 = (int)Math.Ceiling(chain.Count/2f);
    Console.WriteLine("End of input. Result game 1 found: {0}",result1);

    //For part 2, we can look at always the same "side" of the chain (left or right) and each time we cross an unknown tile, we added to our known ones and check if any neighbours tiles are in this position, 
    //repeat the process until we finish the chain loop and take the number of tile we got if we didn't touch the border or the remaining tiles else (nbTotal - chain - checkedTiles).
    
    foreach(Point p in chain){
        LookAtTile(new Point(Math.Max(Math.Min(p.X + lookOffSet.X,map[0].Length-1),0),Math.Max(Math.Min(p.Y + lookOffSet.Y,map.Count-1),0)));
        if(UpdateOffset(ref lookOffSet,p,prevPointChain)){
            LookAtTile(new Point(Math.Max(Math.Min(p.X + lookOffSet.X,map[0].Length-1),0),Math.Max(Math.Min(p.Y + lookOffSet.Y,map.Count-1),0)));
        }   
        prevPointChain = p;
    }
    LookAtTile(new Point(chain[0].X + lookOffSet.X,chain[0].Y + lookOffSet.Y));

    if(outside){
        //For some reason, the calcul when lookLeft=false is false. I will try to look into it but for now THIS PROGRAM WORKS ONLY WHEN "lookLeft=true".
        //(POST FIX EDIT: it was because I didn't checked that the point I was checking was inbound while calling "LookAtTile" from outside the function itself... Found by checking on the smaller example given with the rules)
        Console.WriteLine("We looked at outside tiles. Total:{0}, Seen:{1}, Chain:{2}",map[0].Length*map.Count,seenTiles.Count,chain.Count);
        result2 = map[0].Length*map.Count - seenTiles.Count - chain.Count;
    }
    else{
        Console.WriteLine("We looked at inside tiles. Total:{0}, Seen:{1}, Chain:{2}",map[0].Length*map.Count,seenTiles.Count,chain.Count);
        result2 = seenTiles.Count;
    }

    Console.WriteLine("End of input. Result game 2 found: {0}",result2);
    
    /*
    //DEBUG LOGS AND OUTPUT
    List<string> outMap = new List<string>();
    char seenTile = 'I';
    char otherTile = 'O';
    if(outside){
        seenTile = 'O';
        otherTile = 'I';
    }

    for(int i=0; i<map.Count; i++){
        string mapLine = map[i];
        string outLine = "";
        for(int j=0; j<mapLine.Length;j++){
            Point pos = new Point(j,i);
            if(chain.Contains(pos)){
                outLine += 'C';
            }
            else if(seenTiles.Contains(pos)){
                outLine+=seenTile;
            }
            else{
                outLine+=otherTile;
            }
        }
        outMap.Add(outLine);
    }

    // Append text to an existing file named "Output.txt".
    using (StreamWriter outputFile = new StreamWriter(Directory.GetCurrentDirectory()+"\\..\\..\\..\\Output.txt", true))
    {
        foreach (string outLine in outMap){
            outputFile.WriteLine(outLine);
        }
    }
    */
}
catch(Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}
finally
{
    Console.WriteLine("END");
}