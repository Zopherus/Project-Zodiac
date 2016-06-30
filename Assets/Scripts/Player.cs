using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour {
    
    [SerializeField]
    private Stats popularity;

    public void Awake()
    {
        popularity.Initialize();
    }
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            popularity.CurrentVal -= 10;
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            popularity.CurrentVal += 10;
        }

    }
}
