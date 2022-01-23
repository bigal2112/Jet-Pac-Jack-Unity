using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

	//  the information needed for the laser to fire from the correct place
	public Transform firePoint;
	public LineRenderer lineRenderer;

	//  how quick the spaceman move from dsdie to side and how quickly he flies up
	[SerializeField] private float moveForce;
	[SerializeField] private float jumpForce;

	//  components needed in script to move and animate the spaceman
	private Rigidbody2D myBody;
	private Animator anim;

	//  animation setting
	private string IDLE_ANIMATION = "Idle";
	private string WALK_ANIMATION = "Walking";
	private string FLYING_ANIMATION = "Flying";

	private bool facingRight;                             //  is the spaceman facing right?
	private bool isGrounded;                              //  is the spaceman on the ground?

	private float moveHorizontal;                         //  used to hold the horizontal movement value if keys have been pressed
	private float moveVertical;                           //  used to hold the vertical movement value if keys have been pressed
	private float delayBeforePlayerCanMove;               //  how long must the spaceman be facing the correct way before he can move
	private float playerCanMoveCountdown;                 //  the countdown from when the spaceman last turned

	public float laserDistance = 5f;                      //  how far the spaceman can shoot and hit something

	public string pickedUpItemName;
	public string nextAllowedPickUpItemName;

	private bool playerIsDying;

	private LevelController levelController;


	private void Awake()
	{
		//  get the required components
		myBody = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}

	void Start()
	{

		levelController = LevelController.Instance;
		if (levelController == null)
		{
			Debug.LogError("No levelController found");
		}

		//  set our variables
		moveForce = 5.0f;
		jumpForce = 0.3f;
		isGrounded = true;
		facingRight = true;
		delayBeforePlayerCanMove = 0.13f;
		playerCanMoveCountdown = delayBeforePlayerCanMove;

		//  set the initial animation
		anim.SetBool(IDLE_ANIMATION, true);
		anim.SetBool(WALK_ANIMATION, false);
		anim.SetBool(FLYING_ANIMATION, false);

		nextAllowedPickUpItemName = "Part2PickupPoint";
		playerIsDying = false;

	}

	void Update()
	{
		// check for keyboard interation
		moveHorizontal = Input.GetAxisRaw("Horizontal");
		moveVertical = Input.GetAxisRaw("Vertical");

		//  decremement the timer that allows the player to move after turning
		playerCanMoveCountdown -= Time.deltaTime;
	}



	private void FixedUpdate()
	{
		if (!playerIsDying)
		{
			//  check whether we need to move the spaceman or make him shoot
			AnimatePlayer();
			if (Input.GetButtonDown("Fire1"))
			{
				StartCoroutine(PlayerShoot());
			}
		}
	}



	void AnimatePlayer()
	{
		//  if user has pressed the left or right key
		if (moveHorizontal > 0.1f || moveHorizontal < -0.1f)
		{

			//  if player is facing left but the right move key has been pressed the flip him and set the flag - DO NOT MOVE HIM
			if (!facingRight && moveHorizontal > 0.1f)
			{
				transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				facingRight = true;
				playerCanMoveCountdown = delayBeforePlayerCanMove;
			}

			//  else if player is facing right but the left move key has been pressed the flip him and set the flag - DO NOT MOVE HIM
			else if (facingRight && moveHorizontal < -0.1f)
			{
				transform.rotation = Quaternion.Euler(0f, 180f, 0f);
				facingRight = false;
				playerCanMoveCountdown = delayBeforePlayerCanMove;
			}

			//  else if the the player of facing right and the right key was pressed OR the player is facing left and the left
			//  key was pressed then start moving him and set the walking animation to play if he's on the ground
			else if ((facingRight && moveHorizontal > 0.1f) || (!facingRight && moveHorizontal < -0.1f))
			{

				if (playerCanMoveCountdown <= 0)
				{
					transform.position += new Vector3(moveHorizontal, 0f, 0f) * Time.deltaTime * moveForce;

					// set the animation to walk if player is grounded
					if (isGrounded)
					{
						anim.SetBool(IDLE_ANIMATION, false);
						anim.SetBool(WALK_ANIMATION, true);
						anim.SetBool(FLYING_ANIMATION, false);
					}
				}

			}

		}

		//  if player is jumping/flying
		if (moveVertical > 0.1f)
		{
			myBody.AddForce(new Vector2(0f, moveVertical * jumpForce), ForceMode2D.Impulse);

			anim.SetBool(IDLE_ANIMATION, false);
			anim.SetBool(WALK_ANIMATION, false);
			anim.SetBool(FLYING_ANIMATION, true);
		}

		//  if player is idle and grounded
		if (moveVertical == 0 && moveHorizontal == 0 && isGrounded)
		{
			anim.SetBool(IDLE_ANIMATION, true);
			anim.SetBool(WALK_ANIMATION, false);
			anim.SetBool(FLYING_ANIMATION, false);
			transform.position += new Vector3(0f, 0f, 0f);
		}

		//  if player is idle and in the air
		if (moveVertical == 0 && moveHorizontal == 0 && !isGrounded)
		{
			anim.SetBool(IDLE_ANIMATION, false);
			anim.SetBool(WALK_ANIMATION, false);
			anim.SetBool(FLYING_ANIMATION, true);
		}

	}



	IEnumerator PlayerShoot()
	{

		RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right, laserDistance);

		if (hitInfo)
		{
			// Debug.Log("We hit something: " + hitInfo.transform.name);
			lineRenderer.SetPosition(0, firePoint.position);
			lineRenderer.SetPosition(1, hitInfo.point);

		}
		else
		{
			lineRenderer.SetPosition(0, firePoint.position);
			lineRenderer.SetPosition(1, firePoint.position + firePoint.right * laserDistance);
		}

		lineRenderer.enabled = true;
		yield return new WaitForSeconds(0.02f);
		lineRenderer.enabled = false;
	}



	//  if the box collider has hit something 
	private void OnTriggerEnter2D(Collider2D collider)
	{

		if (collider.gameObject.tag == "LiftOffEntrance")
		{
			// Debug.Log("SPACEMAN INSIDE");
			LevelController.SpacemanInside = true;
			Destroy(gameObject, 0.01f);
		}


		//  we're on the gound so set the flag
		if (collider.tag == "Ground")
			isGrounded = true;



		//  if we've been hit by an enemy
		if (collider.tag == "Enemy")
		{
			if (!playerIsDying)
			{

				//  decouple any spaceship parts or fuel cells that we were carrying
				foreach (Transform child in transform)
				{

					if (child.tag == "SpaceshipPart1" || child.tag == "SpaceshipPart2")
					{
						Debug.Log("Decoupling " + child.tag + " from player to " + levelController.spaceship.name);
						child.parent = levelController.spaceship.transform;
					}

					if (child.tag == "Collectable")
					{
						Debug.Log("Decoupling " + child.name + " from player");
						child.parent = null;
					}
				}


				playerIsDying = true;
				GameMaster.DecrementPlayersLives();

				//  stop the player moving and run the explosion animation
				Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
				if (rb != null)
				{

					rb.bodyType = RigidbodyType2D.Kinematic;
					rb.velocity = Vector3.zero;
				}

				//  Run the killed animation it is exists on the Player
				Animator playerAnim = gameObject.GetComponent<Animator>();
				if (playerAnim != null)
					playerAnim.SetBool("Killed", true);

				//  remove the player object after a second
				Debug.Log("Destroying gameObject:" + gameObject.name + " in 3 seconds");
				Destroy(gameObject, 3.0f);
			}
		}
	}




	private void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.tag == "Ground")
			isGrounded = false;
	}

	IEnumerator EnableTrigger(BoxCollider2D bc2D)
	{
		yield return new WaitForSeconds(0.1f);
		bc2D.isTrigger = false;
	}

	private void OnDestroy()
	{
		Debug.Log("Player OnDestroy");
	}

}

