using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {

    public enum EnemyState { WalkFoward, WalkBackward, Jump, Punch, Kick, SwitchDirection, Super, None }
    EnemyState state;

	// Use this for initialization
	void Start () {
        state = EnemyState.None;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    /// <summary>
    /// Update the EnemyState
    /// </summary>
    void UpdateState()
    {

    }

    void ExecuteAction()
    {
        switch(state)
        {
            case EnemyState.WalkBackward:
                break;
            case EnemyState.WalkFoward:
                break;
            case EnemyState.SwitchDirection:
                break;
            case EnemyState.Punch:
                break;
            case EnemyState.Kick:
                break;
            case EnemyState.Jump:
                break;
            case EnemyState.Super:
                break;
        }
    }
}
