using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

    GameObject mainScreenObj;
    GameObject helpScreenObj;
    GameObject characterScreenObj;

    // Use this for initialization
    void Awake() {
        //get game objects
        mainScreenObj = transform.FindChild("MainScreen").gameObject;
        helpScreenObj = transform.FindChild("HelpScreen").gameObject;
        characterScreenObj = transform.FindChild("CharacterSelectionScreen").gameObject;
    }
	
	// Update is called once per frame
	void Update () {

    }

    //proceed to character selection scene
    public void PlayMain()
    {
        characterScreenObj.SetActive(true);
        mainScreenObj.SetActive(false);
        helpScreenObj.SetActive(false);
    }

    //proceed to help screen
    public void HelpMain()
    {
        helpScreenObj.SetActive(true);
        characterScreenObj.SetActive(false);
        mainScreenObj.SetActive(false);
    }

    //exit the game
    public void ExitGame()
    {
        Application.Quit();
    }

    //Deprecated
    //***************************
    public void SettingsMain()
    {

    }
    //***************************

    //proceed to main screen
    public void BackToMainMenu()
    {
        mainScreenObj.SetActive(true);
        helpScreenObj.SetActive(false);
        characterScreenObj.SetActive(false);
    }
}
