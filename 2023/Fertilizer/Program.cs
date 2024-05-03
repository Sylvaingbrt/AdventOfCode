string line;
double result1 = -1;
List<double> sources;
List<double> destinations = new List<double>();
List<double> map;


//Variable for part 2
double result2 = -1;
List<List<double>> sourcesRange;
List<List<double>> destinationsRange = new List<List<double>>();



List<double> GetNumbersFromString(string nbString){
    List<double> result = new List<double>();
    string[] toConvert = nbString.Split(" ");
    foreach(string s in toConvert){
        if(s.Trim().Length > 0){
            result.Add(double.Parse(s.Trim()));
        }
        
    }
    return result;
}

List<List<double>> GetRangeFromString(string nbString){
    List<List<double>> result = new List<List<double>>();
    string[] toConvert = nbString.Split(" ");
    int i=0;
    double startR = 0;
    foreach(string s in toConvert){
        if(s.Trim().Length > 0){
            List<double> range = new List<double>();
            if(i%2== 0){
                startR = double.Parse(s.Trim());
                range.Clear();
            }
            else{
                range.Add(startR);
                range.Add(startR+double.Parse(s.Trim())-1);
                result.Add(range);
            }
            i++;
        }
        
    }
    return result;
} 

try
{
    //Pass the file path and file name to the StreamReader constructor
    StreamReader sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\Input.txt");

    //Read the first line of text
    line = sr.ReadLine();

    //write the line to console window
    Console.WriteLine(line);

    //Get source
    sources = GetNumbersFromString(line.Split(":")[1]);
    sourcesRange = GetRangeFromString(line.Split(":")[1]) ;

    /*
    //DEBUG LOGS
    foreach(List<double> ld in sourcesRange){
        Console.WriteLine("[{0},{1}]",ld[0],ld[1]);
    }
    */

    //Start reading all maps
    line = sr.ReadLine();


    //Continue to read until you reach end of file
    while (line != null)
    {
        //write the line to console window
        Console.WriteLine(line);

        //Parse
        if(!line.Contains(":")){
            if(line.Length==0){
                //Start new catégorie
                foreach(double d in destinations){
                    sources.Add(d);
                }
                destinations.Clear();


                foreach(List<double> ld in destinationsRange){
                    sourcesRange.Add(ld);
                    //Possible optimisation would be to remake ranges if some are neighbours
                }
                destinationsRange.Clear();
            }
            else{
                //Execute the maping
                map = GetNumbersFromString(line);
                for(int i = sources.Count-1; i >= 0; i--){
                    if(sources[i]>=map[1] && sources[i]<map[1]+map[2]){
                        destinations.Add(sources[i] - (map[1]- map[0]));
                        sources.RemoveAt(i);
                    }
                }

                for(int i = sourcesRange.Count-1; i >= 0; i--){
                    List<double> rangeSource = sourcesRange[i];
                    if((rangeSource[0]<map[1]+map[2] && rangeSource[0]>=map[1]) || (rangeSource[1]<map[1]+map[2] && rangeSource[1]>=map[1]) || (rangeSource[0]<map[1] && rangeSource[1]>=map[1]+map[2])){
                        //Range intersections, needs to do something
                        sourcesRange.RemoveAt(i);

                        if(rangeSource[0]<map[1]){
                            List<double> outsideRange = new List<double> //If initialization is outside this scope, it creates weird artefact where "rangeSource" clears itself at the same time as "outsideRange" in the second if loop
                            {
                                rangeSource[0],
                                map[1] - 1
                            };
                            rangeSource[0]=map[1];
                            sourcesRange.Add(outsideRange);
                        }

                        if(rangeSource[1]>=map[1]+map[2]){
                            List<double> outsideRange = new List<double>
                            {
                                map[1] + map[2],
                                rangeSource[1]
                            };
                            rangeSource[1]=map[1]+map[2]-1;
                            sourcesRange.Add(outsideRange);
                        }

                        rangeSource[0] = rangeSource[0] - (map[1]- map[0]);
                        rangeSource[1] = rangeSource[1] - (map[1]- map[0]);
                        destinationsRange.Add(rangeSource);
                    }
                }
            }
        }

        //Read the next line
        line = sr.ReadLine();
    }

    //Minimum of the last output list
    foreach(double d in sources){
        if(result1<0 || d<result1){
            result1 = d;
        }
    }
    foreach(double d in destinations){
        if(result1<0 || d<result1){
            result1 = d;
        }
    }


    //Minimum of the start of all ranges
    foreach(List<double> ld in sourcesRange){
        if(result2<0 || ld[0]<result2){
            result2 = ld[0];
        }
    }
    foreach(List<double> ld in destinationsRange){
        if(result2<0 || ld[0]<result2){
            result2 = ld[0];
        }
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