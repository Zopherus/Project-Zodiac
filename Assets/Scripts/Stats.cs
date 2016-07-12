using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Stats {

    [SerializeField]
    private BarScript bar;

    [SerializeField]
    private float maxVal;

    [SerializeField]
    private float currentVal;

    public float CurrentVal
    {
        get
        {
            return currentVal;
        }

        set
        {
            this.currentVal = Mathf.Clamp(value, 0, MaxVal);
            bar.Value = currentVal;
        }
    }

    public float MaxVal
    {
        get
        {
            return maxVal;
        }

        set
        {
            this.maxVal = value;
            bar.MaxValue = maxVal;
        }
    }

    public void Initialize(string parentName)
    {
        this.MaxVal = maxVal;
        float scaledPopularity = PopularityManager.popularity / 2f;
        if (parentName == "Computer")
        {
            this.CurrentVal = (50f - scaledPopularity) + 50f; //computer's health bar (inverse of player's)
        }
        else
        {
            this.CurrentVal = scaledPopularity + 50f; //player's health bar
        }
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
