using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBehaviour1 : MonoBehaviour
{

	public enum GroundContactAction { EXPLODE, BOUNCE }

	public float speed = 2f;
	public int waitBeforeAttack = 3;
	public bool mirrorImage;
	public GroundContactAction groundContactAction = GroundContactAction.EXPLODE;

	private Animator anim;
	private Rigidbody2D rb;

	private bool dying = false;
	private float yValue;
	private bool inTheRespawnBubble;

	void Start()
	{

		//	get the component we need
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		inTheRespawnBubble = false;

		//  if we've been spawned on the left-hand side then birl us around so we're pointing the right way
		//	and set us off in a left-to-right direction by using a +ve value for the x
		if (transform.position.x < 0)
		{
			if (!mirrorImage)
				transform.rotation = Quaternion.Euler(0f, 180f, 0f);

			// Calculate a random y value between -1 and 1 and set our enemy on its merry way. 
			yValue = Random.Range(-10, 11) / 10.0f;
			rb.velocity = new Vector2(1, yValue) * speed;
		}
		else
		{
			// // Calculate a random y value between -1 and 1 and set our enemy on its merry way but in a right-to-left direction by using a -ve value for the x
			yValue = Random.Range(-10, 11) / 10.0f;
			rb.velocity = new Vector2(-1, yValue) * speed;
		}

		// freeze the rotation so it doesnt go spinning after a collision
		rb.freezeRotation = true;

		//	destroy us after 10 seconds
		Destroy(gameObject, 10.0f);
	}


	private void FixedUpdate()
	{
		// if we're dying then stop us moving.
		if (dying)
			rb.velocity = Vector2.zero;
	}




	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.CompareTag("Ground") || collider.CompareTag("Ceiling"))
		{
			// Debug.Log("Hit the " + collider.name + " - " + groundContactAction);

			if (groundContactAction == GroundContactAction.EXPLODE)
			{
				dying = true;
				anim.SetBool("KillMe", true);
				Destroy(gameObject, 1.0f);
			}
			else if (groundContactAction == GroundContactAction.BOUNCE)
			{

				// make us bounce off the collider by negating the y value of our velocity
				rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * -1); ;

			}

		}

		if (collider.CompareTag("Player"))
		{
			//	if we've killed the player and we're in the respawn bubble then reduce the
			//	count by 1 as we're just about to explode.....
			if (inTheRespawnBubble) GameMaster.EnemiesInRespawnBubble--;

			//  and kill me
			dying = true;
			anim.SetBool("KillMe", true);
			Destroy(gameObject, 1.0f);
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
