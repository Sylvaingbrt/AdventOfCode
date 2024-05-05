string line;
double result1 = 0;
Dictionary<string, int> hands = new Dictionary<string, int>();
Dictionary<char, int> cards =  new Dictionary<char, int>(){
    {'2',1},
    {'3',2},
    {'4',3},
    {'5',4},
    {'6',5},
    {'7',6},
    {'8',7},
    {'9',8},
    {'T',9},
    {'J',10},
    {'Q',11},
    {'K',12},
    {'A',13}
};
string hand;
List<string> orderedHands = new List<string>();

//For part 2
double result2 = 0;
Dictionary<char, int> cards2 =  new Dictionary<char, int>(){
    {'J',0},
    {'2',1},
    {'3',2},
    {'4',3},
    {'5',4},
    {'6',5},
    {'7',6},
    {'8',7},
    {'9',8},
    {'T',9},
    {'Q',11},
    {'K',12},
    {'A',13}
};
List<string> orderedHands2 = new List<string>();


bool CardIsBetter(char c1, char c2, bool usingJoker)
{
    if(usingJoker){
        return cards2[c1]>cards2[c2];
    }
    return cards[c1]>cards[c2];
}

int HandScore(string handToCompare,bool usingJokers, bool debugLog = false)
{
    //usingJokers is a rule for part2 only
    int jokerBoost = 0;

    Dictionary<char, int> cardsOccurence =  new Dictionary<char, int>();
    foreach (char c in handToCompare){
        if(usingJokers && c=='J')
        {
            jokerBoost++;
        }
        else{
            if(cardsOccurence.ContainsKey(c)){
                cardsOccurence[c]++;
            }
            else{
                cardsOccurence[c] = 1;
            }
        }
    }

    //Use joker boost to hand
    if(cardsOccurence.Count==0){
        cardsOccurence['J'] = jokerBoost;
    }
    else if(jokerBoost>0){
        foreach(var occurence in cardsOccurence){
            cardsOccurence[occurence.Key] = occurence.Value+jokerBoost;
        }
    }
    //End of joker rule changes

    int nbDiffrentCards = cardsOccurence.Count;
    if(nbDiffrentCards == 1){
        //Five of a kind
        if(debugLog){
            Console.WriteLine("Five of a kind");
        }
        return 6;
    }
    else if(nbDiffrentCards == 2){
        //Four of a kind or full house
        if(cardsOccurence.Values.Contains(4)){
            //Four of a kind
            if(debugLog){
                Console.WriteLine("Four of a kind");
            }
            return 5;
        }
        else{
            if(debugLog){
                Console.WriteLine("Full house");
            }
            return 4;
        }
    }
    else if (nbDiffrentCards == 3){
        //Three of a kind or two pairs
        if(cardsOccurence.Values.Contains(3)){
            //Three of a kind
            if(debugLog){
                Console.WriteLine("Three of a kind");
            }
            return 3;
        }
        else{
            if(debugLog){
                Console.WriteLine("Two pairs");
            }
            return 2;
        }
    }
    else if (nbDiffrentCards == 4){
        //One pair
        if(debugLog){
            Console.WriteLine("One pair");
        }
        return 1;
    }
    else if (nbDiffrentCards == 5){
        //high card
        if(debugLog){
            Console.WriteLine("High card");
        }
        return 0;
    }
    else{
        //Wait... something is wrong
        Console.WriteLine("Something went wrong... there are {0} different cards. That should not be.",nbDiffrentCards);
        return -1;
    }
    
}

bool BetterHandValue(string handToCompare, string otherHand,bool usingJoker)
{
    for (int i = 0;i<Math.Min(handToCompare.Length,otherHand.Length);i++){
        if(handToCompare[i] != otherHand[i]){
            return CardIsBetter(handToCompare[i],otherHand[i],usingJoker);
        }
    }
    return false;
}

bool HandIsStronger(string handToCompare, string otherHand, bool usingJokers = false)
{
    int scoreHandToCompare = HandScore(handToCompare,usingJokers);
    int scoreOtherHand = HandScore(otherHand,usingJokers);
    if(scoreHandToCompare>scoreOtherHand){
        return true;
    }
    else if(scoreHandToCompare==scoreOtherHand){
        return BetterHandValue(handToCompare, otherHand,usingJokers);
    }
    return false;
}

try
{
    //Pass the file path and file name to the StreamReader constructor
    StreamReader sr = new StreamReader(Directory.GetCurrentDirectory()+"\\..\\..\\..\\Input.txt");

    //Get first line
    line = sr.ReadLine();

    //Continue to read until you reach end of file
    while (line != null)
    {
        //write the line to console window
        Console.WriteLine(line);

        //reset variables
        hand = line.Split(" ")[0];
        if(hands.ContainsKey(hand)){
            Console.WriteLine("Duplicate hand: {0}",hand);
        }
        else{
            hands[hand] = Int32.Parse(line.Split(" ")[1]);
            
            //Order hands by strenght
            bool insertedHands = false;
            for(int i=0; i<orderedHands.Count; i++){
                if(HandIsStronger(orderedHands[i],hand)){
                    orderedHands.Insert(i, hand);
                    insertedHands = true;
                    break;
                }
            }
            if(!insertedHands){
                orderedHands.Add(hand);
            }


            //Make the same but with part 2 rules
            insertedHands = false;
            HandScore(hand,true,true);
            for(int i=0; i<orderedHands2.Count; i++){
                if(HandIsStronger(orderedHands2[i],hand,true)){
                    orderedHands2.Insert(i, hand);
                    insertedHands = true;
                    break;
                }
            }
            if(!insertedHands){
                orderedHands2.Add(hand);
            }
        }
        

        //Read the next line
        line = sr.ReadLine();
    }

    //Caluclate the result from our ordered list
    for(int i=0; i<orderedHands.Count; i++){
        result1 += (i+1)*hands[orderedHands[i]];
    }

    for(int i=0; i<orderedHands2.Count; i++){
        result2 += (i+1)*hands[orderedHands2[i]];
    }
    

    Console.WriteLine("End of input. Result game 1 found: {0}",result1);
    Console.WriteLine("End of input. Result game 1 found: {0}",result2);

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

