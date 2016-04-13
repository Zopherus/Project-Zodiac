using UnityEngine;
using System.Collections;

public class ReturnToMenuScript : MonoBehaviour {

    //Switch Scene to MainMenuScene
    public void ExitToMenu()
    {
        Application.LoadLevel("MainMenuScene");
    }
}
