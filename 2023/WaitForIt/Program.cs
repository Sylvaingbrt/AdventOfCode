//This exercice is the résolution of an 2nd degree equation.
//Let's say we have T the total allowed time
//T = t1+t2 with t1 the time used to press the boat button and t2 the time to wake it move.
//We get V the velocity of the boat by V = t1*1 (gain 1mm.s-1 each milliseconde) and D=t2*V
//We have then D=t2*t1 and t2=T-t1. Wich get us D=Tt1-t1². by fixing D the score we want to beat, we finally want to resolve -t1²+Tt1-D>0.
//Since it's a 2nd degree equation with a negative factor we know that possible whole value for t1 will be all value between "ta" and "tb" the two possible solutions for -t1²+Tt1-D=0.
//We will need to be careful about negative values and equations without solutions but that would not have any physical sense in the contexte, so the checks will be there only for security
//Moreover by looking at our equation and its derivative for t1=0, we understand that we can only have negative solution if D or T are negative.

string line;
double result1 = 1;
List<int> times = new List<int>();
List<int> distances = new List<int>();

//For part 2
double time;
double distance;
double result2 = 0;

List<int> GetNumbersFromString(string nbString){
    List<int> result = new List<int>();
    string[] toConvert = nbString.Split(" ");
    foreach(string s in toConvert){
        if(s.Trim().Length > 0){
            result.Add(Int32.Parse(s.Trim()));
        }
        
    }
    return result;
}


try
{
    //Pass the file path and file name to the StreamReader constructor
    StreamReader sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\Input.txt");

    //Get times
    line = sr.ReadLine();
    Console.WriteLine(line);
    times = GetNumbersFromString(line.Split(":")[1]);
    time = double.Parse(line.Split(":")[1].Replace(" ",""));

    //Get distances
    line = sr.ReadLine();
    Console.WriteLine(line);
    distances = GetNumbersFromString(line.Split(":")[1]);
    distance = double.Parse(line.Split(":")[1].Replace(" ",""));
    

    for(int i = 0; i<times.Count; i++){
        int delta = (times[i]*times[i])-4*distances[i];
        if( delta> 0){
            double ta = Math.Floor((times[i]-Math.Sqrt(delta))/2f);
            double tb = Math.Floor((times[i]+Math.Sqrt(delta))/2f);
            if(ta>0 && tb>0){
                result1 *= tb-ta;
            }
            else{
                Console.WriteLine("Negative answer possible, does not make sense except if time or distance negative (or an error). time:{0} distance: {1}",times[i],distances[i]);
            }
        }
        else{
            Console.WriteLine("Run that cannot be beaten, time:{0} distance: {1}",times[i],distances[i]);
        }
    }

    double delta2 = time*time - 4*distance;
    if( delta2> 0){
        double ta = Math.Floor((time-Math.Sqrt(delta2))/2f);
        double tb = Math.Floor((time+Math.Sqrt(delta2))/2f);
        if(ta>0 && tb>0){
            result2 = tb-ta;
        }
        else{
            Console.WriteLine("Negative answer possible, does not make sense except if time or distance negative (or an error). time:{0} distance: {1}",time,distance);
        }
    }
    else{
        Console.WriteLine("Rigged race, time:{0} distance: {1}",time,distance);
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