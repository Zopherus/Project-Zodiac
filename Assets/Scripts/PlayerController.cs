using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	private GameObject panel;

    //Movement variables
    public float speed;
    public float jump;
	float moveVelocity;

    //Boolean to see if on the ground
    bool grounded;

    public const float HealthBarOffsetHeight = 1.5f;  // The height that the health bar is above the character

	void Start()
	{
		panel = GameObject.Find ("Panel");
        grounded = false;
	}

	void Update () 
	{
        float percentageHealth = (float)GetComponent<CharacterStats>().CurrentHealth / (float)GetComponent<CharacterStats>().MaximumHealth;

        panel.GetComponent<Image>().fillAmount = percentageHealth;
		RectTransform panelTransform = panel.GetComponent<RectTransform> ();

        Vector3[] corners = new Vector3[4];

        panelTransform.GetWorldCorners(corners);


        Vector3 vector = new Vector3(GetComponent<RectTransform>().position.x + (GetComponent<RectTransform>().rect.width / 2)  - (corners[2].x - corners[0].x)/2, transform.position.y + HealthBarOffsetHeight);




        panelTransform.position = vector;



		if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.W)) 
		{
			if(grounded)
			{
				GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jump);
			}
		}

		//Left and right movement

		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, GetComponent<Rigidbody2D>().velocity.y);
        }

		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(speed, GetComponent<Rigidbody2D>().velocity.y);
        }
	}

	//Method to check if character is on the ground

	void OnCollisionEnter2D(Collision2D other)
	{
        if (other.gameObject  == GameObject.Find("Ground"))
		    grounded = true;
	}

	void OnCollisionExit2D(Collision2D other)
	{
        if (other.gameObject == GameObject.Find("Ground"))
            grounded = false;
    }
}
