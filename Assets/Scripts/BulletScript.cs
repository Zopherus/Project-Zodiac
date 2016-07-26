using UnityEngine;
using System.Collections;
using System;

public class BulletScript : MonoBehaviour {

    public float bulletSpeed;
    public float timer;

	// Use this for initialization
	void Start () 
    {
        Vector2 speed = new Vector2(bulletSpeed, 0);
        Rigidbody2D bullet = GetComponent<Rigidbody2D>();
        bullet.velocity = speed;
        timer = 91.0f;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(this.gameObject.activeSelf)
            timer -= Time.deltaTime;

        if (timer < 0)
            DestroyObject(this.gameObject);
    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.ToString().Split(' ')[1] == "Border")
        {
            DestroyObject(this.gameObject);
            return;
        }
        try
        {
            string name = other.transform.parent.name;
            Player computer = GameObject.Find(name).transform.FindChild("Health Bar").GetComponent<Player>();
            computer.popularity.CurrentVal -= 5;
            DestroyObject(this.gameObject);
        }
        catch { }
    }
    
}
