using UnityEngine;
using System.Collections;

public class MusicScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject); //don't destroy this object (so that music plays throuhgout)
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnLevelWasLoaded(int level)
    {
        //if this level was loaded, destroy itself (prevent double music when returning to title screen)
        if (level == 0)
        {
            Instantiate(this.gameObject); //don't worry, it makes sense. Simon knows...
            Destroy(this.gameObject);
        }
    }
}
