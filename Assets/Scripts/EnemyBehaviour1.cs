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


	void Start()
	{

		//	get the component we need
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		//	TODO: get a ramdon number between -1 and 1 to use as the y value so the up/down direction and angle is erm random!

		//  if we've been spawned on the left-hand side then birl us around so we're pointing the right way
		//	and set us off in a left-to-right direction by using a +ve value for the x
		if (transform.position.x < 0)
		{
			if (!mirrorImage)
				transform.rotation = Quaternion.Euler(0f, 180f, 0f);

			// set our enemy on its merry way. 
			rb.velocity = new Vector2(1, -0.5f) * speed;
		}
		else
		{
			// set our enemy on its merry way but in a right-to-left direction by using a -ve value for the x
			rb.velocity = new Vector3(-1, -0.5f, 0) * speed;
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

		if (collider.tag == "Player")
		{

			//  and kill me
			dying = true;
			anim.SetBool("KillMe", true);
			Destroy(gameObject, 1.0f);

		}
	}

}   //  class
