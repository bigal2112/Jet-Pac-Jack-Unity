using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScreenWrapper))]
public class EnemyBehaviour3 : MonoBehaviour
{
	public enum GroundContactAction { EXPLODE, BOUNCE }

	public float speed = 200f;
	public bool mirrorImage;
	public GroundContactAction groundContactAction = GroundContactAction.BOUNCE;
	public int scoreValue = 45;

	private Rigidbody2D rb;


	private bool dying = false;
	public bool flying = false;
	private bool youDancing;

	private bool inTheRespawnBubble;

	public GameObject explosion;

	public float waitBeforeAttack;
	public float attackCountdown;

	public bool flyingRight;
	private GameObject target;               //  the player
	public Vector2 direction;
	public Vector2 force;

	private ScreenWrapper scrWrap;

	void Start()
	{

		//	get the component we need
		rb = GetComponent<Rigidbody2D>();

		// freeze the rotation so we doesnt go spinning after a collision
		rb.freezeRotation = true;

		inTheRespawnBubble = false;


		//	if we've been spawned on the left of the screen, regardless of whether we're flying left or right, we're in a waiting state before we can fly
		//	howevber if we've spawned on the right of the screen then we're a left flyer that's wrapped and need to be in a flying state with no position change
		if (transform.position.x < 0)
		{

			//  As all these enemies are originally spawned on the left had side of the screen, but can move both left and right, we need to randlomly birl a 
			//	couple around.  Well weight it more towards going right though.
			int goingLeft = Random.Range(1, 11);

			if (goingLeft > 2)
			{
				transform.rotation = Quaternion.Euler(0f, 180f, 0f);
				flyingRight = false;
				GetComponent<ScreenWrapper>().goingLeft = true;
			}
			else
			{
				flyingRight = true;
			}



			//  move the enemy onto the screen
			transform.position = new Vector3(transform.position.x + 1.2f, transform.position.y, 0);

			//  pause before attacking
			attackCountdown = waitBeforeAttack;
			flying = false;
		}
		else
		{
			//	we're flying
			flying = true;
		}


	}



	private void FixedUpdate()
	{
		//	if there's no Player object in the scene see if we can find one, they're probably respawning
		if (target == null)
		{
			target = GameObject.FindGameObjectWithTag("Player");
			return;
		}


		// if we're dying then stop us moving and that's that.
		if (dying)
		{
			rb.velocity = Vector2.zero;
			return;
		}

		//	if we're flying then update our direction based on where the Player is
		if (flying)
		{
			UpdateEnemyDirection();
			return;
		}

		//  if we're ready to attack then start moving otherwise keep waiting......
		if (attackCountdown <= 0)
		{
			flying = true;
			GetComponentInChildren<Animator>().enabled = false;
		}

		else
			attackCountdown -= Time.deltaTime;

	}




	//  
	private void UpdateEnemyDirection()
	{

		//  if the player is in front of us then update our direction towards him
		//  if the player if behind us then continue on our current path

		if (flyingRight)                                                                  //  we're flying right
		{
			if (target.transform.position.x > transform.position.x)                         //	and the player is to the right of us
			{
				direction = (target.transform.position - transform.position).normalized;      //	find the shortest direction from us to the Player
				force = direction * speed * Time.deltaTime;                                   //	and calculate the force to get us there

				//	we need to clamp the x force to a max of 4 so we keep flying in 
				//	a horizontal direction and don't slow down.
				force.x = Mathf.Max(force.x, 4f);
			}
		}
		else                                                                                //	we're flying left
		{
			//	when flying left we start on the righthand side then move left, off the 
			//	screen, and back onto the screen on the righthand side. So to begin with
			//	if our x pos is < -8.29 (0.01 more [to the right] of the the spawn point)
			//	then just fly straight otherwise track the target
			if (transform.position.x <= -8.29)
			{                                                                                   //	we're still on the lefthand side of the screen
				force = new Vector2(-4, 0);                                                       //	simply send us straight left at -4ms.
																																													// Debug.Log("Setting force to (-4, 0)");
			}
			else
			{
				// Debug.Log("In here");
				if (target.transform.position.x < transform.position.x)                         //	and the player is to the left of us
				{
					// Debug.Log("Target is to the left");
					direction = (target.transform.position - transform.position).normalized;      //	find the shortest direction from us to the Player
					force = direction * speed * Time.deltaTime;                                   //	and calculate the force to get us there
																																												// Debug.Log("Setting force direction * speed * Time.deltaTime;");

					//	we need to clamp the x force to a min of -4 so we keep flying in 
					//	a horizontal direction and don't slow down.
					force.x = Mathf.Min(force.x, -4f);
				}
			}

			//	if we're got to the middle of the screen then untag us so we will get killed
			//	when we reach the other side rather than get respawned and wrapped 
			if (transform.position.x > -1 && transform.position.x < 1)
				gameObject.tag = "Untagged";

		}

		//	we need to clamp the y force to a max of 1.5/-1.5 so we don't fly completely vertically.
		if (force.y > 1.5) force.y = 1.5f;
		if (force.y < -1.5) force.y = -1.5f;

		//	we can now apply the force to our rigid body
		rb.velocity = force;
	}



	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (!dying)
		{
			if (collider.CompareTag("Ground") || collider.CompareTag("Ceiling") || collider.CompareTag("Edge"))
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
