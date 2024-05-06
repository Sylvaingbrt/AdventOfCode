
string line;
double result1 = 1;
List<string> map = new List<string>();
int X = 0;
int Y = 0;
int X1 = -1;
int Y1 = -1;
int X2 = -1;
int Y2 = -1;
int prevX1 = -1;
int prevY1 = -1;
int prevX2 = -1;
int prevY2 = -1;

bool startFound = false;

void Move(ref int currentX, ref int currentY, ref int prevX, ref int prevY)
{
    char c = map[currentY][currentX];
    int xChange = 0;
    int yChange = 0;
    switch (c){
        case '|':
            if (prevY>currentY){
                yChange=-1;
            }
            else{
                yChange=1;
            }
            break;
        case '-':
            if (prevX>currentX){
                xChange=-1;
            }
            else{
                xChange=1;
            }
            break;
        case 'L':
            if (prevY<currentY){
                xChange=1;
            }
            else{
                yChange=-1;
            }
            break;
        case 'J':
            if (prevY<currentY){
                xChange=-1;
            }
            else{
                yChange=-1;
            }
            break;
        case '7':
            if (prevY>currentY){
                xChange=-1;
            }
            else{
                yChange=1;
            }
            break;
        case 'F':
            if (prevY>currentY){
                xChange=1;
            }
            else{
                yChange=1;
            }
            break;
        default:
            break;
    }
    prevX = currentX;
    prevY = currentY;
    currentX+=xChange;
    currentY+=yChange;
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
                    X=i;
                    startFound = true;
                }
            }
        }

        if(!startFound){
            Y++;
        }

        //Read the next line
        line = sr.ReadLine();
    }

    //close the file
    sr.Close();

    //Find the two directions to go through for the loop
    Console.WriteLine("At pos {0},{1} we have {2}",X,Y,map[Y][X]);
    for(int i=0;i<2;i++){
        for(int j=0;j<2;j++){
            int offSet = (int)Math.Pow(-1,j);
            if(i%2==0){
                char c = map[Y][X+offSet];
                if(c=='-' || (offSet<0 && (c=='L' || c=='F')) || (offSet>0 && (c=='J' || c=='7'))){
                    Console.WriteLine("Can move at pos {0},{1} by {2}",X+offSet,Y,map[Y][X+offSet]);
                    if(X1<0){
                        X1 = X+offSet;
                        Y1 = Y;
                    }
                    else{
                        X2 = X+offSet;
                        Y2 = Y;
                    }
                    
                }
            }
            else{
                char c = map[Y+offSet][X];
                if(c=='|' || (offSet>0 && (c=='L' || c=='J')) || (offSet<0 && (c=='F' || c=='7'))){
                    Console.WriteLine("Can move at pos {0},{1} by {2}",X,Y+offSet,map[Y+offSet][X]);
                    if(X1<0){
                        X1 = X;
                        Y1 = Y+offSet;
                    }
                    else{
                        X2 = X;
                        Y2 = Y+offSet;
                    }
                }
            }
            
        }
    }

    //Navigate through the loop until we arrive at the same cell, or cross each other (meaning we get the farthest point or points) or both ways leads to a dead-end, but we know it is a loop so that won't happen.
    prevX1 = X;
    prevY1 = Y;
    prevX2 = X;
    prevY2 = Y;
    while((X1!=X2 || Y1!=Y2) && (X1!=prevX2 || Y1!=prevY2) && result1<(map.Count*map[0].Length)/2){
        result1++;
        //Console.WriteLine("Following point 2 at pos {0},{1} where we have {2}",X2,Y2,map[Y2][X2]);
        //Console.Read();
        Move(ref X1,ref Y1,ref prevX1,ref prevY1);
        Move(ref X2,ref Y2,ref prevX2,ref prevY2);
    }
    //Console.WriteLine("We end up with point 1 at pos {0},{1} where we have {2}",X1,Y1,map[Y1][X1]);
    //Console.WriteLine("We end up with point 2 at pos {0},{1} where we have {2}",X2,Y2,map[Y2][X2]);
    Console.WriteLine("End of input. Result game 1 found: {0}",result1);

}
catch(Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}
finally
{
    Console.WriteLine("END");
}

