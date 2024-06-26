﻿using System.IO.Compression;
using System.Windows.Markup;

string line;
double result = 0;
List<Tuple<double,double,double>> positions = new List<Tuple<double,double,double>>();
List<Tuple<double,double,double>> speeds = new List<Tuple<double,double,double>>();
double minPos = 0;
double maxPos = 0;


bool IntersectInBound2D(Tuple<double,double,double> pA, Tuple<double,double,double> vA, Tuple<double,double,double> pB, Tuple<double,double,double> vB, double minP, double maxP){
    
    double t1 = 0;
    double t2 = 0;

    if((vB.Item2*vA.Item1)-(vB.Item1*vA.Item2)==0 && (vB.Item3*vA.Item2)-(vB.Item2*vA.Item3)==0 && (vB.Item1*vA.Item3)-(vB.Item3*vA.Item1)==0){

        Console.WriteLine("Calcul: {0}+{1}+{2}-{3}-{4}-{5}={6}",vB.Item2*vA.Item1,vB.Item3*vA.Item2,vB.Item1*vA.Item3,vB.Item1*vA.Item2,vB.Item2*vA.Item3,vB.Item3*vA.Item1,0);
        Console.WriteLine("Parallel vectors in: ({0};{1},{2}) | ({3};{4},{5}) and ({6};{7},{8}) | ({9};{10},{11})",pA.Item1,pA.Item2,pA.Item3,pB.Item1,pB.Item2,pB.Item3,vA.Item1,vA.Item2,vA.Item3,vB.Item1,vB.Item2,vB.Item3);
        Console.WriteLine("Parallel in space");
    }


    if((vB.Item2*vA.Item1)-(vB.Item1*vA.Item2)==0){
        //Colinear vectors, parallel paths, we need to check if they can intersect
        //xA = xVB*t2+xB && yA = yVB*t2+yB && t2>=0 && vice versa
        /*
        if(vA.Item1!=0){
            t2 = (pB.Item1-pA.Item1)/vA.Item1;
        }
        else{
            t2 = (pB.Item2-pA.Item2)/vA.Item2;
        }
        if(vB.Item1!=0){
            t1 = (pA.Item1-pB.Item1)/vB.Item1;
        }
        else{
            t1 = (pA.Item2-pB.Item2)/vB.Item2;
        }
        */

        //It seems like no 2D parallel paths intersects... however, technically, it could. So I leave the above code (not finished, but it works with the following simplification)
        return false;
    }
    else{
        if(vA.Item1!=0){
            t2 = (vA.Item1/(vA.Item1*vB.Item2-vA.Item2*vB.Item1))*((vA.Item2/vA.Item1)*(pB.Item1-pA.Item1)+pA.Item2-pB.Item2);
        }
        else{
            t2 = (vA.Item2/(vA.Item2*vB.Item1-vA.Item1*vB.Item2))*((vA.Item1/vA.Item2)*(pB.Item2-pA.Item2)+pA.Item1-pB.Item1);
        }
        if(vB.Item1!=0){
            t1 = (vB.Item1/(vB.Item1*vA.Item2-vB.Item2*vA.Item1))*((vB.Item2/vB.Item1)*(pA.Item1-pB.Item1)+pB.Item2-pA.Item2);
        }
        else{
            t1 = (vB.Item2/(vB.Item2*vA.Item1-vB.Item1*vA.Item2))*((vB.Item1/vB.Item2)*(pA.Item2-pB.Item2)+pB.Item1-pA.Item1);
        }
    }

    if(t1<0 || t2 <0){
        //Console.WriteLine("We have for: ({0};{1}) | ({2};{3}) and ({4};{5}) | ({6};{7})",pA.Item1,pA.Item2,pB.Item1,pB.Item2,vA.Item1,vA.Item2,vB.Item1,vB.Item2);
        //Console.WriteLine("They cross path in the past...");
        return false;
    }
    else{
        double newPointX;
        double newPointY;
        //double newPointZ;

        if(t1!=0){
            newPointX = vA.Item1 * t1 + pA.Item1;
            newPointY = vA.Item2 * t1 + pA.Item2;
            //newPointZ = zVA * t1 + zPA;

            //Console.WriteLine("We have for: ({0};{1}) | ({2};{3}) and ({4};{5}) | ({6};{7})",pA.Item1,pA.Item2,pB.Item1,pB.Item2,vA.Item1,vA.Item2,vB.Item1,vB.Item2);
            //Console.WriteLine("At t={0}, (newX;newY)=({1};{2})",t1,newPointX,newPointY);
        }
        else if(t2!=0){
            newPointX = vB.Item1 * t2 + pB.Item1;
            newPointY = vB.Item2 * t2 + pB.Item2;
            //newPointZ = zVB * t2 + zPB;

            //Console.WriteLine("We have for: ({0};{1}) | ({2};{3}) and ({4};{5}) | ({6};{7})",pA.Item1,pA.Item2,pB.Item1,pB.Item2,vA.Item1,vA.Item2,vB.Item1,vB.Item2);
            //Console.WriteLine("At t={0}, (newX;newY)=({1};{2})",t2,newPointX,newPointY);
        }
        else{
            //2 points have to be equals
            return pA.Item1==pB.Item1 && pA.Item1>=minP && pA.Item1<=maxP && pA.Item2==pB.Item2 && pA.Item2>=minP && pA.Item2<=maxP;
        }

        return newPointX>=minP && newPointX<=maxP && newPointY>=minP && newPointY<=maxP;
    }

}


try
{
    //To get the time of code execution!
    var watch = System.Diagnostics.Stopwatch.StartNew();

    bool test = false;

    //Pass the file path and file name to the StreamReader constructor
    StreamReader sr;
    if(test){
        sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\TestInput.txt");
    }
    else{
        sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\Input.txt");
    }
    
    

    //Read the first line of text
    line = sr.ReadLine();

    //Continue to read until you reach end of file
    while (line != null)
    {
        //write the line to console window
        Console.WriteLine(line);
        string[] posValues = line.Split("@")[0].Trim().Split(", ");
        string[] velValues = line.Split("@")[1].Trim().Split(", ");
        positions.Add(Tuple.Create(double.Parse(posValues[0]), double.Parse(posValues[1]), double.Parse(posValues[2])));
        speeds.Add(Tuple.Create(double.Parse(velValues[0]), double.Parse(velValues[1]), double.Parse(velValues[2])));

        //Read the next line
        line = sr.ReadLine();
    }

    //close the file
    sr.Close();

    //DEBUG LOGS
    //Console.WriteLine();
    //Console.WriteLine("We have {0} positions and {1} speeds",positions.Count,speeds.Count);
    for(int i = 0; i < positions.Count; i++){
        //Console.WriteLine("Point at ({0},{1},{2}) is goind at speed ({3},{4},{5})",positions[i].Item1,positions[i].Item2,positions[i].Item3,speeds[i].Item1,speeds[i].Item2,speeds[i].Item3);
    }

    //PART 1
    if(test){
        minPos = 7;
        maxPos = 27;
    }
    else{
        minPos = 200000000000000;
        maxPos = 400000000000000;
    }
    
    //Console.WriteLine();
    for(int i = 0; i <positions.Count; i++){
        for(int j = i+1; j<positions.Count; j++){
            if(IntersectInBound2D(positions[i],speeds[i],positions[j],speeds[j],minPos,maxPos)){
                //Console.WriteLine("Intersect in bound!");
                result++;
            }
            else{
                //Console.WriteLine("Does not intersect in bound!");
            }
        }
    }
    Console.WriteLine();
    Console.WriteLine("End of input. Result game 1 found: {0}",result);

    //PART 2
    //I went full code at first, try to find my way with an incremental or recursive way to search and validate a solution when in fact it is a super mathematical problem.
    //A good way would be to solve a system of equations that would gives us the solution:
    //We know that our rock will have a line that will collide all others so it validates the equation:
    //(Velocity_rock * time + StartPos_rock = Velocity_hail * time + StartPos_hail)
    //VXr*t + Xr = VXh*t + Xh
    //VYr*t + Yr = VYh*t + Yh
    //VZr*t + Zr = VZh*t + Zh
    //So we have 3 equations and 7 unknown variables (VXr, Vyr, VZr, Xr, Yr, Zr, t)
    //But if we do this for 3 different hails, we have 9 equations for 9 varaibles !


    //In exemple we have to solve the following system: 
    //ar+x=-2r+19
    //br+y=1r+13
    //cr+z=-2r+30
    //as+x=-1s+18
    //bs+y=-1s+19
    //cs+z=-2s+22
    //at+x=-2t+20
    //bt+y=-2t+25
    //ct+z=-4t+34



    Console.WriteLine();
    Console.WriteLine("End of input. Result game 2 found by following the same method as Simon Toth you can find here : https://medium.com/@simontoth/daily-bit-e-of-c-advent-of-code-day-24-3faeef93c982");

    
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