using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour {

    float timeLeft; //Used to determine the amount of time remaining in the battle
    string timerText; //Used to change the text of the timer in the scene
    Text clock; //Child timer object
	// Use this for initialization
	void Start () {
        timeLeft = 91.0f;
        clock = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        timeLeft -= Time.deltaTime;

        timerText = timeLeft.ToString();
        clock.text = timerText;
	}
}
