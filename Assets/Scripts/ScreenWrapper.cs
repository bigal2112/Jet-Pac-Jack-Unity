using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrapper : MonoBehaviour
{
	public float spriteWidth;
	public float buffer;

	public Vector2 screenBounds;
	public Vector2 screenOrigo;

	public bool replicaAlreadyCreated;

	public bool goingLeft;
	private Color myColor;

	// Start is called before the first frame update
	void Start()
	{

		// get the bounds of the screen and the origin
		screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
		screenOrigo = Camera.main.ScreenToWorldPoint(Vector2.zero);

		//  get the width of the sprite and set the buffer width. This will be used to help
		//  the smoothness of the transition from side to side when the twin is created.
		SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
		spriteWidth = sRenderer.sprite.bounds.size.x;                     //  gets the width of our element.
		myColor = sRenderer.color;

		replicaAlreadyCreated = false;
		buffer = 0.5f;

		//  if we were born on the right of the screen (+ve x) then we're going left to start with
		goingLeft = transform.position.x > 0;


	}


	private void FixedUpdate()
	{
		GameObject replicaObject;
		Vector3 pos = transform.position;

		//  if we are starting to go outside the bounds of the screen 
		//      1. we're going right and we're past the righthand side of the screen OR
		//      2. we're going left and we've passed the lefthand side of the screen AND
		//      3. a replica of ourselves HAS NOT already been created AND
		//      4. we're an Enemy (not a replica) 
		//
		//      Then create a replica
		if (((pos.x + (spriteWidth / 2) > screenBounds.x && !goingLeft) ||
			(pos.x - (spriteWidth / 2) < screenOrigo.x && goingLeft)) &&
			!replicaAlreadyCreated &&
			gameObject.tag == "Enemy")
		{
			float newX;
			if (pos.x < 0)
				newX = (pos.x * -1) + spriteWidth;
			else
				newX = (pos.x * -1) - spriteWidth;

			replicaObject = Instantiate(gameObject, new Vector3(newX, pos.y, pos.z), transform.rotation);

			if (replicaObject == null)
			{
				Debug.LogError("Error initiating replica of " + gameObject.name);
			}
			else
			{
				replicaObject.tag = "ReplicaEnemy";
				replicaObject.GetComponent<SpriteRenderer>().color = myColor;
				replicaAlreadyCreated = true;
			}
		}

		//  if we are a ReplicaEnemy and it is fully inside the screen boudaries we can sfely change it to an Enemy
		if ((pos.x + (spriteWidth / 2) < screenBounds.x && pos.x - (spriteWidth / 2) > screenOrigo.x) && gameObject.tag == "ReplicaEnemy")
		{
			// Debug.Log("Changing tag to Enemy");
			gameObject.tag = "Enemy";
		}

		// if we are fully outside the screen boudaries we can safely destroy ourselves
		if (pos.x - spriteWidth - buffer > screenBounds.x || pos.x + spriteWidth + buffer < screenOrigo.x)
		{
			// Debug.Log("Killing myself");
			Destroy(gameObject);
		}

	}

}
