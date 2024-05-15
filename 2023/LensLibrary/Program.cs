
string line;
double result1 = 0;
string[] steps = {};


//For part 2
double result2 = 0;
Dictionary<int,List<string>> boxes = new Dictionary<int,List<string>>();
Dictionary<string,int> focals = new Dictionary<string,int>();


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

        steps = line.Split(',');

        //Read the next line
        line = sr.ReadLine();
    }

    //close the file
    sr.Close();

    //Console.WriteLine("");
    foreach (string step in steps){
        int value = 0;
        foreach(char c in step){
            value += c;
            value = (value*17)%256;
        }
        //Console.WriteLine("Step: {0} has a value of {1}",step,value);
        result1 += value;
    }

    Console.WriteLine("");
    Console.WriteLine("End of input. Result game 1 found: {0}",result1);

    //Console.WriteLine("");
    foreach (string step in steps){
        int value = 0;
        string label;
        bool remove = false;
        int focalValue = -1;
        if(step.Contains("=")){
            label = step.Split("=")[0];
            focalValue = Int32.Parse(step.Split("=")[1]);
        }
        else{
            label = step.Split("-")[0];
            remove = true;
        }
        foreach(char c in label){
            value += c;
            value = (value*17)%256;
        }
        //Console.WriteLine("Step: {0} has a label value of {1}",step,value);

        if(boxes.ContainsKey(value)){
            List<string> lenses = boxes[value];
            int index = lenses.IndexOf(label);
            if(index < 0 && !remove){
                lenses.Add(label);
                focals[label] = focalValue;
            }
            else if(index>=0){
                if(remove){
                    lenses.RemoveAt(index);
                }
                else{
                    focals[label] = focalValue;
                }
            }
        }
        else{
            List<string> lenses = new List<string>{label};
            boxes[value] = lenses;
            focals[label] = focalValue;
        }

    }

    /*
    //DEBUG LOGS
    Console.WriteLine("");
    foreach(var box in boxes){
        Console.WriteLine("Box: {0}",box.Key);
        foreach(string label in box.Value){
            Console.WriteLine("[{0} {1}]",label,focals[label]);
        }
        Console.WriteLine("");
    }
    */

    foreach(var box in boxes){
        for(int i=0; i<box.Value.Count; i++){
            string label = box.Value[i];
            result2 += (box.Key+1)*focals[label]*(i+1);
        }
    }

    Console.WriteLine("");
    Console.WriteLine("End of input. Result game 2 found: {0}",result2);
}
catch(Exception e)
{
    Console.WriteLine("Exception: " + e.ToString());
}
finally
{
    Console.WriteLine("END");
}