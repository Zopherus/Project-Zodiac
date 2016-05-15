using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PolicyScript : MonoBehaviour {

    private const float maxDifference = 5f;
    private float medianValue;
    public float value;
    private Text text;

	// Use this for initialization
	void Start () {
        medianValue = 50f;
        value = 50f;
        text = transform.FindChild("Value").GetComponent<Text>();
	}

	// Update is called once per frame
	void Update () {
	
	}

    public void IncreaseValue()
    {
        //only increase if doing so won't make it go past maxDifference from median
        if(value + 1 <= medianValue + maxDifference)
        {
            value++;
        }
        UpdateValueText(value);
    }

    public void DecreaseValue()
    {
        //only increase if doing so won't make it go past maxDifference from median
        if (value - 1 >= medianValue - maxDifference)
        {
            value--;
        }
        UpdateValueText(value);
    }

    public void UpdateValueText(float newValue)
    {
        text.text = newValue.ToString();
    }
}
