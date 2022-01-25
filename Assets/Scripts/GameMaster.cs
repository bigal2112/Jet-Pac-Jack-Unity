using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

		if (_remainingLives == 0)
		{
			//	GAME OVER
			Debug.Log("GAME OVERRRRRRRRR");
		}
		else
		{
			_remainingLives--;

			player1Lives.text = _remainingLives.ToString();
			SpawnSpaceman();
		}
	}


	//	S  p  a  w  n  S  p  a  c  e  m  a  n
	//	-----------------------------------------------------------------------------------------------------
	//
	//	Publically accessable method to start the spawning of a new spaceman. This calls the private method
	//	_spawnSpaceman() which in turn starts the co-routine StartSpawningTheSpaceman() which does the actual
	//	work.
	//
	public static void SpawnSpaceman()
	{
		gmInstance._spawnSpaceman();
	}

	private void _spawnSpaceman()
	{
		StartCoroutine(StartSpawningTheSpaceman());
	}



	IEnumerator StartSpawningTheSpaceman()
	{

		//	wait for another little bit then respawn player.
		yield return new WaitForSeconds(5.5f);

		//  TODO: only spawn the player if there are no enemies nearby. Try using raycast to see what's happening around you.
		//  SPAWN THE PLAYER AS HE'S BEEN DESTROYED AT THE END OF THE PREVIOUS LOOP
		if (NoPlayersInScene())
			Instantiate(spaceman, playerSpawnPoint.transform.position, Quaternion.identity);
	}

	private bool NoPlayersInScene()
	{

		//  go through all the game object and if any of them are collectables destroy them
		List<GameObject> rootObjects = new List<GameObject>();
		Scene scene = SceneManager.GetActiveScene();
		scene.GetRootGameObjects(rootObjects);
		bool noPlayersInScene = true;

		// iterate root objects and do something
		for (int i = 0; i < rootObjects.Count; i++)
		{
			GameObject gameObject = rootObjects[i];
			if (gameObject.CompareTag("Player"))
				noPlayersInScene = false;
		}

		return noPlayersInScene;
	}

}
