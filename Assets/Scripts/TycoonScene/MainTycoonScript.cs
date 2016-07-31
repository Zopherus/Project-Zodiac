using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Collections;

#region MainTycoonScript Class
public class MainTycoonScript : MonoBehaviour
{
    //character enum
    public enum Character { Trump, Cruz, Clinton, Sanders };
    public enum eventsButtons { Clear, SponsorSpeech, SuperPAC, TownHall, TelevisionAd, Conference };
    //class instances
    private static CalendarHandler calendar;
    public static EventHandler events;
    //private ScrollingTextHandler scrollingText;
    //variables
    static public Character currentCharacter;
    public bool timeIsActive = true; //whether or not time goes by (ex. settings)
    private Text highlightText; //text object for displaying highlightText
    private Text stateText; //text object for displaying state info
    public static State[] states = new State[10];
    public static bool firstTimeAwake = true;
    public static float[] policyValues = new float[4];

    //const variables
    public const float DAY_LENGTH = 10f; //length of 1 day in seconds
    public const int DAYS_PER_CYCLE = 10; //number of days in each cycle

    // Use this for initialization
    void Awake()
    {
        if (firstTimeAwake)
        {
            //initialize variables class instances
            Text[] components = transform.FindChild("Canvas").FindChild("Calendar").GetComponentsInChildren<Text>(); //get text components
            calendar = new CalendarHandler(this, DAY_LENGTH, components[0], components[1]);
            events = new EventHandler(DAY_LENGTH);
            //scrollingText = new ScrollingTextHandler(this);
            highlightText = GameObject.Find("HighlightText").GetComponent<Text>();
            stateText = GameObject.Find("StateText").GetComponent<Text>();
            System.Random r = new System.Random(); //seed rng outside of loop (otherwise, each rng will be seeded with the same or close to the same values -> not random AT ALL)
            for (int i = 0; i < states.Length; i++)
            {
                states[i] = new State(i.ToString(), r); //iniatialize each state and seed it randomly
            }
            changeState(1); //display first state
        }
        else
        {
            GetComponent<AudioSource>().Stop();
            PopularityManager.currentState++;
            if (PopularityManager.currentState >= 10) //end of game
            {
                timeIsActive = false;
                int totalPossibleDelegates = 0;
                for (int i = 0; i < states.Length; i++)
                {
                    totalPossibleDelegates += states[i].numDelegates;
                }
                if((float)events.delegates / (float)totalPossibleDelegates > 0.5f)
                {
                    //win
                    GameObject.Find("Won").SetActive(true);
                }
                else
                {
                    //lose
                    GameObject.Find("Lost").SetActive(false);
                }
            }
            calendar.sceneTimerTime = GameObject.Find("TimeUntil").gameObject.GetComponent<Text>();
            calendar.sceneTimerDays = GameObject.Find("DaysUntil").gameObject.GetComponent<Text>();
            calendar.calendarDayText = GameObject.Find("DayText").GetComponent<Text>();
            calendar.calendarMonthText = GameObject.Find("MonthText").GetComponent<Text>();
            //scrollingText = new ScrollingTextHandler(this);
            highlightText = GameObject.Find("HighlightText").GetComponent<Text>();
            stateText = GameObject.Find("StateText").GetComponent<Text>();
            changeState(PopularityManager.currentState); //display first state
            calendar.numDays = 0;
            events.UpdateMoney(this); //update money
            events.popularity.UpdatePopularity(this, GetPolicyValues(), 1f);
            //UpdatePolicyValues();
            GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().Play();
        }
    }

    //same as awake, but only when level is reloaded
    void OnLevelWasLoaded(int level)
    {
        //if this level was loaded, initialize icon and reactivate time variable
        if (level == 1)
        {
            //reactivate timeIsActive
            timeIsActive = true;
            //initialize character icon
            GameObject trumpIcon = transform.Find("Canvas").Find("Icon").FindChild("Trump").gameObject;
            GameObject cruzIcon = transform.Find("Canvas").Find("Icon").FindChild("Cruz").gameObject;
            GameObject sandersIcon = transform.Find("Canvas").Find("Icon").FindChild("Sanders").gameObject;
            GameObject clintonIcon = transform.Find("Canvas").Find("Icon").FindChild("Clinton").gameObject;
            switch (currentCharacter)
            {
                case Character.Trump:
                    trumpIcon.SetActive(true);
                    cruzIcon.SetActive(false);
                    sandersIcon.SetActive(false);
                    clintonIcon.SetActive(false);
                    break;
                case Character.Cruz:
                    cruzIcon.SetActive(true);
                    trumpIcon.SetActive(false);
                    sandersIcon.SetActive(false);
                    clintonIcon.SetActive(false);
                    break;
                case Character.Sanders:
                    sandersIcon.SetActive(true);
                    trumpIcon.SetActive(false);
                    cruzIcon.SetActive(false);
                    clintonIcon.SetActive(false);
                    break;
                case Character.Clinton:
                    clintonIcon.SetActive(true);
                    trumpIcon.SetActive(false);
                    cruzIcon.SetActive(false);
                    sandersIcon.SetActive(false);
                    break;
            }

            //reinitialize all variables
        }
    }

    // Update is called once per frame
    void Update()
    {
        //update time and calendar
        if (timeIsActive)
        {
            events.UpdateTimer(this);
            if(calendar.numDays % DAYS_PER_CYCLE == 0 && calendar.numDays > 0)
            {
                firstTimeAwake = false;
                policyValues = GetPolicyValues(); //update static policy values array
                timeIsActive = false;
                SceneManager.LoadScene("Fighting Scene");
            }
            calendar.UpdateCalendar();
            //scrollingText.UpdateScrollingText();
        }
    }

    public void changeState(int state)
    {
        stateText.text = "";
        if(PopularityManager.currentState == state - 1) { stateText.text += "CURRENT STATE\n"; }
        stateText.text += states[state - 1].numDelegates + " delegates" + "\n";
        for(int i = 0; i < events.POLICY_LEANINGS_NAMES.Length; i++)
        {
            stateText.text += events.POLICY_LEANINGS_NAMES[i] + ": ";
            if(states[state - 1].policyLeanings[i] < 45) //between 0 and 45 (liberal)
            {
                stateText.text += "liberal\n";
            }
            else if(states[state - 1].policyLeanings[i] > 55) //between 55 and 100 (conservative)
            {
                stateText.text += "conservative\n";
            }
            else //between 45 and 55 (moderate)
            {
                stateText.text += "moderate\n";
            }
            
        }
        if (states[state - 1].fightFinished)
        {
            if (states[state - 1].won) { stateText.text += "You have WON this state\n"; } // player has won this state
            else { stateText.text += "You have LOST this state\n"; } //player has lost this state
        }
    }

    //change text when highlightd
    public void changeHighlightsText(string _event)
    {
        switch(_event)
        {
            case "Clear":
                highlightText.text = "";
                break;
            case "SponsorSpeech":
                highlightText.text = "Give a speech to your sponsors!\nTakes 0.5 day\nReturns $200";
                break;
            case "SuperPAC":
                highlightText.text = "Expand your Super PAC!\nTakes 1 day\nReturns $500";
                break;
            case "TownHall":
                highlightText.text = "Take part in a Town Hall meeting!\nTakes 1.5 days\nCosts $100\nModerately effective";
                break;
            case "TelevisionAd":
                highlightText.text = "Release a TV Ad!\nTakes 1 day\nCosts $1000\nVery effective";
                break;
            case "Conference":
                highlightText.text = "Host a fully fledged conference!\nTakes 3 days\nCosts $300\nVery effective";
                break;
        }
    }

    //Deprecated
    //***************************
    //opens settings
    public void Settings()
    {
        timeIsActive = false;
    }
    //****************************

    //Switch Scene to MainMenuScene
    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    //call sponsor speech from EventHandler class instance
    public void TriggerSponsorSpeech()
    {
        if (timeIsActive)
        {
            events.SponsorSpeech();
        }
    }

    //call super PAC from EventHandler class instance
    public void TriggerSuperPAC()
    {
        if (timeIsActive)
        {
            events.SuperPAC();
        }
    }

    //call town hall from EventHandler class instance
    public void TriggerTownHall()
    {
        if (timeIsActive)
        {
            events.TownHall(this);
        }
    }

    //call television ad from EventHandler class instance
    public void TriggerTelevisionAd()
    {
        if (timeIsActive)
        {
            events.TelevisionAd(this);
        }
    }

    //call conference from EventHandler class instance
    public void TriggerConference()
    {
        if (timeIsActive)
        {
            events.Conference(this);
        }
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

    //update policy values in child sliders
    public void UpdatePolicyValues()
    {
        PolicyScript[] policyScripts = transform.FindChild("Canvas").FindChild("Policies").GetComponentsInChildren<PolicyScript>();
        for (int i = 0; i < policyScripts.Length; i++)
        {
            policyScripts[i].value = policyValues[i];
            policyScripts[i].UpdateMedian();
        }
    }
}
#endregion

#region PopularityManagerClass
public class PopularityManager
{
    public static float popularity; //percent of people i.e. number from 1-100
    public static int currentState; //number of current state
    //array of policy factors ([state number, policy]): between -1.0 and 1.0
    //private float[,] policyFactors = {{ 0f,  0f,  0f,  0f}, //1 row for each state
    ///* [,0]: foriegn factor        */ { 0f,  0f,  0f,  0f},
    ///* [,1]: economic factor       */ { 0f,  0f,  0f,  0f},
    ///* [,2]: social factor         */ { 0f,  0f,  0f,  0f},
    ///* [,3]: persona factor        */ { 0f,  0f,  0f,  0f},
    //                                  { 0f,  0f,  0f,  0f},
    //                                  { 0f,  0f,  0f,  0f},
    //                                  { 0f,  0f,  0f,  0f},
    //                                  { 0f,  0f,  0f,  0f},
    //                                  { 0f,  0f,  0f,  0f}};

    public PopularityManager(float initialPopularity)
    {
        popularity = initialPopularity;
        currentState = 0;
    }

    //updates popularity based on state stats
    public void UpdatePopularity(MainTycoonScript script, float[] values, float differenceMultiplier)
    {
        float difference = 0;
        PolicyScript[] policyScripts = script.transform.FindChild("Canvas").FindChild("Policies").GetComponentsInChildren<PolicyScript>();
        for (int i = 0; i < values.Length; i++)
        {
            //update the median in each policy script
            policyScripts[i].UpdateMedian();
            difference += Math.Abs(values[i] - MainTycoonScript.states[currentState].policyLeanings[i]);
        }

        difference /= 4f; //divide by 4 since there are 4 values (max difference is 100)
        difference *= differenceMultiplier;  //multiplier by multiplier (since some events are less effective)
        popularity = 100f - difference; //popularity is just max (100) minus difference between policy leanings and player values

        //float difference = 0;
        //PolicyScript[] policyScripts = script.transform.FindChild("Canvas").FindChild("Policies").GetComponentsInChildren<PolicyScript>();
        //for (int i = 0; i < values.Length; i++)
        //{
        //    //update the median in each policy script
        //    policyScripts[i].UpdateMedian();

        //    //get overall difference (sum of all 4 differences for policy)
        //    float tempDif = values[i] - policyFactors[currentState, i];
        //    difference += tempDif;
        //}
        //difference /= 4;
        //popularity = 100f - (difference * differenceMultiplier);

        //update text
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
#endregion

#region CalendarHandler Class
//class for handling calendar/date actions
public class CalendarHandler
{
    public int numDays; //public access var for number of days
    private DateTime calendar; //calender (for day and month references)
    private float dayTimer; //time until next day
    public Text calendarDayText; //child text object for days
    public Text calendarMonthText; //child text object for months
    public float SECONDS_PER_DAY; //seconds per day
    public float DAYS_IN_CYCLE; //days in a cycle
    public Text sceneTimerTime; //timer text in scene (for displaying time until next fight)
    public Text sceneTimerDays; //timer text in scene (for displaying days until next fight)

    public CalendarHandler(MainTycoonScript script, float dayLength, Text dayText, Text monthText)
    {
        calendar = new DateTime(2016, 4, 20);
        SECONDS_PER_DAY = dayLength;
        dayTimer = SECONDS_PER_DAY;
        DAYS_IN_CYCLE = MainTycoonScript.DAYS_PER_CYCLE;

        calendarDayText = dayText;
        calendarMonthText = monthText;
        sceneTimerTime = GameObject.Find("TimeUntil").gameObject.GetComponent<Text>();
        sceneTimerDays = GameObject.Find("DaysUntil").gameObject.GetComponent<Text>();
    }

    public void UpdateCalendar()
    {
        //every 10 seconds, change the day
        dayTimer -= Time.deltaTime;
        if (dayTimer <= 0)
        {
            numDays++;
            calendar = calendar.AddDays(1.0);
            calendarDayText.text = calendar.Day.ToString(); //update calendar graphic
            calendarMonthText.text = new DateTimeFormatInfo().GetAbbreviatedMonthName(calendar.Month);
            dayTimer = SECONDS_PER_DAY;
        }
        //update mainTimer in scene
        int totalSecondsLeft = (int)((14 - (numDays % DAYS_IN_CYCLE)) * SECONDS_PER_DAY + dayTimer);
        string minutes = (totalSecondsLeft / 60).ToString("D2");
        string seconds = (totalSecondsLeft % 60).ToString("D2");
        sceneTimerTime.text = "TIME UNTIL NEXT FIGHT: " + minutes + ":" + seconds;
        sceneTimerDays.text = "DAYS UNTIL NEXT FIGHT: " + (DAYS_IN_CYCLE - (numDays % DAYS_IN_CYCLE)).ToString();
    }
}
#endregion

#region EventHandlerClass
//class for handling political event actions
public class EventHandler
{
    private int money; //amount of money player has
    public int delegates; //number of delegates player has
    private string busy; //wether the player is busy doing an event or not (if busy, string is event they are doing, otherwise busy = "")
    private float timer; //stores time until no longer busy
    private float DAY_LENGTH; //length of day (get from main script)
    public PopularityManager popularity;

    public readonly string[] POLICY_LEANINGS_NAMES = { "Foreign Policy", "Social Policy", "Economic Policy", "Charisma" };

    //constant variables for events
    private const int PROFIT_SPONSOR_SPEECH = 200; //money made from sponsor speech
    private const float TIME_SPONSOR_SPEECH = 0.5f; //time for sponsor speech in days

    private const int PROFIT_SUPER_PAC = 500; //money made from super PAC
    private const float TIME_SUPER_PAC = 1f; //time for super PAC in days

    private const int COST_TOWN_HALL = 100; //cost of town hall
    private const float TIME_TOWN_HALL = 1.5f; //time for town hall in days
    private const float MULTIPLIER_TIME_HALL = 0.75f;

    private const int COST_TELEVISION_AD = 1000; //cost of television ad
    private const float TIME_TELEVISION_AD = 1f; //time for television ad in days
    private const float MULTIPLIER_TELEVISION_AD = 1.5f;

    private const int COST_CONFERENCE = 300; //cost of conference
    private const float TIME_CONFERENCE = 3f; //time for conference in days
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
        Vector3 scale = new Vector3(1, 0.3f, 1);
        //x scale will be time left divided by total time
        switch (busy)
        {
            case "sponsor speech":
                scale = new Vector3(timeRemainingEvent / TIME_SPONSOR_SPEECH, 0.3f, 1);
                break;
            case "super pac":
                scale = new Vector3(timeRemainingEvent / TIME_SUPER_PAC, 0.3f, 1);
                break;
            case "town hall":
                scale = new Vector3(timeRemainingEvent / TIME_TOWN_HALL, 0.3f, 1);
                break;
            case "television ad":
                scale = new Vector3(timeRemainingEvent / TIME_TELEVISION_AD, 0.3f, 1);
                break;
            case "conference":
                scale = new Vector3(timeRemainingEvent / TIME_CONFERENCE, 0.3f, 1);
                break;
        }
        var statusBar = script.transform.FindChild("StatusBar"); //get status bar and apply changes
        statusBar.transform.localScale = scale;
    }

    //update amount of money displayed
    public void UpdateMoney(MainTycoonScript script)
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
#endregion

/*#region ScrollingTextHandlerClass
public class ScrollingTextHandler : MonoBehaviour
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

        StreamReader fileImport = new StreamReader(importFilePath); //access file at provided path
        List<string> headlines = new List<string>(fileImport.ReadToEnd().Split('\n'));
        headlines.Shuffle();
        foreach (string s in headlines)
        {
            scrollingText.text += s + " | ";
        }
        fileImport.Close();
    }

    // Update is called once per frame
    public void UpdateScrollingText()
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
            StreamReader fileImport = new StreamReader(importFilePath); //access file at provided path
            List<string> headlines = new List<string>(fileImport.ReadToEnd().Split('\n'));
            headlines.Shuffle();
            foreach(string s in headlines)
            {
                scrollingText.text += s + " | ";
            }       


            fileImport.Close();
        }
    }
}
#endregion*/

#region State Struct
public struct State
{
    public string name;
    public bool won;
    public bool fightFinished;
    public int numDelegates;
    public int[] policyLeanings;

    public State(string stateName, System.Random r)
    {
        name = stateName;
        won = false;
        fightFinished = false;
        numDelegates = r.Next(20, 30); //randomly initialize numDelegates and policyLeanings
        policyLeanings = new int[] { r.Next(0, 100), r.Next(0, 100), r.Next(0, 100), r.Next(0, 100) };
    }
}
#endregion

#region Shuffle
static class ShufflerClass
{
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
#endregion