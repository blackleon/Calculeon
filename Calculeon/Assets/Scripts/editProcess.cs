using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class editProcess : MonoBehaviour
{
    public int stoppervalue = 10;//max iterations of for loops w/o condition

    public void add() //add to the process array
    {
        globals.process.Add(gameObject.name); //add the name of button pressed
        globals.updateProcessText(); //update process ui
    }

    public void addWithParanthes() //add to the process array with an open brackets
    {
        globals.process.Add(gameObject.name); //add the name of button pressed
        globals.process.Add("("); //add open brackets
        globals.updateProcessText(); //update process ui
    }

    public void clear() //clear the process
    {
        globals.process.Clear(); //call global clear method
        globals.ans = 0; //set answer to 0
        globals.updateProcessText(); //update process ui
        globals.updateResultText("0"); //update result ui
    }

    public void undo() //undo last input
    {
        if(globals.process.Count > 0) //when process has symbols inside
        {
            globals.process.RemoveAt(globals.process.Count - 1); //remove last one
            globals.updateProcessText(); //update process ui
        }
    }
    
    public void calcuate() //calculate the result
    {
        List<string> result; //result list

        List<string> process = new List<string>(); //process list for recursion
        process.AddRange(globals.process); //add processes to the list

        result = calculateRec(process); //call recursive method

        string resultString = ""; //convert list<string> to string
        foreach (string item in result)
        {
            resultString += item;
        }
        try
        {
            globals.ans = float.Parse(resultString); //try to convert result from string to float
        }catch
        {
            globals.ans = 0; //if fails set answer to 0
        }

        globals.updateResultText(resultString); //update result ui
    }

    private List<string> calculateRec(List<string> process) //recursive method
    {
        try
        {
            List<string> expectedStringValues = new List<string>() { "log", "sin", "cos", "tan", "e", "pi", "(", ")", "*", "/", "-", "+", "^", "endpoint" }; //unknown symbols

            int stopper1 = stoppervalue; //set limit for stopper
            while (stopper1 > 0) //while loop works till result is calculated
            {
                int notCalculateds = 0; //number of not calculated symbols initialised
                for (int i = 0; i < process.Count; i++) //check every symbol
                {
                    if (process[i] != "?") //if symbol isnt question mark
                    {
                        if (expectedStringValues.Contains(process[i].ToString())) //if symbol is unknown
                        {
                            notCalculateds++; //increase number of unknowns
                        }
                    }
                }

                if (notCalculateds == 0) //if all are known
                {
                    break; //break loop
                }

                process.Add("endpoint"); //end point for merging numbers
                int numberOfIndexesToMerge = 0; //number holder
                for (int i = 0; i < process.Count; i++) //for all the process
                {
                    if (!expectedStringValues.Contains(process[i])) //increase the number until you face an unknown symbol (while you hit digits)
                    {
                        numberOfIndexesToMerge++;
                    }
                    else
                    { //if you hit a symbol after digits
                        if (numberOfIndexesToMerge > 1)
                        {
                            for (int j = i - numberOfIndexesToMerge + 1; j < i; j++)
                            {
                                process[i - numberOfIndexesToMerge] += process[j]; //merge all digits in one symbol
                                process[j] = "?"; //set others to question mark (deleted later)
                            }
                        }
                        numberOfIndexesToMerge = 0; //reset number holder
                    }
                }
                process.Remove("endpoint"); //remove endpoint

                for (int j = process.Count - 1; j > -1; j--) //for all processes remove the question marked ones
                {
                    if (process[j].ToString().Equals("?"))
                    {
                        process.RemoveAt(j);
                    }
                }

                int stopper2 = stoppervalue; //stopper set for loop
                while ((process.Contains("(") || process.Contains(")")) && stopper2 > 0) //while process has brackets (solve inside brackets first)
                {
                    bool openBracketFound = false; //is open bracket found
                    int startIndex = 404; //index of open bracket
                    int endIndex = 404; //index of close bracket
                    for (int i = 0; i < process.Count; i++)
                    {
                        if (process[i].Equals("(")) //if you find open bracket
                        {
                            startIndex = i; //update the open bracket position
                            openBracketFound = true; //set the flag
                        }
                        else if (process[i].Equals(")") && openBracketFound) //if you find close bracket and an open bracket was found before
                        {
                            endIndex = i; //update close bracket position
                            List<string> subResult = calculateRec(process.GetRange(startIndex + 1, endIndex - (startIndex + 1))); //you found matching brackets. go in using recursive and return result

                            for (int j = 0; j < subResult.Count; j++) //swap the results with current process starting from opening bracket you found
                            {
                                process[startIndex + j] = subResult[j];
                            }

                            for (int j = startIndex + subResult.Count; j < endIndex + 1; j++) //set the remaining ones till closing bracket to question mark
                            {
                                process[j] = "?";
                            }

                            for (int j = process.Count - 1; j > -1; j--) //remove question marks from process list
                            {
                                if (process[j].ToString().Equals("?"))
                                {
                                    process.RemoveAt(j);
                                }
                            }

                            openBracketFound = false; //set flag to false
                            break; //break to go to the beggining of the process
                        }
                    }

                    stopper2--;
                }

                for (int i = 0; i < process.Count; i++) //for each process (solve functions and constants)
                {
                    if (process[i].ToString().Equals("e")) //if its e replace with value
                    {
                        process[i] = Mathf.Exp(1).ToString();
                    }
                    else if (process[i].ToString().Equals("pi")) //if its pi replace with value
                    {
                        process[i] = Mathf.PI.ToString();
                    }
                    else if (process[i].ToString().Equals("ans")) //if its ans replace with value
                    {
                        process[i] = globals.ans.ToString();
                    }
                    else if (process[i].ToString().Equals("log")) //if its log apply the logarithmic function to next symbols
                    {
                        int numberOfDigits = 0;
                        if (!expectedStringValues.Contains(process[i + 1]))
                        {
                            numberOfDigits++;
                            float resultFloat = Mathf.Log10(float.Parse(process[i + 1]));
                            process[i] = resultFloat.ToString();
                            process[i + 1] = "?";
                        }
                    }
                    else if (process[i].ToString().Equals("cos")) //if its cos apply the trigonometric function to next symbols
                    {
                        if (!expectedStringValues.Contains(process[i + 1]))
                        {
                            float resultFloat = Mathf.Cos((float.Parse(process[i + 1])) * Mathf.Deg2Rad);
                            process[i] = resultFloat.ToString();
                            process[i + 1] = "?";
                        }
                    }
                    else if (process[i].ToString().Equals("sin")) //if its sin apply the trigonometric function to next symbols
                    {
                        if (!expectedStringValues.Contains(process[i + 1]))
                        {
                            float resultFloat = Mathf.Sin((float.Parse(process[i + 1])) * Mathf.Deg2Rad);
                            process[i] = resultFloat.ToString();
                            process[i + 1] = "?";
                        }
                    }
                    else if (process[i].ToString().Equals("tan")) //if its tan apply the trigonometric function to next symbols
                    {
                        if (!expectedStringValues.Contains(process[i + 1]))
                        {
                            float resultFloat = Mathf.Tan((float.Parse(process[i + 1])) * Mathf.Deg2Rad);
                            process[i] = resultFloat.ToString();
                            process[i + 1] = "?";
                        }
                    }
                    for (int j = process.Count - 1; j > -1; j--) //remove if there are question mark'ed symbols (empty spots)
                    {
                        if (process[j].ToString().Equals("?"))
                        {
                            process.RemoveAt(j);
                        }
                    }
                }

                for (int i = 0; i < process.Count; i++) //for each process (solve power functions)
                {
                    if (process[i].ToString().Equals("^")) //if there's a power symbol get the next symbol'th power of prevues symbol
                    {
                        process[i - 1] = Mathf.Pow(float.Parse(process[i - 1]), float.Parse(process[i + 1])).ToString();
                        process[i] = "?";
                        process[i + 1] = "?";
                    }
                    for (int j = process.Count - 1; j > -1; j--) //remove if there are question mark'ed symbols (empty spots)
                    {
                        if (process[j].ToString().Equals("?"))
                        {
                            process.RemoveAt(j);
                        }
                    }
                }

                for (int i = 0; i < process.Count; i++) //for each process (solve division/multiplication)
                {
                    if (process[i].ToString().Equals("/")) //if there's a division symbol divide the previous symbol to next symbol
                    {
                        process[i - 1] = (float.Parse(process[i - 1]) / float.Parse(process[i + 1])).ToString();
                        process[i] = "?";
                        process[i + 1] = "?";
                    }
                    else if (process[i].ToString().Equals("*")) //if there's a multiplication symbol multiply the previous symbol to next symbol
                    {
                        process[i - 1] = (float.Parse(process[i - 1]) * float.Parse(process[i + 1])).ToString();
                        process[i] = "?";
                        process[i + 1] = "?";
                    }
                    for (int j = process.Count - 1; j > -1; j--) //remove if there are question mark'ed symbols (empty spots)
                    {
                        if (process[j].ToString().Equals("?"))
                        {
                            process.RemoveAt(j);
                        }
                    }
                }

                for (int i = 0; i < process.Count; i++) //for each process (solve addition/substraction)
                {
                    if (process[i].ToString().Equals("+")) //if there's a addition symbol add previous symbol to next symbol
                    {
                        process[i - 1] = (float.Parse(process[i - 1]) + float.Parse(process[i + 1])).ToString();
                        process[i] = "?";
                        process[i + 1] = "?";
                    }
                    else if (process[i].ToString().Equals("-")) //if there's a substraction symbol substract next symbol from previous symbol
                    {
                        process[i - 1] = (float.Parse(process[i - 1]) - float.Parse(process[i + 1])).ToString();
                        process[i] = "?";
                        process[i + 1] = "?";
                    }
                    for (int j = process.Count - 1; j > -1; j--) //remove if there are question mark'ed symbols (empty spots)
                    {
                        if (process[j].ToString().Equals("?"))
                        {
                            process.RemoveAt(j);
                        }
                    }
                }


                for (int j = process.Count - 1; j > -1; j--) //remove if there are question mark'ed symbols (empty spots) (if not happened inside the individual loop)
                {
                    if (process[j].ToString().Equals("?"))
                    {
                        process.RemoveAt(j);
                    }
                }
                stopper1--;
            }


            for (int i = 1; i < process.Count; i++) //merge result inside brackets to one symbol (has to be calculated in the loops above)
            {
                process[0] += process[i];
                process[i] = "?";
            }
            for (int j = process.Count - 1; j > -1; j--) //remove if there are question mark'ed symbols (empty spots)
            {
                if (process[j].ToString().Equals("?"))
                {
                    process.RemoveAt(j);
                }
            }


            return process;
        }catch //if try fails return error
        {
            return new List<string>() { "error" };
        }
        
    }
}
