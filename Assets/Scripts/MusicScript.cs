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
}
