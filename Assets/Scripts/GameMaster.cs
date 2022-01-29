using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
// using System;

public class GameMaster : MonoBehaviour
{

	public static GameMaster gmInstance;
	[SerializeField] private GameObject gameOverUI;

	//  internal remaining lives parameter and externally accessible getter
	private static int _remainingLives;
	public static int RemainingLives
	{
		set { _remainingLives = value; }
		get { return _remainingLives; }
	}

	private static int _player1Score;
	public static int Player1Score
	{
		set { _player1Score = value; }
		get { return _player1Score; }
	}

	public Transform playerSpawnPoint;
	public GameObject spaceman;

	public TextMeshProUGUI player1ScoreText;                                //  player 1's score
	public TextMeshProUGUI player1Lives;                                    //  player 1's lives

	private static int _enemiesInRespawnBubble = 0;
	public static int EnemiesInRespawnBubble
	{
		set { _enemiesInRespawnBubble = value; }
		get { return _enemiesInRespawnBubble; }
	}
	public int EnemiesInMyBubble;

	private Color[] colors;

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

		PopulateColorsArray();

		//	make sure we have the correct ordering in the GameMaster object
		if (transform.GetChild(0).GetChild(0).name != "GameOverUI")
		{
			Debug.LogError("FATAL ERROR: Ensure the Canvas object is the first child of the GameMaster and the GameOverUI is the first child of the Canvas !!!");
		}

		InitialisePlayer1ScoreAndLives();
	}



	private void Update()
	{
		EnemiesInMyBubble = _enemiesInRespawnBubble;
	}



	private void PopulateColorsArray()
	{
		colors = new Color[8];

		colors[0] = Color.black;
		colors[1] = Color.blue;
		colors[2] = Color.red;
		colors[3] = Color.magenta;
		colors[4] = Color.green;
		colors[5] = Color.cyan;
		colors[6] = Color.yellow;
		colors[7] = Color.white;
	}

	public static Color GetRandomColor()
	{
		return gmInstance._getRandomColor();
	}

	public Color _getRandomColor()
	{
		return colors[Random.Range(1, 8)];
	}


	//  I  n  i  t  i  a  l  i  s  e  P  l  a  y  e  r  1  S  c  o  r  e  A  n  d  L  i  v  e  s
	//  ---------------------------------------------------------------------------------------------------
	//  
	//  A global method to add an amount to the score of player 1
	//
	public static void InitialisePlayer1ScoreAndLives()
	{
		gmInstance._initialisePlayer1ScoreAndLives();
	}

	//  internal method that actually adds the amount to player 1's score
	private void _initialisePlayer1ScoreAndLives()
	{
		Player1Score = 0;
		player1ScoreText.text = _player1Score.ToString("000000");

		RemainingLives = 3;
		player1Lives.text = _remainingLives.ToString();

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
			gameOverUI.SetActive(true);
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

		//	this will wait until there are no enemies in the respawn bubble before respawing the player.
		while (true)
		{
			if (_enemiesInRespawnBubble == 0)
				break;

			yield return new WaitForSeconds(0.1f);
		}

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
