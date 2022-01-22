using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipPartsBehaviour : MonoBehaviour
{
  public enum ObjectState { FALLING, WAITING, IN_TRANSIT, DROPPING, DOCKED };
  public ObjectState state;
  private Rigidbody2D rb;
  public float speed = 2.0f;
  public float dropzoneX = 4.0f;
  private Transform parent;


  private void Start()
  {
    state = ObjectState.WAITING;
    rb = GetComponent<Rigidbody2D>();
  }



  private void FixedUpdate()
  {
    //  if the object is falling from the sky OR has been dropped in the drop zone then apply a small downward force to it
    if (state == ObjectState.FALLING)
      rb.MovePosition(new Vector2(transform.position.x, transform.position.y - speed * Time.deltaTime));
    else if (state == ObjectState.DROPPING)
    {
      //    force the spaceship part to the correct x position and drop it down to be docked.
      gameObject.transform.position = new Vector3(dropzoneX, gameObject.transform.position.y, 0f);
      rb.MovePosition(new Vector2(dropzoneX, transform.position.y - speed * Time.deltaTime));
    }
  }



  private void OnTriggerEnter2D(Collider2D collider)
  {

    //  if the part has hit the ground then it means the player dies and dropped the part so set it to WAITING to stop if falling
    if (collider.gameObject.tag == "Ground" && state != ObjectState.IN_TRANSIT)
    {
      //   Debug.Log(gameObject.tag + " hit the ground");
      state = ObjectState.WAITING;
    }


    //  if the player has bumped into the part when it was either in the air or on the ground
    //  than attached the part to the player so it is no in transit and will go whereever
    //  the player goes.
    if (collider.tag == "Player" && (state == ObjectState.WAITING || state == ObjectState.FALLING))
    {
      //   Debug.Log("Player collided with " + gameObject.tag);

      if (gameObject.tag == LevelController.NextSpaceshipPart)
      {

        //  save the parent object for later re-attachement
        parent = gameObject.transform.parent;

        //  assign this object to the player we go where he goes
        Transform player = collider.gameObject.transform;
        gameObject.transform.parent = player;
        gameObject.transform.position = player.position;
        state = ObjectState.IN_TRANSIT;
      }
    }


    //  if the player has a part in transit and passes through the drop zone then unhook the
    //  cell from the player set the state of it to dropping.
    if (collider.tag == "Dropzone" && state == ObjectState.IN_TRANSIT)
    {

      //   Debug.Log("Dropping " + gameObject.tag + ". Attaching to ther parent:" + parent.name);

      //  remove the part from the player object and assign it back to it's parent
      gameObject.transform.parent = parent;

      state = ObjectState.DROPPING;

    }

    //  if the part has landed on another part then see if it's in the drop zone
    //  if it is then stop the part moving as it's now docked and inform the levelcontroller
    //  TODO if it isn't then it means the player died and dropped the part on top of the next part
    //  TODO so we need to decide what we're going to do with it!!!
    if (gameObject.tag == "SpaceshipPart2" && collider.tag == "SpaceshipPart3")
    {
      //   Debug.Log(gameObject.tag + " docked successfully");
      state = ObjectState.DOCKED;
      LevelController.NextSpaceshipPart = "SpaceshipPart1";
    }
    else if (gameObject.tag == "SpaceshipPart1" && collider.tag == "SpaceshipPart2")
    {
      //   Debug.Log(gameObject.tag + " docked successfully");
      state = ObjectState.DOCKED;
      LevelController.NextSpaceshipPart = null;
      LevelController.SpaceshipBuilt = true;
    }

  }
}
