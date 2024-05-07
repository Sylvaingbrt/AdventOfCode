using System.Drawing;

string line;
double result1 = 0;
List<Point> galaxies = new List<Point>();
List<string> map = new List<string>();

//For part 2
double result2 = 0;
List<List<int>> weightMap = new List<List<int>>();
int weightEmpty = 500000; //Previous part made us double each empty column, since we now want to duplicate empty space by 1 million, we will put the weight of one empty column to 500000 wich doubled will give us the target space.
List<List<Point>> coordMap = new List<List<Point>>();

List<string> Transpose(List<string> matrix){
    List<string> transposedMatrix = new List<string>();
    for(int i = 0; i < matrix[0].Length; i++){
        string transposedLine = "";
        for(int j = 0; j < matrix.Count; j++){
            transposedLine+=matrix[j][i];
        }
        transposedMatrix.Add(transposedLine);
    }
    return transposedMatrix;
}

List<List<int>> TransposeInt(List<List<int>> matrix){
    List<List<int>> transposedMatrix = new List<List<int>>();
    for(int i = 0; i < matrix[0].Count; i++){
        List<int> transposedLine = new List<int>();
        for(int j = 0; j < matrix.Count; j++){
            transposedLine.Add(matrix[j][i]);
        }
        transposedMatrix.Add(transposedLine);
    }
    return transposedMatrix;
}

int Distance(Point A, Point B){
    return Math.Abs(A.X - B.X) + Math.Abs(A.Y - B.Y);
}

List<int> MakeWeightList(int weight, int size){
    List<int> weightList = new List<int>();
    for(int i = 0; i<size; i++){
        weightList.Add(weight);
    }
    return weightList;
}

try
{
    //Pass the file path and file name to the StreamReader constructor
    StreamReader sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\Input.txt");

    //Read the first line of text
    line = sr.ReadLine();

    int yIndex = 0;

    //Continue to read until you reach end of file
    while (line != null)
    {
        //write the line to console window
        Console.WriteLine(line);

        map.Add(line);

        
        if(!line.Contains("#")){
            //double empty lines
            map.Add(line);
            weightMap.Add(MakeWeightList(weightEmpty,line.Length));
            weightMap.Add(MakeWeightList(weightEmpty,line.Length));
        }
        else{
            weightMap.Add(MakeWeightList(1,line.Length));
        }
        
        
        

        //Read the next line
        line = sr.ReadLine();
        yIndex++;
    }
    

    //close the file
    sr.Close();

    Console.WriteLine("");

    //Transpose the map and double again empty line
    map = Transpose(map);
    List<string> tempoMap = new List<string>();

    weightMap = TransposeInt(weightMap);
    List<List<int>> tempoWeightMap = new List<List<int>>();

    for(int i=0;i<map.Count;i++){
        string mapLine = map[i];
        tempoMap.Add(mapLine);
        if(!mapLine.Contains("#")){
            tempoMap.Add(mapLine);
            tempoWeightMap.Add(MakeWeightList(weightEmpty,mapLine.Length));
            tempoWeightMap.Add(MakeWeightList(weightEmpty,mapLine.Length));
        }
        else{
            tempoWeightMap.Add(weightMap[i]);
        }
    }

    //For confort purpose, we transpose the map again but it is not necessary
    map = Transpose(tempoMap);
    weightMap = TransposeInt(tempoWeightMap);

    //Get all galaxies coordinates
    for(int i=0; i<map.Count; i++){
        for(int j=0; j<map[i].Length;j++){
            if (map[i][j]=='#'){
                galaxies.Add(new Point(j,i));
            }
        }
    }

    //An A* algorithm (or Dijkstra) is tempting but would be wayyyy overkill for part 1. Since we can only move horizontally and vertically, without any obstacles, the "shortest" path is the absolute difference between the coordinates of galaxies.
    for(int i=0; i<galaxies.Count; i++){
        for(int j=i+1; j<galaxies.Count; j++){
            result1 += Distance(galaxies[i],galaxies[j]);
        }
    }

    Console.WriteLine("End of input. Result game 1 found: {0}",result1);
    
    //Part 2 only
    //Set coordinates from our weight map
    int X;
    int Y;
    for(int i=0; i<weightMap.Count; i++){
        List<Point> linePoints = new List<Point>();
        for(int j=0; j<weightMap[i].Count;j++){
            if(i>0){
                Y = weightMap[i-1][j] + coordMap[i-1][j].Y;
            }
            else{
                Y = 0;
            }
            if(j>0){
                X = weightMap[i][j-1] + linePoints[j-1].X;
            }
            else{
                X = 0;
            }
            linePoints.Add(new Point(X,Y));
        }
        coordMap.Add(linePoints);
    }

    //Console.WriteLine("Sizes Map: ({0},{1}), weightMap: ({2},{3}), coordMap: ({4},{5})",map.Count,map[0].Length,weightMap.Count,weightMap[0].Count,coordMap.Count,coordMap[0].Count);

    //Get all galaxies coordinates from new map
    galaxies.Clear();
    for(int i=0; i<map.Count; i++){
        for(int j=0; j<map[i].Length;j++){
            if (map[i][j]=='#'){
                galaxies.Add(coordMap[i][j]);
            }
        }
    }
    //Get result
    for(int i=0; i<galaxies.Count; i++){
        for(int j=i+1; j<galaxies.Count; j++){
            result2 += Distance(galaxies[i],galaxies[j]);
        }
    }

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