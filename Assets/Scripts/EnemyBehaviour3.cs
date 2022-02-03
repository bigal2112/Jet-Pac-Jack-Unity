using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour3 : MonoBehaviour
{
	public enum GroundContactAction { EXPLODE, BOUNCE }

	public float speed = 200f;
	public bool mirrorImage;
	public GroundContactAction groundContactAction = GroundContactAction.BOUNCE;
	public int scoreValue = 45;

	private Rigidbody2D rb;


	private bool dying = false;
	private bool flying = false;
	private bool youDancing;

	private bool inTheRespawnBubble;

	public GameObject explosion;

	public float waitBeforeAttack;
	public float attackCountdown;

	public float currentX;
	private GameObject target;               //  the player
	public Vector2 direction;
	public Vector2 force;



	void Start()
	{

		//	get the component we need
		rb = GetComponent<Rigidbody2D>();

		inTheRespawnBubble = false;

		//  As all these enemies are spawned on the left had side of the screen, but can move both left and right, we need to randlomly birl a couple around
		//  Well weight it more towards going right though.
		int goingLeft = Random.Range(1, 11);

		if (goingLeft > 7)
		{
			transform.rotation = Quaternion.Euler(0f, 180f, 0f);
			currentX = -1;
		}
		else
		{
			currentX = 1;
		}

		// freeze the rotation so we doesnt go spinning after a collision
		rb.freezeRotation = true;

		//  finally move the enemy onto the screen
		transform.position = new Vector3(transform.position.x + 1.2f, transform.position.y, 0);

		//  pause before attacking
		attackCountdown = waitBeforeAttack;


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

		if (currentX > 0)                                                                 //  we're moving right
		{
			if (target.transform.position.x > transform.position.x)                         //	and the player is to the right of us
			{
				direction = (target.transform.position - transform.position).normalized;      //	find the shortest direction from us to the Player
				force = direction * speed * Time.deltaTime;                                   //	and calculate the force to get us there

				//	now we need to clamp the speed to a min of 4 on the x so we keep
				//	flying in a horizontal direction and don't slow down and a max of 1.5/-1.5 on 
				//	the y so we don't	flying vertically.
				force.x = Mathf.Max(force.x, 4f);
				if (force.y > 1.5) force.y = 1.5f;
				if (force.y < -1.5) force.y = -1.5f;
			}
		}

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
