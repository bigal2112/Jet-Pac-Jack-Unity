using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMaster : MonoBehaviour
{

	public static GameMaster gmInstance;

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

	// private bool playerAlreadyDead = false;


	private void Awake()
	{
		//	---------------------------------------------------------------------------------------
		//	Create a Singleton of the GameMaster class that will not be destroyed between scenes.
		//	---------------------------------------------------------------------------------------

		//  if there is an instance of the GameMaster
		if (gmInstance != null)
		{
			//  and the instance is not this instance (the first instance) then destory it
			if (gmInstance != this)
				Destroy(this.gameObject);
		}
		else
		//  if there is no instance then create one and set to the not be destroyed between scenes
		{
			gmInstance = this;
			DontDestroyOnLoad(this);
		}
	}



	//  I  n  c  r  e  m  e  n  t  P  l  a  y  e  r  1  S  c  o  r  e
	//  ---------------------------------------------------------------------------------------------------
	//  
	//  A global method to add an amount to the score of player 1
	//
	public static void IncrementPlayer1Score(int amount)
	{
		gmInstance._incrementPlayer1Score(amount);
	}

	//  internal method that actually adds the amount to player 1's score
	private void _incrementPlayer1Score(int amount)
	{
		Player1Score += amount;
		player1ScoreText.text = _player1Score.ToString("000000");

	}


	public static void DecrementPlayersLives()
	{
		gmInstance._decrementPlayersLives();
	}

	private void _decrementPlayersLives()
	{
		_remainingLives--;
		player1Lives.text = _remainingLives.ToString();

		if (_remainingLives == 0)
		{
			//	GAME OVER
			Debug.Log("GAME OVERRRRRRRRR");
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
		// playerAlreadyDead = false;

	}

}
