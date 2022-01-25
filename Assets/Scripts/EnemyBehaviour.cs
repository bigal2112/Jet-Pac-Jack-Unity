using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
	public enum GroundContactAction { EXPLODE, BOUNCE }

	public float attackAngle = 0f;
	public float speed = 2f;
	public int waitBeforeAttack = 3;
	public GroundContactAction groundContactAction = GroundContactAction.EXPLODE;
	Vector3 lastVelocity;
	ContactPoint2D[] contactPoints;

	private Animator anim;

	private Rigidbody2D rb;
	private int direction = -1;

	private bool dying = false;
	private float attackAngleInRad;



	void Start()
	{
		//  if we've been spawned on the left-hand side then birl us around so we're pointing the right way
		if (transform.position.x < 0)
		{
			transform.rotation = Quaternion.Euler(0f, 180f, 0f);
			direction = 1;
		}

		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		Destroy(gameObject, 10.0f);

		//	change the attack angle from degrees to radians so the Mathf.Sin() function works in the FixedUpdate() method.
		attackAngleInRad = attackAngle / Mathf.Rad2Deg;

	}

	private void FixedUpdate()
	{
		if (!dying)
			rb.MovePosition(new Vector2(transform.position.x + (speed * direction) * Time.fixedDeltaTime, transform.position.y - Mathf.Sin(attackAngleInRad) * Time.fixedDeltaTime));
	}


	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.tag == "Ground")
		{
			// Debug.Log("Hit the ground - " + groundContactAction);
			if (groundContactAction == GroundContactAction.EXPLODE)
			{
				dying = true;
				anim.SetBool("KillMe", true);
				Destroy(gameObject, 1.0f);
			}
			else if (groundContactAction == GroundContactAction.BOUNCE)
			{

				//	we need to change the attack angle by +- 270 so that the enemy bouncys off the ball in the same direction (left/right), but at 90 degrees
				//	to how it came into the wall.
				//	we then need to change the new angle to radians so the Mathf.Sin() function works in the FixedUpdate() method.
				if (attackAngle <= 90)
					attackAngle += 270;
				else
					attackAngle -= 270;

				attackAngleInRad = attackAngle / Mathf.Rad2Deg;
			}

		}

		if (collider.tag == "Player")
		{
			//  call the player dead method
			// GameMaster.PlayerDied(collider.gameObject);

			//  and kill me
			dying = true;
			anim.SetBool("KillMe", true);
			Destroy(gameObject, 1.0f);

		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		Debug.Log("Collided with " + other.gameObject.name);
	}


}   //  class
