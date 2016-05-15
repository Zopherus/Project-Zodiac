using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using System.IO;
using System.Collections;

public class MainTycoonScript : MonoBehaviour
{
    //class instances
    private CalendarHandler calendar;
    private EventHandler events;
    private ScrollingTextHandler scrollingText;

    //const variables
    const float DAY_LENGTH = 1f;

    // Use this for initialization
    void Awake()
    {
        //initialize variables class instances
        Text[] components = transform.FindChild("Canvas").FindChild("Calendar").GetComponentsInChildren<Text>(); //get text components
        calendar = new CalendarHandler(this, DAY_LENGTH, components[0], components[1]);
        events = new EventHandler(DAY_LENGTH);
        scrollingText = new ScrollingTextHandler(this);
    }

    // Update is called once per frame
    void Update()
    {
        events.UpdateTimer(this);
        calendar.UpdateCalendar();
        scrollingText.UpdateScrollingText();
    }

    //Switch Scene to MainMenuScene
    public void ExitToMenu()
    {
        Application.LoadLevel("MainMenuScene");
    }

    //call sponsor speech from EventHandler class instance
    public void TriggerSponsorSpeech()
    {
        events.SponsorSpeech();
    }

    //call super PAC from EventHandler class instance
    public void TriggerSuperPAC()
    {
        events.SuperPAC();
    }

    //call town hall from EventHandler class instance
    public void TriggerTownHall()
    {
        events.TownHall(this);
    }

    //call television ad from EventHandler class instance
    public void TriggerTelevisionAd()
    {
        events.TelevisionAd(this);
    }

    //call conference from EventHandler class instance
    public void TriggerConference()
    {
        events.Conference(this);
    }

    //gets policy values from policy scripts
    public float[] GetPolicyValues()
    {
        float[] values = new float[4];
        PolicyScript[] policyScripts = transform.FindChild("Canvas").FindChild("Policies").GetComponentsInChildren<PolicyScript>();
        for(int i = 0; i < policyScripts.Length; i++)
        {
            values[i] = policyScripts[i].value;
        }
        return values;
    }
}

public class PopularityManager
{
    private float popularity; //percent of people i.e. number from 1-100
    private int currentState; //number of current state
    //array of policy factors ([state number, policy]): between -1.0 and 1.0
    private float[,] policyFactors = {{ 0f,  0f,  0f,  0f}, //1 row for each state
    /* [,0]: foriegn factor        */ { 0f,  0f,  0f,  0f},
    /* [,1]: economic factor       */ { 0f,  0f,  0f,  0f},
    /* [,2]: social factor         */ { 0f,  0f,  0f,  0f},
    /* [,3]: persona factor        */ { 0f,  0f,  0f,  0f},
                                      { 0f,  0f,  0f,  0f},
                                      { 0f,  0f,  0f,  0f},
                                      { 0f,  0f,  0f,  0f},
                                      { 0f,  0f,  0f,  0f},
                                      { 0f,  0f,  0f,  0f}};

    public PopularityManager(float initialPopularity)
    {
        popularity = initialPopularity;
        currentState = 0;
    }

    //updates popularity based on state stats
    public void UpdatePopularity(MainTycoonScript script, float[] values, float differenceMultiplier)
    {
        float difference = 0;
        for (int i = 0; i < values.Length; i++)
        {
            float tempDif = values[i] - policyFactors[currentState, i];
            difference += tempDif;
        }
        difference /= 4;
        popularity = 100f - (difference * differenceMultiplier);

        Text statsText = script.transform.FindChild("Canvas").FindChild("YourStats").GetComponent<Text>();
        string[] newStatText = statsText.text.Split('\n');
        newStatText[1] = String.Format("Popularity: {0:N1}%", popularity);

        statsText.text = string.Empty;
        foreach(string line in newStatText)
        {
            statsText.text += line + "\n";
        }
    }

    //increases state number (go on to next state)
    public void goToNextState()
    {
        currentState++;
    }
}

//class for handling calendar/date actions
public class CalendarHandler
{
    private DateTime calendar; //calender (for day and month references)
    private float timer; //time until next day
    private Text calendarDayText; //child text object for days
    private Text calendarMonthText; //child text object for months
    public float DAY_CYCLE; //seconds per day

    public CalendarHandler(MainTycoonScript script, float dayLength, Text dayText, Text monthText)
    {
        calendar = new DateTime(2016, 4, 20);
        DAY_CYCLE = dayLength;
        timer = DAY_CYCLE;

        calendarDayText = dayText;
        calendarMonthText = monthText;
    }

    public void UpdateCalendar()
    {
        //every 10 seconds, change the day
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            calendar = calendar.AddDays(1.0);
            calendarDayText.text = calendar.Day.ToString(); //update calendar graphic
            calendarMonthText.text = new DateTimeFormatInfo().GetAbbreviatedMonthName(calendar.Month);
            timer = DAY_CYCLE;
        }
    }
}

//class for handling political event actions
public class EventHandler
{
    private int money; //amount of money player has
    private int delegates; //number of delegates player has
    private string busy; //wether the player is busy doing an event or not (if busy, string is event they are doing, otherwise busy = "")
    private float timer; //stores time until no longer busy
    private float DAY_LENGTH; //length of day (get from main script)
    private PopularityManager popularity;

    //constant variables for events
    private const int PROFIT_SPONSOR_SPEECH = 200; //money made from sponsor speech
    private const int TIME_SPONSOR_SPEECH = 1; //time for sponsor speech in days

    private const int PROFIT_SUPER_PAC = 500; //money made from super PAC
    private const int TIME_SUPER_PAC = 2; //time for super PAC in days

    private const int COST_TOWN_HALL = 100; //cost of town hall
    private const int TIME_TOWN_HALL = 3; //time for town hall in days
    private const float MULTIPLIER_TIME_HALL = 0.75f;

    private const int COST_TELEVISION_AD = 1000; //cost of television ad
    private const int TIME_TELEVISION_AD = 2; //time for television ad in days
    private const float MULTIPLIER_TELEVISION_AD = 1.5f;

    private const int COST_CONFERENCE = 300; //money made from sponsor speech
    private const int TIME_CONFERENCE = 6; //time for conference in days
    private const float MULTIPLIER_CONFERENCE = 1.5f;

    public EventHandler(float dayLength)
    {
        money = 0;
        delegates = 0;
        busy = "";
        timer = 0f;
        DAY_LENGTH = dayLength;
        popularity = new PopularityManager(50f); //start with 50% of people voting for player's candidate
    }

    //Sponsor Speech event is bought
    public void SponsorSpeech()
    {
        if (busy == "")
        {
            //if not already busy, set busy to "sponsor speech" and add length to timer
            busy = "sponsor speech";
            timer = TIME_SPONSOR_SPEECH * DAY_LENGTH; //add money for speech late
        }
    }

    //Super PAC event is bought
    public void SuperPAC()
    {
        if (busy == "")
        {
            //if not already busy, set busy to "super pac" and add length to timer
            busy = "super pac";
            timer = TIME_SUPER_PAC * DAY_LENGTH; //add money for super pac later
        }
    }

    //Town Hall event is bought
    public void TownHall(MainTycoonScript script)
    {
        if (busy == "" && money >= COST_TOWN_HALL)
        {
            //if not already busy and has enough money, set busy to "town hall", subtract money and add length to timer
            busy = "town hall";
            timer = TIME_TOWN_HALL * DAY_LENGTH;
            money -= COST_TOWN_HALL;
            UpdateMoney(script);
            popularity.UpdatePopularity(script, script.GetPolicyValues(), MULTIPLIER_TIME_HALL);
        }
    }

    //Television Ad event is bought
    public void TelevisionAd(MainTycoonScript script)
    {
        if (busy == "" && money >= COST_TELEVISION_AD)
        {
            //if not already busy and has enough money, set busy to "television ad", subtract money and add length to timer
            busy = "television ad";
            timer = TIME_TELEVISION_AD * DAY_LENGTH;
            money -= COST_TELEVISION_AD;
            UpdateMoney(script);
            popularity.UpdatePopularity(script, script.GetPolicyValues(), MULTIPLIER_TELEVISION_AD);
        }
    }

    //Conference is event is bought
    public void Conference(MainTycoonScript script)
    {
        if (busy == "" && money >= COST_CONFERENCE)
        {
            //if not already busy and has enough money, set busy to "conference", subtract money and add length to timer
            busy = "conference";
            timer = TIME_CONFERENCE * DAY_LENGTH;
            money -= COST_CONFERENCE;
            UpdateMoney(script);
            popularity.UpdatePopularity(script, script.GetPolicyValues(), MULTIPLIER_CONFERENCE);
        }
    }

    //update timer and other time-related variables
    public void UpdateTimer(MainTycoonScript script)
    {
        //if player is busy, update timer
        if (busy != "")
        {
            timer -= Time.deltaTime;
            if (timer <= 0) //if timer reaches 0, player finishes event
            {
                UpdateStatusBar(0, script); //update status bar so it scales to 0
                //add money based and popualarity to player stas
                switch (busy)
                {
                    case "sponsor speech":
                        money += PROFIT_SPONSOR_SPEECH;
                        UpdateMoney(script);
                        break;
                    case "super pac":
                        money += PROFIT_SUPER_PAC;
                        UpdateMoney(script);
                        break;
                }
                //set busy back to zero
                busy = "";
                //set timer to 0 so Status Bar scales to 0
                timer = 0f;
            }
            else
            {
                UpdateStatusBar(timer / DAY_LENGTH, script); //update status bar, accounting for DAY_LENGTH
            }
        }   
    }    

    //update size of status bar
    private void UpdateStatusBar(float timeRemainingEvent, MainTycoonScript script)
    {
        //get status bar and change x scale based on time elapsed
        Vector3 scale = new Vector3(1, 1, 1);
        //x scale will be time left divided by total time
        switch (busy)
        {
            case "sponsor speech":
                scale = new Vector3(timeRemainingEvent / TIME_SPONSOR_SPEECH, 1, 1);
                break;
            case "super pac":
                scale = new Vector3(timeRemainingEvent / TIME_SUPER_PAC, 1, 1);
                break;
            case "town hall":
                scale = new Vector3(timeRemainingEvent / TIME_TOWN_HALL, 1, 1);
                break;
            case "television ad":
                scale = new Vector3(timeRemainingEvent / TIME_TELEVISION_AD, 1, 1);
                break;
            case "conference":
                scale = new Vector3(timeRemainingEvent / TIME_CONFERENCE, 1, 1);
                break;
        }
        var statusBar = script.transform.FindChild("StatusBar"); //get status bar and apply changes
        statusBar.transform.localScale = scale;
    }

    //update amount of money displayed
    private void UpdateMoney(MainTycoonScript script)
    {
        //get text from child
        Text yourStats = script.transform.FindChild("Canvas").FindChild("YourStats").GetComponent<Text>();

        string[] lines = yourStats.text.Split('\n'); //split yourStats string into lines and edit relevant line
        lines[0] = "Money: $" + money.ToString();

        yourStats.text = ""; //reset text
        foreach (string s in lines) //re-add (now edited) lines to child Text object
        {
            yourStats.text += s + "\n";
        }
    }
}

public class ScrollingTextHandler
{
    private Text scrollingText; //child text object
    private float timer;
    private System.Random rand; //random number generator
    private const float REFRESH_CYCLE = 0.1f; //seconds per char refresh
    private const float IMPORT_CYCLE = 60; //characters per import check
    private string importFilePath = "Assets\\SaveData\\ScrollingText.txt"; //path for scrolling text import

    // Use this for initialization
    public ScrollingTextHandler(MainTycoonScript script)
    {
        scrollingText = script.transform.FindChild("Canvas").FindChild("ScrollingText").GetComponentInChildren<Text>();
        timer = REFRESH_CYCLE;
        rand = new System.Random();
    }

    // Update is called once per frame
    public void UpdateScrollingText()
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
            while (nextLine != null) //get length of file
            {
                length++;
                nextLine = fileImport.ReadLine();
            }
            fileImport.Close(); //close and reopen the file

            fileImport = new StreamReader(importFilePath);
            int randomMax = rand.Next(0, length);
            for (int i = 0; i < randomMax; i++)//choose random number between 0 and length, skip that many lines
            {
                fileImport.ReadLine();
            }
            string lineToAppend = fileImport.ReadLine();
            scrollingText.text = scrollingText.text + "  |  " + lineToAppend; //add the next line (from file)
            fileImport.Close();
        }
    }
}
