using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class AIControl : MonoBehaviour {
    public enum AIState { WalkForward, WalkBack, Punch, Shoot, Jump, None };
    private List<AIState> currentState = new List<AIState>(new AIState[] { AIState.None });

    //Movement variables
    public float speed;
    public float jump;
    float moveVelocity;

    const float PUNCH_DISTANCE = 5;

    //Boolean to see if on the ground
    bool grounded = true;
    Animator animator;
    bool direction; //positive is right
    bool moving;

    //Variables related to the shooting
    float timer = 3.0f;
    bool singleFire;

    float punchTimer = 0.5f;
    const float PUNCH_WAIT = 0.5f;

    float stateTimer = 1.3f;
    const float STATE_CHANGE_WAIT = 1.3f;

    GameObject player;
    GameObject computer;
    GameObject playerCharacter;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        direction = false;
        player = GameObject.Find("Player");
        computer = GameObject.Find("Computer");
        switch(MainTycoonScript.currentCharacter)
        {
            case MainTycoonScript.Character.Clinton:
                playerCharacter = player.transform.FindChild("Clinton").gameObject;
                break;
            case MainTycoonScript.Character.Sanders:
                playerCharacter = player.transform.FindChild("Bernie").gameObject;
                break;
            case MainTycoonScript.Character.Cruz:
                playerCharacter = player.transform.FindChild("Zodiac").gameObject;
                break;
            case MainTycoonScript.Character.Trump:
                playerCharacter = player.transform.FindChild("Trump").gameObject;
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
        #region Update CurrentState
        currentState.Clear();
        float distanceFromPlayer = Math.Abs(playerCharacter.transform.position.x - transform.position.x);
        //Debug.Log(distanceFromPlayer);

        if (playerCharacter.transform.position.x < transform.position.x & direction) { direction = false; }
        if (playerCharacter.transform.position.x > transform.position.x & !direction) { direction = true; }
        animator.SetBool("direction", direction);
        stateTimer -= Time.deltaTime; //update stateTimer
        if (distanceFromPlayer > 15) //keep player relatively close to player
        {
            currentState.Add(AIState.WalkForward);
        }
        if (stateTimer <= 0)
        {
            if (distanceFromPlayer <= PUNCH_DISTANCE) //punch the player
            {
                currentState.Add(AIState.Punch);
            }
            else
            {
                currentState.Add(AIState.Shoot);
            }
            stateTimer = STATE_CHANGE_WAIT;
        }
        #endregion

        #region Execute CurrentState actions
        if (currentState.Contains(AIState.Jump))
        {
            if (grounded & direction)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jump);
                animator.SetTrigger("jumpRight");
            }

            if (grounded & !direction)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jump);
                animator.SetTrigger("jumpLeft");
            }
        }

        animator.SetBool("onGround", grounded);

        moveVelocity = 0;

        //Left and right movement

        //left
        if ((currentState.Contains(AIState.WalkBack) && direction) || (currentState.Contains(AIState.WalkForward) && !direction))
        {
            moveVelocity = -speed;

            animator.SetTrigger("movingStart");

            direction = false;
            animator.SetBool("direction", direction);

            moving = true;
            animator.SetBool("moving", moving);
        }

        //right
        if ((currentState.Contains(AIState.WalkBack) && !direction) || (currentState.Contains(AIState.WalkForward) && direction))
        {
            moveVelocity = speed;

            animator.SetTrigger("movingStart");

            direction = true;
            animator.SetBool("direction", direction);

            moving = true;
            animator.SetBool("moving", moving);
        }

        else
        {
            moving = false;
            animator.SetBool("moving", moving);
        }


        //Testing to see if shooting works
        if (currentState.Contains(AIState.Shoot) & grounded & direction)
        {
            animator.SetTrigger("shoot");
            GameObject bacon = Instantiate<GameObject>(transform.FindChild("Projectile").gameObject);
            bacon.SetActive(true);
            bacon.transform.position = transform.FindChild("spawnPoint").position;
            transform.FindChild("shootSound").GetComponent<AudioSource>().Play();
        }

        if (currentState.Contains(AIState.Shoot) & grounded & !direction)
        {
            transform.FindChild("Projectile").GetComponent<BulletScript>().bulletSpeed *= -1;
            animator.SetTrigger("shoot");
            GameObject projectile = Instantiate<GameObject>(transform.FindChild("Projectile").gameObject);
            projectile.SetActive(true);
            projectile.transform.position = transform.FindChild("spawnPoint left").position;
            transform.FindChild("Projectile").GetComponent<BulletScript>().bulletSpeed *= -1;
            transform.FindChild("shootSound").GetComponent<AudioSource>().Play();
        }

        float playerX = playerCharacter.transform.position.x;
        float compX = transform.position.x;

        //Testing to see if punching works
        if (currentState.Contains(AIState.Punch) & direction)
        {
            animator.SetTrigger("punch");
            if (Mathf.Abs(playerX - compX) < PUNCH_DISTANCE & compX < playerX & punchTimer <= 0f)
            {
                player.transform.FindChild("Health Bar").GetComponent<Player>().popularity.CurrentVal -= 7;
                punchTimer = PUNCH_WAIT;
            }
        }

        if (currentState.Contains(AIState.Punch) & !direction)
        {
            animator.SetTrigger("punch");
            if (Mathf.Abs(playerX - compX) < PUNCH_DISTANCE & compX > playerX & punchTimer <= 0f)
            {
                player.transform.FindChild("Health Bar").GetComponent<Player>().popularity.CurrentVal -= 7;
                punchTimer = PUNCH_WAIT;
            }
        }
        
        punchTimer -= Time.deltaTime;

        GetComponent<Rigidbody2D>().velocity = new Vector2(moveVelocity, GetComponent<Rigidbody2D>().velocity.y);
        #endregion
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


