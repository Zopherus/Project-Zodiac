using UnityEngine;
using UnityEngine.SceneManagement;
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

        mainScreenObj.SetActive(true);
        helpScreenObj.SetActive(false);
        characterScreenObj.SetActive(false);
    }

    void OnLevelWasLoaded(int level)
    {
        //if this level was loaded, initialize screen (start at Main Menu Screen)
        if (level == 0)
        {
            mainScreenObj.SetActive(true);
            helpScreenObj.SetActive(false);
            characterScreenObj.SetActive(false);
        }
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

    public void ChooseCharacter(string character)
    {
        switch (character)
        {
            case "Trump":
                MainTycoonScript.currentCharacter = MainTycoonScript.Character.Trump;
                SceneManager.LoadScene("TycoonScene");
                break;
            case "Cruz":
                MainTycoonScript.currentCharacter = MainTycoonScript.Character.Cruz;
                SceneManager.LoadScene("TycoonScene");
                break;
            case "Sanders":
                MainTycoonScript.currentCharacter = MainTycoonScript.Character.Sanders;
                SceneManager.LoadScene("TycoonScene");
                break;
            case "Clinton":
                MainTycoonScript.currentCharacter = MainTycoonScript.Character.Clinton;
                SceneManager.LoadScene("TycoonScene");
                break;
        }
    }
}
