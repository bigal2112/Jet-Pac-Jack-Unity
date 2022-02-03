using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour2 : MonoBehaviour
{
	public enum GroundContactAction { EXPLODE, BOUNCE }

	public float speed = 2f;
	public int waitBeforeAttack = 3;
	public bool mirrorImage;
	public GroundContactAction groundContactAction = GroundContactAction.BOUNCE;
	public int scoreValue = 45;

	private Animator anim;
	private Rigidbody2D rb;


	private bool dying = false;
	private bool inTheRespawnBubble;

	public GameObject explosion;

	public float timeBeforeDirectionChange;
	public float directionChangeDelayLowerLimit = 0.5f;
	public float directionChangeDelayUpperLimit = 2.0f;
	public int currentYDirection;       //	=> -1, 0, 1

	void Start()
	{

		//	get the component we need
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		inTheRespawnBubble = false;

		//	set the initial direction change delay
		UpdateDirectionChangeDelay();

		//  if we've been spawned on the left-hand side then birl us around so we're pointing the right way
		//	and set us off in a left-to-right direction by using a +ve value for the x
		if (transform.position.x < 0)
		{
			if (!mirrorImage)
				transform.rotation = Quaternion.Euler(0f, 180f, 0f);

			// Calculate a random y direction value between -1 and 1 and set our enemy on its merry way. This will give us a 45, 0 or 315 degree angle on the x axis.
			currentYDirection = Random.Range(-1, 2);
			rb.velocity = new Vector2(1, currentYDirection) * speed;
		}
		else
		{
			// Calculate a random y direction between -1 and 1 and set our enemy on its merry way but in a right-to-left direction by using a -ve value for the x
			currentYDirection = Random.Range(-1, 2);
			rb.velocity = new Vector2(-1, currentYDirection) * speed;
		}

		// freeze the rotation so we doesnt go spinning after a collision
		rb.freezeRotation = true;
	}


	private void FixedUpdate()
	{
		// if we're dying then stop us moving.
		if (dying)
			rb.velocity = Vector2.zero;
		else
		{
			if (timeBeforeDirectionChange <= 0)
			{
				//	change direction
				ChangeDirection();

				//	reset the direction change delay
				UpdateDirectionChangeDelay();
			}
			else
			{
				timeBeforeDirectionChange -= Time.deltaTime;
			}
		}
	}

	private void UpdateDirectionChangeDelay()
	{
		timeBeforeDirectionChange = Random.Range(directionChangeDelayLowerLimit, directionChangeDelayUpperLimit);
	}



	private void ChangeDirection()
	{

		//	get a new direciton index that is not the same as the current one
		int newYDirection = currentYDirection;
		while (newYDirection == currentYDirection)
		{
			newYDirection = Random.Range(-1, 2);
		}

		//	update the currentDirection and alter our course
		currentYDirection = newYDirection;
		rb.velocity = new Vector2(rb.velocity.x, currentYDirection);

	}


	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (!dying)
		{
			if (collider.CompareTag("Ground") || collider.CompareTag("Ceiling") || collider.CompareTag("Edge"))
			{
				// Debug.Log("Hit the " + collider.name + " - " + groundContactAction);

				if (groundContactAction == GroundContactAction.EXPLODE)
				{
					//	if we've killed the player and we're in the respawn bubble then reduce the
					//	count by 1 as we're just about to explode.....
					if (inTheRespawnBubble) GameMaster.EnemiesInRespawnBubble--;

					dying = true;

					GameObject newExplosion = Instantiate(explosion, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
					AudioSource noise = newExplosion.GetComponent<AudioSource>();
					noise.Play();
					Destroy(gameObject);
					Destroy(newExplosion, 1.0f);
				}
				else if (groundContactAction == GroundContactAction.BOUNCE)
				{

					if (collider.CompareTag("Ground") || collider.CompareTag("Ceiling"))

						// 	if we've hit the ceiling, the ground or the top or bottom of a platform then
						//	bounce of at 90 degrees to our incoming angle on the x axis
						rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * -1);
					else
						// 	if we've hit the left or righthand sides of a platform then
						//	bounce of at 90 degrees to our incoming angle on the y axis
						rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);

					//	reset the direction change delay
					UpdateDirectionChangeDelay();

				}

			}
		}

		if (collider.CompareTag("Bullet"))
		{
			if (!dying)
			{
				dying = true;

				GameObject newExplosion = Instantiate(explosion, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
				AudioSource noise = newExplosion.GetComponent<AudioSource>();
				noise.Play();
				Destroy(gameObject);
				Destroy(newExplosion, 1.0f);
				GameMaster.IncrementPlayer1Score(scoreValue);
			}
		}


		if (collider.CompareTag("Player"))
		{
			if (!dying)
			{
				dying = true;

				GameObject newExplosion = Instantiate(explosion, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
				AudioSource noise = newExplosion.GetComponent<AudioSource>();
				noise.Play();
				Destroy(gameObject);
				Destroy(newExplosion, 1.0f);
			}
		}

		if (collider.CompareTag("Respawn"))
		{
			inTheRespawnBubble = true;
			GameMaster.EnemiesInRespawnBubble++;
		}
	}


	private void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.CompareTag("Respawn"))
		{
			inTheRespawnBubble = false;
			GameMaster.EnemiesInRespawnBubble--;
		}
	}


}   //  class
