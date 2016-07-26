using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private const float RELOADTIME = 0.5f;
    private const float PUNCHCD = 0.5f;
    private const int PUNCHDAMAGE = 7;




	//Movement variables
	public float speed;
	public float jump;
	private float moveVelocity;

	//Boolean to see if on the ground
	private bool grounded = true;
	private Animator animator;

    // True if right, false if left
	public bool direction = true;
	private bool moving;

    // Restricts the left, right movement if is next to a border
    private bool canMoveLeft = true;
    private bool canMoveRight = true;


    //Variables that restrict spamming of punching and shooting
    private float punchCD;
    private float reload;
    private bool singleFire;


    //For quick access to the Player and Computer objects
    private GameObject player;
    private GameObject computer;

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
        animator.SetBool("direction", direction);
		moveVelocity = 0;

		//Left and right movement
		if (canMoveLeft && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) ))
		{
			moveVelocity = -speed;

			animator.SetTrigger ("movingStart");

			direction = false;
            animator.SetBool("direction", direction);

			moving = true;
			animator.SetBool ("moving", moving);
		}

		else if (Input.GetKeyUp (KeyCode.LeftArrow) || Input.GetKeyUp (KeyCode.A)) 
		{
			moving = false;
			animator.SetBool ("moving", moving);
		}

		if (canMoveRight && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)))
		{
			moveVelocity = speed;

			animator.SetTrigger ("movingStart");

			direction = true;
            animator.SetBool("direction", direction);

			moving = true;
			animator.SetBool ("moving", moving);
		}

		else if (Input.GetKeyUp (KeyCode.RightArrow) || Input.GetKeyUp (KeyCode.D)) 
		{
			moving = false;
			animator.SetBool ("moving", moving);
		}

        //Shooting
        if (Input.GetKeyDown(KeyCode.O) & reload < 0 & grounded)
        {

            animator.SetTrigger("shoot");
            GameObject projectile = Instantiate<GameObject>(transform.FindChild("Projectile").gameObject);
            projectile.SetActive(true);
            if (direction)
                projectile.transform.position = transform.FindChild("spawnPoint").position;
            else
            {
                projectile.transform.position = transform.FindChild("spawnPoint left").position;
                projectile.GetComponent<BulletScript>().bulletSpeed *= -1;
            }
            reload = RELOADTIME;
            transform.FindChild("shootSound").GetComponent<AudioSource>().Play();
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
                computer.transform.FindChild("Health Bar").GetComponent<Player>().popularity.CurrentVal -= PUNCHDAMAGE;
                punchCD = PUNCHCD;
            }
        }

        if (Input.GetKeyDown(KeyCode.P) & !direction & punchCD <= 0)
        {
            animator.SetTrigger("punch");
            if (Mathf.Abs(playerX - compX) < 10 & playerX > compX)
            {
                computer.transform.FindChild("Health Bar").GetComponent<Player>().popularity.CurrentVal -= PUNCHDAMAGE;
                punchCD = PUNCHCD;
            }
        }

        reload = reload - Time.deltaTime;
        punchCD = punchCD - Time.deltaTime;

        GetComponent<Rigidbody2D>().velocity = new Vector2(moveVelocity, GetComponent<Rigidbody2D>().velocity.y);
	}

	//Method to check if character is on the ground
	void OnTriggerEnter2D(Collider2D other)
	{
        if (other.ToString() == "Ground (UnityEngine.BoxCollider2D)")
        {
            grounded = true;
        }
        else if (other.ToString() == "Left Border (UnityEngine.EdgeCollider2D)")
        {
            canMoveLeft = false;
        }
        else if (other.ToString() == "Right Border (UnityEngine.EdgeCollider2D)")
            canMoveRight = false;
	}

	void OnTriggerExit2D(Collider2D other)
	{
        if (other.ToString() == "Ground (UnityEngine.BoxCollider2D)")
		    grounded = false;
        else if (other.ToString() == "Left Border (UnityEngine.EdgeCollider2D)")
        {
            canMoveLeft = true;
        }
        else if (other.ToString() == "Right Border (UnityEngine.EdgeCollider2D)")
            canMoveRight = true;
	}
}
