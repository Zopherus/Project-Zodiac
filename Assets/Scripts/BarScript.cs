﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BarScript : MonoBehaviour {


	[SerializeField]
	private float fillAmount;

	[SerializeField]
	private Image Content;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		handleBars ();
	}

	private void handleBars()
	{
		Content.fillAmount = Map (180, 0, 230, 0, 1);
	}


	// Value is the amount of health that the user currently has
	// inMin is the minimum amount of the health the user can have
	// inMax is the maximum amount of the health the user can have
	private float Map(float value, float inMin, float inMax, float outMin, float outMax)
	{
		return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
		// (78 - 0) * (1 - 0) / (230 - 0) + 0
		// 78 * 1 / 230 = 0.339
	}
}