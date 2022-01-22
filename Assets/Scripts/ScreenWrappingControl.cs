using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrappingControl : MonoBehaviour
{
  public GameObject playerTwin;     //  this is the instance of the player that will be show on the opposite side of the screen.

  public float spriteWidth;
  public float buffer;

  public Vector2 screenBounds;
  public Vector2 screenOrigo;

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
    buffer = 0.25f;

  }


  private void FixedUpdate()
  {

    GameObject twinObject;
    Vector3 pos = transform.position;

    //  if our player is outside the bounds of the screen by the buffer amount and
    //  the twinObject is in the scene then move the player to the twins position and remove the twin
    if (pos.x - buffer > screenBounds.x || pos.x + buffer < screenOrigo.x)
    {
      twinObject = GameObject.FindGameObjectWithTag("Twin");
      if (twinObject != null)
      {
        transform.position = new Vector3(twinObject.transform.position.x, twinObject.transform.position.y, twinObject.transform.position.y);
        Destroy(twinObject);
      }
    }

    //  if the player is beginning to move out of view
    if (pos.x > screenBounds.x || pos.x < screenOrigo.x)
    {
      //  if the twin is not already on the screen then create it
      twinObject = GameObject.FindGameObjectWithTag("Twin");
      if (twinObject == null)
        Instantiate(playerTwin, new Vector3(pos.x * -1, pos.y, pos.z), transform.rotation);
    }

  }

}
