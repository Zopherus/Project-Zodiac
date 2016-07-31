using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class ScrollingText : MonoBehaviour
{
    public TextAsset asset;
    private Text scrollingText; //child text object
    private float timer;
    private System.Random rand; //random number generator
    private const float REFRESH_CYCLE = 0.1f; //seconds per char refresh
    private const float IMPORT_CYCLE = 60; //characters per import check
    

    void Start()
    {
        scrollingText = transform.FindChild("Canvas").FindChild("ScrollingText").GetComponentInChildren<Text>();
        timer = REFRESH_CYCLE;
        rand = new System.Random();

        List<string> headlines = new List<string>(asset.text.Split('\n'));
        headlines.Shuffle();
        foreach (string s in headlines)
        {
            scrollingText.text += s + " | ";
        }
    }

    // Update is called once per frame
    void Update()
    {
        //make the text scroll
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = REFRESH_CYCLE;
            scrollingText.text = scrollingText.text.Remove(0, 1); //remove first char and update text
        }

        //every IMPORT_CYCLE number of chars, append new line of scrolling text
        if (scrollingText.text.Length < IMPORT_CYCLE)
        {
            List<string> headlines = new List<string>(asset.text.Split('\n'));
            headlines.Shuffle();
            foreach (string s in headlines)
            {
                scrollingText.text += s + " | ";
            }
        }
    }
}