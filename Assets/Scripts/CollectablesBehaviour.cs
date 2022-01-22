using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablesBehaviour : MonoBehaviour
{

  public enum ObjectState { FALLING, WAITING, COLLECTED };
  public ObjectState state;
  private Rigidbody2D rb;

  public int scoreValue = 20;
  public float speed = 2.0f;


  private void Start()
  {
    state = ObjectState.FALLING;
    rb = GetComponent<Rigidbody2D>();
  }

  private void FixedUpdate()
  {
    //  if the object is still falling then apply a small downward force to it
    if (state == ObjectState.FALLING)
      rb.MovePosition(new Vector2(transform.position.x, transform.position.y - speed * Time.deltaTime));
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

      //  and remove us from the scene
      Destroy(gameObject);

    }

  }

}
