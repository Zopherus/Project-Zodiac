using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainTycoonScript : MonoBehaviour {

    private int money; //amount of money player has
    private int delegates; //number of delegates player has
    private string busy; //wether the player is busy doing an event or not (if busy, string is event they are doing, otherwise busy = "")
    private float timer; //stores time until no longer busy
    private float DAY_LENGTH; //time (in seconds) per day

    //constant variables for events
    private const int PROFIT_SPONSOR_SPEECH = 200; //money made from sponsor speech
    private const int TIME_SPONSOR_SPEECH = 1; //time for sponsor speech in days

    private const int PROFIT_SUPER_PAC = 500; //money made from super PAC
    private const int TIME_SUPER_PAC = 2; //time for super PAC in days

    private const int COST_TOWN_HALL = 100; //cost of town hall
    private const int TIME_TOWN_HALL = 3; //time for town hall in days

    private const int COST_TELEVISION_AD = 1000; //cost of television ad
    private const int TIME_TELEVISION_AD = 2; //time for television ad in days

    private const int COST_CONFERENCE = 300; //money made from sponsor speech
    private const int TIME_CONFERENCE = 6; //time for conference in days

    // Use this for initialization
    void Start()
    {
        //initialize variables to 0
        money = 0;
        delegates = 0;
        busy = "";
        timer = 0f;
        //initialize day length from const variable in DayCycleScript (to keep things consistent and simple)
        DAY_LENGTH = DayCycleScript.DAY_CYCLE;
    }
	
	// Update is called once per frame
	void Update () {
        //if player is busy, update timer
        if(busy != "")
        {
            timer -= Time.deltaTime;
            if(timer <= 0) //if timer reaches 0, player finishes event
            {
                //add money based and popualarity to player stas
                switch(busy)
                {
                    case "sponsor speech":
                        money += PROFIT_SPONSOR_SPEECH;
                        UpdateMoney();
                        break;
                    case "super pac":
                        money += PROFIT_SUPER_PAC;
                        UpdateMoney();
                        break;
                }
                //set busy back to zero
                busy = "";
            }
        }
	}

    //Sponsor Speech event is bought
    public void SponsorSpeech()
    {
        if(busy == "")
        {
            //if not already busy, set busy to "sponsor speech" and add length to timer
            busy = "sponsor speech";
            timer = TIME_SPONSOR_SPEECH * DAY_LENGTH;
        }
    }
    
    //Super PAC event is bought
    public void SuperPAC()
    {
        if (busy == "")
        {
            //if not already busy, set busy to "super pac" and add length to timer
            busy = "super pac";
            timer = TIME_SUPER_PAC * DAY_LENGTH;
        }
    }

    //Town Hall event is bought
    public void TownHall()
    {
        if (busy == "" && money >= COST_TOWN_HALL)
        {
            //if not already busy and has enough money, set busy to "town hall", subtract money and add length to timer
            busy = "town hall";
            timer = TIME_TOWN_HALL * DAY_LENGTH;
            money -= COST_TOWN_HALL;
            UpdateMoney();
        }
    }

    //Television Ad event is bought
    public void TelevisionAd()
    {
        if (busy == "" && money >= COST_TELEVISION_AD)
        {
            //if not already busy and has enough money, set busy to "television ad", subtract money and add length to timer
            busy = "television ad";
            timer = TIME_TELEVISION_AD * DAY_LENGTH;
            money -= COST_TELEVISION_AD;
        }
    }

    //Conference is event is bought
    public void Conference()
    {
        if (busy == "" && money >= COST_CONFERENCE)
        {
            //if not already busy and has enough money, set busy to "conference", subtract money and add length to timer
            busy = "conference";
            timer = TIME_CONFERENCE * DAY_LENGTH;
            money -= COST_CONFERENCE;
            UpdateMoney();
        }
    }

    //update amount of money displayed
    public void UpdateMoney()
    {
        //get text from child
        Text yourStats = transform.GetChild(0).GetChild(0).GetComponent<Text>();

        string[] lines = yourStats.text.Split('\n'); //split yourStats string into lines and edit relevant line
        lines[0] = "Money: $" + money.ToString();

        yourStats.text = lines[0]; //re-add (now edited) lines to child Text object
        for(int i = 1; i < lines.Length - 1; i++)
        {
            yourStats.text += lines[i] + "\n";
        }
    }
}
