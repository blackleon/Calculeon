using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class globals : MonoBehaviour
{
    public static List<string> process = new List<string>(); //global process variable
    public static float ans = 0; //global answer variable
    public static Text processText; //process ui
    public static Text resultText; //result ui
    private void Start()
    {
        processText = GameObject.Find("process").GetComponent<Text>(); //set process object
        resultText = GameObject.Find("result").GetComponent<Text>(); //set result object
    }
    public static void updateProcessText() //update the text for process ui
    {
        processText.text = "";
        foreach (string part in process)
        {
            processText.text += part;
        }
    }

    public static void updateResultText(string result) //update the text for result ui (value must be passed as string)
    {
        resultText.text = result;
    }
}
