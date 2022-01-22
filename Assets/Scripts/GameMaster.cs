using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMaster : MonoBehaviour
{

  public static GameMaster gm;

  //  internal remaining lives parameter and externally accessible getter
  private static int _remainingLives = 3;
  public static int RemainingLives
  {
    set { _remainingLives = value; }
    get { return _remainingLives; }
  }

  private static int _player1Score = 0;
  public static int Player1Score
  {
    set { _player1Score = value; }
    get { return _player1Score; }
  }

  public Transform playerSpawnPoint;
  public GameObject spaceman;

  public TextMeshProUGUI player1ScoreText;                                //  player 1's score
  public TextMeshProUGUI player1Lives;                                    //  player 1's lives

  private bool playerAlreadyDead = false;


  private void Awake()
  {
    //    make sure we have a GameMaster object in the scene
    if (gm == null)
      gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
  }



  //  I  n  c  r  e  m  e  n  t  P  l  a  y  e  r  1  S  c  o  r  e
  //  ---------------------------------------------------------------------------------------------------
  //  
  //  A global method to add an amount to the score of player 1
  //
  public static void IncrementPlayer1Score(int amount)
  {
    gm._incrementPlayer1Score(amount);
  }

  //  internal method that actually adds the amount to player 1's score
  private void _incrementPlayer1Score(int amount)
  {
    Player1Score += amount;
    player1ScoreText.text = _player1Score.ToString("000000");

  }

  //  P  l  a  y  e  r  D  i  e  d
  //  ------------------------------------------------------------------------------------------------------
  // 
  //  Decrement the lives. 
  //  If they are carrying a fueld cell then unhook it from them and drop it where it is.
  //  If they get to 0 get run the Game Over skit
  //
  public static void PlayerDied(GameObject player)
  {

    gm._playerDied(player);
  }

  private void _playerDied(GameObject _player)
  {

    //  if we haven't taken away a life yet tke one away and set the flag to show the player is dying so we 
    //  don't take any more lives away if another enemy hits us
    if (!playerAlreadyDead)
    {

      playerAlreadyDead = true;
      _remainingLives--;
      player1Lives.text = _remainingLives.ToString();

      //  stop the player moving
      Rigidbody2D rb = _player.GetComponent<Rigidbody2D>();
      if (rb != null)
        rb.Sleep();

      //  Run the killed animation it is exists on the PLayer
      Animator playerAnim = _player.GetComponent<Animator>();
      if (playerAnim != null)
        playerAnim.SetBool("Killed", true);

      //  remove the player object after a second
      Destroy(_player, 1);
    }


    if (_remainingLives <= 0)
    {
      Debug.Log("G  A  M  E     O  V  E  R");
    }
    else
    {
      StartCoroutine(SpawnSpaceman());
    }
  }


  IEnumerator SpawnSpaceman()
  {
    yield return new WaitForSeconds(5.5f);

    Debug.Log("Spawning in GameMaster");
    //  TODO: only spawn the player if there are no enemies nearby. Try using raycast to see what's happening around you.
    //  SPAWN THE PLAYER AS HE'S BEEN DESTROYED AT THE END OF THE PREVIOUS LOOP
    Instantiate(spaceman, playerSpawnPoint.transform.position, Quaternion.identity);
    playerAlreadyDead = false;

  }

}
