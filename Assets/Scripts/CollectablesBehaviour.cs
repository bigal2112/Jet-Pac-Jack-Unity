using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablesBehaviour : MonoBehaviour
{

	public enum ObjectState { FALLING, WAITING, COLLECTED };

	private AudioSource pickupNoise;

	public ObjectState state;
	public bool flashing;
	private Rigidbody2D rb;

	public int scoreValue = 20;
	public float speed = 2.0f;

	private Color[] colors;
	private int colorCounter;
	private SpriteRenderer sr;
	public float flashRate = 1;
	public float flashCounter;


	private void Start()
	{
		state = ObjectState.FALLING;
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();

		colors = new Color[7];

		colors[0] = Color.blue;
		colors[1] = Color.red;
		colors[2] = Color.magenta;
		colors[3] = Color.green;
		colors[4] = Color.cyan;
		colors[5] = Color.yellow;
		colors[6] = Color.white;

		colorCounter = 0;

		flashCounter = flashRate;

		pickupNoise = GetComponent<AudioSource>();
		if (pickupNoise == null)
			Debug.Log("There is no AudioSource component attached to " + gameObject.name);
	}

	private void FixedUpdate()
	{
		//  if the object is still falling then apply a small downward force to it
		if (state == ObjectState.FALLING)
			rb.MovePosition(new Vector2(transform.position.x, transform.position.y - speed * Time.deltaTime));

		flashCounter--;

		if (flashing && flashCounter <= 0)
		{
			sr.color = colors[colorCounter];
			colorCounter++;
			if (colorCounter >= colors.Length) colorCounter = 0;
		}


		if (flashCounter <= 0) flashCounter = flashRate;
	}




	private void OnTriggerEnter2D(Collider2D collider)
	{

		//  if the collectable has hit the ground then change the state so that it stops falling
		if (collider.gameObject.tag == "Ground")
		{
			// Debug.Log("HIT THE GROUND - TRIGGER");
			state = ObjectState.WAITING;
		}


		//  if the player has entered the trigger area them we've been collected
		if (collider.tag == "Player" && state != ObjectState.COLLECTED)
		{
			//  change the state to COLLECTED. With out this we can be triggered several times before we get destroyed so multiple scores can be registered.
			state = ObjectState.COLLECTED;

			//  increment the player's score
			// Debug.Log("Player scored " + scoreValue + " points");
			GameMaster.IncrementPlayer1Score(scoreValue);
			LevelController.DecrementCollectablesCounter();
			pickupNoise.Play();

			//  and remove us from the scene
			Destroy(gameObject, 0.1f);

		}

	}

}
