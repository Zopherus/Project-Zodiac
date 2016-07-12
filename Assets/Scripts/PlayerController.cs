using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	//Movement variables
	public float speed;
	public float jump;
	float moveVelocity;

	//Boolean to see if on the ground
	bool grounded = true;
	Animator animator;
	bool direction;
	bool moving;

    //Variables that restrict spamming of punching and shooting
    float punchCD;
    float reload;
    bool singleFire;


    //For quick access to the Player and Computer objects
    GameObject player;
    GameObject computer;

    void Start()
	{
		animator = GetComponent<Animator> ();
		direction = true;
        player = GameObject.Find("Player");
        computer = GameObject.Find("Computer");
    }

	void Update () 
	{

        //Up and down movement
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

			animator.SetTrigger ("movingStart");

			direction = false;
            animator.SetBool("direction", direction);

			moving = true;
			animator.SetBool ("moving", moving);
		}

		if (Input.GetKeyUp (KeyCode.LeftArrow) || Input.GetKeyUp (KeyCode.A)) 
		{
			moving = false;
			animator.SetBool ("moving", moving);
		}

		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			moveVelocity = speed;

			animator.SetTrigger ("movingStart");

			direction = true;
            animator.SetBool("direction", direction);

			moving = true;
			animator.SetBool ("moving", moving);
		}

		if (Input.GetKeyUp (KeyCode.RightArrow) || Input.GetKeyUp (KeyCode.D)) 
		{
			moving = false;
			animator.SetBool ("moving", moving);
		}

        //Shooting
        if (Input.GetKeyDown(KeyCode.O) & reload < 0 & grounded & direction)
        {
            animator.SetTrigger("shoot");
            GameObject projectile = Instantiate<GameObject>(transform.FindChild("Projectile").gameObject);
            projectile.SetActive(true);
            projectile.transform.position = transform.FindChild("spawnPoint").position;
            reload = .5f;
        }

        if (Input.GetKeyDown(KeyCode.O) & reload < 0 & grounded & !direction)
        {
            transform.FindChild("Projectile").GetComponent<BulletScript>().bulletSpeed *= -1;
            animator.SetTrigger("shoot");
            GameObject projectile = Instantiate<GameObject>(transform.FindChild("Projectile").gameObject);
            projectile.SetActive(true);
            projectile.transform.position = transform.FindChild("spawnPoint left").position;
            reload = .5f;
            transform.FindChild("Projectile").GetComponent<BulletScript>().bulletSpeed *= -1;
        }

        //Floats that determine the position of each character. Important to determining if a punch connects
        //or not.
        string activeChar = PlayerSelectScript.character;
        string compChar = PlayerSelectScript.aiChar;

        float playerX = player.transform.FindChild(activeChar).transform.position.x;
        float compX = computer.transform.FindChild(compChar).transform.position.x;

        //Punching
        if (Input.GetKeyDown(KeyCode.P) & direction & punchCD <= 0)
        {
            animator.SetTrigger("punch");
            if (Mathf.Abs(playerX - compX) < 10 & playerX < compX)
            {
                computer.transform.FindChild("Health Bar").GetComponent<Player>().popularity.CurrentVal -= 7;
                punchCD = .5f;
            }
        }

        if (Input.GetKeyDown(KeyCode.P) & !direction & punchCD >= 0)
        {
            animator.SetTrigger("punch");
            if (Mathf.Abs(playerX - compX) < 10 & playerX > compX)
            {
                computer.transform.FindChild("Health Bar").GetComponent<Player>().popularity.CurrentVal -= 7;
                punchCD = .5f;
            }
        }

        reload = reload - Time.deltaTime;
        punchCD = punchCD - Time.deltaTime;

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
