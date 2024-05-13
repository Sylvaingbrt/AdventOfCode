
string line;
double result1 = 0;
List<string> mapString = new List<string>();
List<double> map;

//For part 2
//Part 2 made me struggle quit a bit even though my first impression was the right one. To be fair my first idea was the right one and the implementation was almost good. That is without having the error that I carried even from part 1 but wasn't showing before.
//I let my first code for part 1, even if it would be very simple to change it to match part 2 but I think it is a good reminder of the limits of computational digits. Even though "double" has a large range, its significant digits are not as wide...
//To be clearer: the issue I was struggling with is that for an input like "####..##..#####.#" I got the decimal number "10111110011001111", which was translated to "10111110011001112" in the "GetNbDiffFromDouble" function. Leading obviously to wrong outputs.
double result2 = 0;

List<double> MapValue(List<string> input, bool column = false){

    List<double> output = new List<double>();
    int boundI = column ? input[0].Length : input.Count;
    int boundJ = column ? input.Count : input[0].Length;

    for(int i = 0; i < boundI; i++){
        double mapVal = 0;
        for(int j = 0; j < boundJ; j++){
            char c;
            if(column){
                c = input[j][i];
            }
            else{
                c = input[i][j];
            }
            if(c=='#'){
                mapVal += Math.Pow(10,j);
            }
        }
        output.Add(mapVal);
    }

    return output;
}

int GetNbDiffFromDouble(double v1, double v2, bool debugLog = false)
{
    string s1 = v1.ToString();
    string s2 = v2.ToString();

    if(debugLog){
        Console.WriteLine("Comparing {0} and {1} from {2} and {3}",s1,s1,v1,v2);
    }

    return GetNbDiff(s1,s2,debugLog);
}

int GetNbDiff(string s1, string s2, bool debugLog = false){
    int result = 0;

    if(s1.Length<s2.Length){
        string compl = "";
        for(int i = 0; i<s2.Length-s1.Length;i++){
            compl+='0';
        }
        s1 = compl+s1;
    }
    else if(s2.Length<s1.Length){
        string compl = "";
        for(int i = 0; i<s1.Length-s2.Length;i++){
            compl+='0';
        }
        s2 = compl+s2;
    }

    if(debugLog){
        Console.WriteLine("Comparing "+s1+" and "+s2);
    }
    
    char c1;
    char c2;
    
    for(int i = 0; i < s1.Length; i++){
        c1 = s1[i];
        c2 = s2[i];
        if(c1!=c2){
            //Console.WriteLine("diff at pos {0}", i);
            result += 1;
        }
    }
    
    //Console.WriteLine("");
    return result;
}

void GetFirstMirrorIndex(List<double> input, ref int firstMirrorIndex, bool withSmudge = false){
    int resultIndex = 1;
    int size = 1;
    int testIndex = 0;
    bool found = false;
    int smudgeError = 1;
    if(input.Count>1){
        while(!found && resultIndex<input.Count){
            while((resultIndex+testIndex<input.Count) && (resultIndex-size>=0)){
                if(input[resultIndex+testIndex]==input[resultIndex-size]){
                    found = true;
                }
                else{
                    if(withSmudge){
                        smudgeError-=GetNbDiffFromDouble(input[resultIndex+testIndex],input[resultIndex-size]);
                        if(smudgeError<0){
                            found = false;
                            smudgeError = 1;
                        }
                        else{
                            found = true;
                        }
                    }
                    else{
                        found = false;
                    }
                }
                if(found){
                    size++;
                    testIndex++;
                }
                else{
                    size=1;
                    testIndex=0;
                    resultIndex++;
                }
            }
            if(withSmudge && smudgeError==1){
                //smudge not found, back to search again
                found = false;
                size=1;
                testIndex=0;
                resultIndex++;
            }
        }
    }

    if(found){
        firstMirrorIndex = resultIndex;
    }
    
}

int GetFirstMirrorIndexFromString(List<string> input, bool withSmudge = false){
    int resultIndex = 1;
    int size = 1;
    int testIndex = 0;
    bool found = false;
    int smudgeError = 1;
    if(input.Count>1){
        while(!found && resultIndex<input.Count){
            while((resultIndex+testIndex<input.Count) && (resultIndex-size>=0)){
                if(input[resultIndex+testIndex].Equals(input[resultIndex-size])){
                    found = true;
                }
                else{
                    if(withSmudge){
                        smudgeError-=GetNbDiff(input[resultIndex+testIndex],input[resultIndex-size]);
                        if(smudgeError<0){
                            found = false;
                            smudgeError = 1;
                        }
                        else{
                            found = true;
                        }
                    }
                    else{
                        found = false;
                    }
                }
                if(found){
                    size++;
                    testIndex++;
                }
                else{
                    size=1;
                    testIndex=0;
                    resultIndex++;
                }
            }
            if(withSmudge && smudgeError==1){
                //smudge not found, back to search again
                found = false;
                size=1;
                testIndex=0;
                resultIndex++;
            }
        }
    }

    if(found){
        return resultIndex;
    }

    return 0;
    
}

List<string> Transpose(List<string> input){
    List<string> result = new List<string>();
    for(int i=0;i<input[0].Length; i++){
        string column = "";
        for(int j=0;j<input.Count; j++){
            column+=input[j][i];
        }
        result.Add(column);
    }
    return result;
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

        if(line.Length==0){
            //Part 1:
            int firstIndexMirror = 0;
            map = MapValue(mapString);

            /*
            //DEBUG LOG
            foreach(double val in map){
                Console.WriteLine(val);
            }
            */

            GetFirstMirrorIndex(map, ref firstIndexMirror);
            if(firstIndexMirror==0){
                //Try with columns
                map = MapValue(mapString, true);
                GetFirstMirrorIndex(map, ref firstIndexMirror);
                result1+=firstIndexMirror;
                Console.WriteLine("Mirror for columns at {0}", firstIndexMirror);
            }
            else{
                result1+=100*firstIndexMirror;
                Console.WriteLine("Mirror for rows at {0}", firstIndexMirror);
            }

            if(firstIndexMirror==0){
                Console.WriteLine("ERROR, NO MIRROR FOUND");
            }

            //Part 2:
            firstIndexMirror = GetFirstMirrorIndexFromString(mapString,true);
            if(firstIndexMirror==0){
                //Try with columns
                firstIndexMirror = GetFirstMirrorIndexFromString(Transpose(mapString),true);
                result2+=firstIndexMirror;
                Console.WriteLine("Smudged mirror for columns at {0}", firstIndexMirror);
            }
            else{
                result2+=100*firstIndexMirror;
                Console.WriteLine("Smudged mirror for rows at {0}", firstIndexMirror);
            }
            if(firstIndexMirror==0){
                Console.WriteLine("ERROR, NO MIRROR WITH SMUDGE FOUND");
                //break;
            }


            mapString.Clear();
            Console.WriteLine("");
        }
        else{
            mapString.Add(line);
        }


        //Read the next line
        line = sr.ReadLine();
    }

    //close the file
    sr.Close();

    
    Console.WriteLine("End of input. Result game 1 found: {0}",result1);
    Console.WriteLine("End of input. Result game 2 found: {0}",result2);

    /*
    //LIMITS TESTS LOGS
    Console.WriteLine("");
    Console.WriteLine("Test 1: {0}",GetNbDiff("####..##..#####..", "####..##..#####.#", true));
    Console.WriteLine("Test 2: {0}",GetNbDiffFromDouble(111110011001111, 10111110011001111, true));
    */

}
catch(Exception e)
{
    Console.WriteLine("Exception: " + e.ToString());
}
finally
{
    Console.WriteLine("END");
}