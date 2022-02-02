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
	public bool mirrorImage;
	public int scoreValue = 25;

	private Animator anim;
	private Rigidbody2D rb;
	private AudioSource noise;
	private int direction = -1;

	private bool dying = false;
	private float attackAngleInRads;

	private bool inTheRespawnBubble;

	void Start()
	{
		// Debug.Log("X:" + (transform.position.x).ToString());

		//  if we've been spawned on the left-hand side then birl us around so we're pointing the right way
		if (transform.position.x < 0)
		{
			if (!mirrorImage)
				transform.rotation = Quaternion.Euler(0f, 180f, 0f);

			direction = 1;
			attackAngle += 90;
		}

		// Debug.Log("attackAngle:" + attackAngle);

		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		noise = GetComponent<AudioSource>();
		if (noise == null)
			Debug.Log("There is no AudioSource component attached to " + gameObject.name);

		Destroy(gameObject, 10.0f);

		//	change the attack angle from degrees to radians so the Mathf.Sin() function works in the FixedUpdate() method.
		attackAngleInRads = attackAngle / Mathf.Rad2Deg;

	}

	private void FixedUpdate()
	{

		if (!dying)
			rb.MovePosition(new Vector2(transform.position.x + (speed * direction) * Time.fixedDeltaTime, transform.position.y - Mathf.Sin(attackAngleInRads) * Time.fixedDeltaTime));
	}


	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (!dying)
		{

			if (collider.CompareTag("Ground") || collider.CompareTag("Ceiling") || collider.CompareTag("Bullet"))
			{
				// Debug.Log("Hit the ground - " + groundContactAction);
				if (groundContactAction == GroundContactAction.EXPLODE)
				{
					if (inTheRespawnBubble) GameMaster.EnemiesInRespawnBubble--;

					dying = true;
					anim.SetBool("KillMe", true);
					noise.Play();
					Destroy(gameObject, 1.0f);

					if (collider.CompareTag("Bullet"))
						GameMaster.IncrementPlayer1Score(scoreValue);
				}
				else if (groundContactAction == GroundContactAction.BOUNCE)
				{

					//	we need to change the attack angle by +- 270 so that the enemy bouncys off the ground/platforms in the same 
					//	direction (left/right), but at 90 degrees	to how it came into the wall.
					//	we then need to change the new angle to radians so the Mathf.Sin() function works in the FixedUpdate() method.
					if (attackAngle <= 90)
						attackAngle += 90;
					else if (attackAngle <= 180)
						attackAngle -= 90;
					else if (attackAngle <= 270)
						attackAngle += 90;
					else if (attackAngle <= 360)
						attackAngle -= 90;

					attackAngleInRads = attackAngle / Mathf.Rad2Deg;
				}

			}
		}

		if (collider.tag == "Player")
		{
			if (!dying)
			{

				//	if we've killed the player and we're in the respawn bubble then reduce the
				//	count by 1 as we're just about to explode.....
				if (inTheRespawnBubble) GameMaster.EnemiesInRespawnBubble--;

				//  and kill me
				dying = true;
				anim.SetBool("KillMe", true);
				noise.Play();
				Destroy(gameObject, 1.0f);
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
