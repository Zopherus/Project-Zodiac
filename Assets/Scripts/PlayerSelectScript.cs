using UnityEngine;
using System.Collections;

public class PlayerSelectScript : MonoBehaviour {

    GameObject player;
    GameObject computer;
    public static string character;
    public static string aiChar;

	// Use this for initialization
	void Start () {

        player = GameObject.Find("Player");
        computer = GameObject.Find("Computer");

        MainTycoonScript.Character playerChar = MainTycoonScript.currentCharacter;

        switch (playerChar)
        {
            case MainTycoonScript.Character.Clinton:
                player.transform.FindChild("Clinton").gameObject.SetActive(true);
                character = "Clinton";
                computer.transform.FindChild("Trump").gameObject.SetActive(true);
                aiChar = "Trump";
                break;
            case MainTycoonScript.Character.Cruz:
                player.transform.FindChild("Zodiac").gameObject.SetActive(true);
                character = "Zodiac";
                computer.transform.FindChild("Bernie").gameObject.SetActive(true);
                aiChar = "Bernie";
                break;
            case MainTycoonScript.Character.Sanders:
                player.transform.FindChild("Bernie").gameObject.SetActive(true);
                character = "Bernie";
                computer.transform.FindChild("Zodiac").gameObject.SetActive(true);
                aiChar = "Zodiac";
                break;
            case MainTycoonScript.Character.Trump:
                player.transform.FindChild("Trump").gameObject.SetActive(true);
                character = "Trump";
                computer.transform.FindChild("Clinton").gameObject.SetActive(true);
                aiChar = "Clinton";
                break;
        }



    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
