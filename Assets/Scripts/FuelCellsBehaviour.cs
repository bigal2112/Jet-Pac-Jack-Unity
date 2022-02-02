using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelCellsBehaviour : MonoBehaviour
{

	public enum ObjectState
	{
		FALLING,            //	the cell is falling from the sky (initial state)
		WAITING,            //	the cell is resting on the ground waiting to be picked up
		IN_TRANSIT,         //	the cell has been picked up and the player has it
		DROPPING,           //	the player has past the drop zone and the cell is dropping into the spaceship
		PLAYER_DIED         //	the player has died and the cell is falling to the ground
	};
	public ObjectState state;
	private Rigidbody2D rb;
	public float speed = 2.0f;
	public float dropzoneX = 4.0f;
	public AudioClip pickupSound;
	public AudioClip dockingSound;
	public AudioSource audioSource;


	private void Start()
	{
		state = ObjectState.FALLING;
		rb = GetComponent<Rigidbody2D>();
		audioSource.GetComponent<AudioSource>();
	}



	private void FixedUpdate()
	{
		//	make sure the fuel cell is always facing the correct way
		gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

		//  if the object is falling from the sky OR has been dropped in the drop zone OR 
		//	has been dropped by the player getting killed then apply a small downward force to it
		if (state == ObjectState.FALLING || state == ObjectState.DROPPING || state == ObjectState.PLAYER_DIED)
			rb.MovePosition(new Vector2(transform.position.x, transform.position.y - speed * Time.deltaTime));

		//  if we're still in a state of IN_TRANSIT but we're not attached to the Player object then
		//  the player has been killed and we've been re-attached to the Spaceship so can fall to the ground
		//	we add a little nudge up so that, if the player dies when standing on the ground the fuel cell
		//	diesn't just fall through the ground but lands on in so that the state changes to WAITING.
		if (state == ObjectState.IN_TRANSIT && gameObject.transform.parent == null)
		{
			// Debug.Log(gameObject.name + " in transit but NOT attached to player so start falling");
			rb.MovePosition(new Vector2(transform.position.x, transform.position.y + 0.05f));
			state = ObjectState.PLAYER_DIED;
		}
	}



	private void OnTriggerEnter2D(Collider2D collider)
	{

		//  if the cell has hit the ground then check whether it's X value is that of the drop zone
		//  if it is then the cell has been loaded into the ship
		//  if not the update the state so that it stops falling
		if (collider.gameObject.tag == "Ground")
		{
			// Debug.Log("HIT THE GROUND");

			if (transform.position.x == dropzoneX)
			{
				LevelController.FuelCellLanded();
				audioSource.PlayOneShot(dockingSound, 1);
				Destroy(gameObject, 0.5f);
			}
			else
			{
				state = ObjectState.WAITING;
			}
		}


		//  if the player has bumped into the cell when it was either in the air or on the ground
		//  than attached the cell to the player so it is no in transit and will go whereever
		//  the player goes.
		if (collider.tag == "Player" && (state == ObjectState.WAITING || state == ObjectState.FALLING))
		{
			// Debug.Log("Player collided");

			//  assign this object to the player we go where he goes
			Transform player = collider.gameObject.transform;
			gameObject.transform.parent = player;
			gameObject.transform.position = player.position;
			state = ObjectState.IN_TRANSIT;
			audioSource.PlayOneShot(pickupSound, 1);
		}


		//  if the player has a cell in transit and passes through the drop zone then unhook the
		//  cell from the player set the state of it to dropping.
		if (collider.tag == "Dropzone" && state == ObjectState.IN_TRANSIT)
		{

			Debug.Log("DROP");

			//  remove the fuel cell/gems from the Pplayer object
			gameObject.transform.parent = null;

			//  give it some physics, make sure it's on the correct X value and rotation
			gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
			gameObject.transform.position = new Vector3(dropzoneX, gameObject.transform.position.y, 0f);


			state = ObjectState.DROPPING;

		}

	}
}
