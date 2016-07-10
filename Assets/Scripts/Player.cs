using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    SceneManager sm = new SceneManager();
    
    [SerializeField]
    public Stats popularity;

    public void Awake()
    {
        popularity.Initialize();
    }
	// Update is called once per frame
	void Update ()
    {
        if(popularity.CurrentVal <= 0)
        {
            SceneManager.LoadScene(1);
        }
    }
}
