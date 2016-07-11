using UnityEngine;
using System.Collections;

public class PlayerSelectScript : MonoBehaviour {

    GameObject player;
    GameObject computer;
    public static string character;

	// Use this for initialization
	void Start () {

        player = GameObject.Find("Player");
        computer = GameObject.Find("Computer");

        MainTycoonScript.Character playerChar = MainTycoonScript.currentCharacter;
        Debug.Log(playerChar.ToString());

        switch (playerChar)
        {
            case MainTycoonScript.Character.Clinton:
                player.transform.FindChild("Clinton").gameObject.SetActive(true);
                character = "Clinton";
                break;
            case MainTycoonScript.Character.Cruz:
                player.transform.FindChild("Zodiac").gameObject.SetActive(true);
                character = "Zodiac";
                break;
            case MainTycoonScript.Character.Sanders:
                player.transform.FindChild("Bernie").gameObject.SetActive(true);
                character = "Bernie";
                break;
            case MainTycoonScript.Character.Trump:
                player.transform.FindChild("Trump").gameObject.SetActive(true);
                character = "Trump";
                break;
        }



    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
