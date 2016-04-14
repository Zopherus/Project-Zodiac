using UnityEngine;
using System.Collections;

public class CharacterStats : MonoBehaviour {

    public int MaximumHealth { get; private set; }
    public int CurrentHealth { get; private set; }


	// Use this for initialization
	void Start ()
    {
        CurrentHealth = 50;
        MaximumHealth = 100;
	}

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
            return;    // Character is dead
    }
}
