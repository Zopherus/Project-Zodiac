using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;

public class ScrollingTextScript : MonoBehaviour {

    Text scrollingText; //child text object
    private float timer;
    private System.Random rand; //random number generator
    const float REFRESH_CYCLE = 0.1f; //seconds per char refresh
    const float IMPORT_CYCLE = 30; //characters per import check
    string importFilePath = "ScrollingText.txt"; //path for scrolling text import

    // Use this for initialization
    void Start() {
        scrollingText = GetComponent<Text>();
        timer = REFRESH_CYCLE;
        rand = new System.Random();
    }

    // Update is called once per frame
    void Update()
    {
        //make the text scroll
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            scrollingText.text = scrollingText.text.Remove(0, 1); //remove first char and update text
            timer = REFRESH_CYCLE;
        }

        //every IMPORT_CYCLE number of chars, append new line of scrolling text
        if (scrollingText.text.Length < IMPORT_CYCLE)
        {
            StreamReader fileImport = new StreamReader(importFilePath); //access file at provided path
            int length = 0;
            string nextLine = fileImport.ReadLine();
            while(nextLine != null) //get length of file
            {
                length++;
                nextLine = fileImport.ReadLine();
            }
            fileImport.Close(); //close and reopen the file

            fileImport = new StreamReader(importFilePath);
            int randomMax = rand.Next(0, length);
            for(int i = 0; i < randomMax; i++)//choose random number between 0 and length, skip that many lines
            {
                fileImport.ReadLine();
            }
            string lineToAppend = fileImport.ReadLine();
            scrollingText.text = scrollingText.text + "  |  " + lineToAppend; //add the next line (from file)
            fileImport.Close();
        }
    }
}