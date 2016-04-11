using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private GameObject sprite;

	//Movement variables
	public float speed;
	public float jump;
	float moveVelocity;

	//Boolean to see if on the ground
	bool grounded = true;

	void Start()
	{
		sprite = GameObject.Find ("Panel");
	}

	void Update () 
	{
		RectTransform transform = sprite.GetComponent<RectTransform> ();
	
		transform.position = this.GetComponent<RectTransform> ().position;
		if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.W)) 
		{
			if(grounded)
			{
				GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jump);
			}
		}

		moveVelocity = 0;

		//Left and right movement

		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			moveVelocity = -speed;
		}

		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			moveVelocity = speed;
		}

		GetComponent<Rigidbody2D> ().velocity = new Vector2 (moveVelocity, GetComponent<Rigidbody2D> ().velocity.y);
	}

	//Method to check if character is on the ground

	void OnTriggerEnter2D()
	{
		grounded = true;
	}

	void OnTriggerExit2D()
	{
		grounded = false;
	}
}
