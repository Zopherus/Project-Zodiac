using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
    
    [SerializeField]
    public Stats popularity;

    public void Awake()
    {
        popularity.Initialize(transform.parent.name);
        //GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().Stop();
    }

	// Update is called once per frame
	void Update ()
    {
        if(popularity.CurrentVal <= 0)
        {
            MainTycoonScript.states[PopularityManager.currentState].fightFinished = true;
            if (transform.parent.name == "Computer") //if computer lost
            {
                MainTycoonScript.states[PopularityManager.currentState].won = true;
                MainTycoonScript.events.delegates += MainTycoonScript.states[PopularityManager.currentState].numDelegates;
            }
            else  //if player lost
            {
                MainTycoonScript.states[PopularityManager.currentState].won = false;
            }
            SceneManager.LoadScene("Scenes/TycoonScene");
        }
    }
}
