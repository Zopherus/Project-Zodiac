using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour {

    float timeLeft; //Used to determine the amount of time remaining in the battle
    string timerText; //Used to change the text of the timer in the scene
    Text clock; //Child timer object
    bool win;

	// Use this for initialization
	void Start () {
        timeLeft = 99f;
        clock = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        timeLeft -= Time.deltaTime;

        timerText = timeLeft.ToString();
        clock.text = timerText;

        if (timeLeft < 1)
        {
            if (GameObject.Find("Player").GetComponentInChildren<Player>().popularity.CurrentVal >= GameObject.Find("Computer")
                .GetComponentInChildren<Player>().popularity.CurrentVal)
            {
                win = true;
            }
            else
            {
                win = false;
            }
            SceneManager.LoadScene("Scenes/TycoonScene");
        }
	}
}
