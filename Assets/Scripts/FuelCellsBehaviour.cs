using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelCellsBehaviour : MonoBehaviour
{

  public enum ObjectState { FALLING, WAITING, IN_TRANSIT, DROPPING };
  public ObjectState state;
  private Rigidbody2D rb;
  public float speed = 2.0f;
  public float dropzoneX = 4.0f;


  private void Start()
  {
    state = ObjectState.FALLING;
    rb = GetComponent<Rigidbody2D>();
  }



  private void FixedUpdate()
  {
    //  if the object is falling from the sky OR has been dropped in the drop zone then apply a small downward force to it
    if (state == ObjectState.FALLING || state == ObjectState.DROPPING)
      rb.MovePosition(new Vector2(transform.position.x, transform.position.y - speed * Time.deltaTime));
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
        Destroy(gameObject);
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
    }


    //  if the player has a cell in transit and passes through the drop zone then unhook the
    //  cell from the player set the state of it to dropping.
    if (collider.tag == "Dropzone" && state == ObjectState.IN_TRANSIT)
    {

      // Debug.Log("DROP");

      //  remove the fuel cell/gems from the Pplayer object
      gameObject.transform.parent = null;

      //  give it some physics, make sure it's on the correct X value and rotation
      gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
      gameObject.transform.position = new Vector3(dropzoneX, gameObject.transform.position.y, 0f);
      gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

      state = ObjectState.DROPPING;

    }

  }
}
