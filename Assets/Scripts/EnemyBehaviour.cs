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

	private Animator anim;

	private Rigidbody2D rb;
	private int direction = -1;

	private bool dying = false;



	void Start()
	{
		//  if we've been spawned on the righ-hand side then birl us around so we're pointing the right way
		if (transform.position.x < 0)
		{
			transform.rotation = Quaternion.Euler(0f, 180f, 0f);
			direction = 1;
		}

		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		Destroy(gameObject, 10.0f);

	}

	private void FixedUpdate()
	{
		if (!dying)
			rb.MovePosition(new Vector2(transform.position.x + (speed * direction) * Time.fixedDeltaTime, transform.position.y - Mathf.Sin(attackAngle) * Time.fixedDeltaTime));
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


}   //  class
