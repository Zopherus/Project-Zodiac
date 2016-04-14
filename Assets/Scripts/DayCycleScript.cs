using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using System.Collections;

public class DayCycleScript : MonoBehaviour {

    private DateTime calendar; //calender (for day and month references)
    private float timer;
    private Text calendarDayText; //child text object for days
    private Text calendarMonthText; //child text object for months
    public const float DAY_CYCLE = 10f; //seconds per day

    // Use this for initialization
    void Start () {
        calendar = new DateTime(2016, 4, 20);
        timer = DAY_CYCLE;

        Text[] components = GetComponentsInChildren<Text>(); //get text components
        calendarDayText = components[0];
        calendarMonthText = components[1];
	}
    
    // Update is called once per frame
    void Update() {
        //every 10 seconds, change the day
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            calendar = calendar.AddDays(1.0);
            calendarDayText.text = calendar.Day.ToString(); //update calendar graphic
            calendarMonthText.text = new DateTimeFormatInfo().GetAbbreviatedMonthName(calendar.Month);
            timer = DAY_CYCLE;
        }
	}
}
