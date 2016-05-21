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
	Animator animator;
	bool direction;
	bool moving;

	void Start()
	{
		animator = GetComponent<Animator> ();
		direction = true;
	}

	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.W)) 
		{
			if(grounded & direction)
			{
				GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jump);
				animator.SetTrigger ("jumpRight");
			}

			if (grounded & !direction)
			{
				GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jump);
				animator.SetTrigger ("jumpLeft");
			}
		}
			
		animator.SetBool ("onGround", grounded);

		moveVelocity = 0;

		//Left and right movement

		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			moveVelocity = -speed;

			animator.SetTrigger ("faceLeft");
			animator.SetTrigger ("movingStart");

			moving = true;
			animator.SetBool ("moving", moving);

			direction = false;
		}

		if (Input.GetKeyUp (KeyCode.LeftArrow) || Input.GetKeyUp (KeyCode.A)) 
		{
			moving = false;
			animator.SetBool ("moving", moving);
		}

		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			moveVelocity = speed;

			animator.SetTrigger ("faceRight");
			animator.SetTrigger ("movingStart");

			moving = true;
			animator.SetBool ("moving", moving);

			direction = true;
		}

		if (Input.GetKeyUp (KeyCode.RightArrow) || Input.GetKeyUp (KeyCode.D)) 
		{
			moving = false;
			animator.SetBool ("moving", moving);
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
