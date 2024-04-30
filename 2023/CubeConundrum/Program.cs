// See https://aka.ms/new-console-template for more information
using System.Reflection;

Console.WriteLine("Hello, World!");

String line;
double result1 = 0;
double result2 = 0;
string[] parts;
string[] plays; 
string[] cubes;
int gameNb;
int nbCube;
char colorCube;

int rCube;
int gCube;
int bCube;

int minRCube;
int minGCube;
int minBCube;

bool gameIsValid;

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

        //Reset Variables
        rCube = 12;
        gCube = 13;
        bCube = 14;
        minRCube = 0;
        minGCube = 0;
        minBCube = 0;

        gameIsValid = true;

        //Parse to check
        parts = line.Split(':');
        gameNb = Int32.Parse(parts[0].Split(" ")[1]);
        Console.WriteLine(gameNb);
        plays = parts[1].Split(";");
        foreach (var play in plays)
        {
            cubes = play.Split(',');
            foreach(var cube in cubes){
                nbCube = Int32.Parse(cube.Trim().Split(" ")[0]);
                colorCube = cube.Trim().Split(" ")[1][0];

                gameIsValid = (colorCube=='r' && rCube>=nbCube) || (colorCube=='g' && gCube>=nbCube) || (colorCube=='b' && bCube>=nbCube) || !gameIsValid;

                if(colorCube=='r' && minRCube<nbCube){
                    minRCube=nbCube;
                }
                else if(colorCube=='g' && minGCube<nbCube){
                    minGCube=nbCube;
                }
                else if(colorCube=='b' && minBCube<nbCube){
                    minBCube=nbCube;
                }
            }

            if(!gameIsValid){
                Console.WriteLine("NG: {0}",play);
            }
            else{
                Console.WriteLine("OK: {0}",play);
            }
        }
        result2 += minRCube*minGCube*minBCube;
        if(gameIsValid){
            result1 += gameNb;
            Console.WriteLine("Good one");
        }
        else{
            Console.WriteLine("INVALID GAME");
        }

        Console.WriteLine("--------------------------------");
        Console.WriteLine("");
        Console.WriteLine("");

        //Read the next line
        line = sr.ReadLine();
    }

    Console.WriteLine("End of input. Result game 1 found: {0}",result1);
    Console.WriteLine("End of input. Result game 2 found: {0}",result2);

    //close the file
    sr.Close();
}
catch(Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}
finally
{
    Console.WriteLine("END");
}