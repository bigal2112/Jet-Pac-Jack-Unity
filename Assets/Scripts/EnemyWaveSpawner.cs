using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSpawner : MonoBehaviour
{

	public enum SpawnState { SPAWNING, WAITING, COUNTING }


	[System.Serializable]
	public class Wave
	{
		public string name;
		public Transform enemy;
		public int count;
		public float rate;
		public bool leftSideOnly;
	}

	public Wave[] waves;
	public int nextWave;
	public float timeBetweenWaves = 5f;
	public float waveCountdown;
	public SpawnState state;

	public Transform[] enemySpawnPoints;

	private LevelController lv;

	private bool[] spawnPointUsed;

	void Start()
	{
		// Debug.Log("EnemyWaveSpawner: Start()");
		lv = LevelController.lvInstance;

		spawnPointUsed = new bool[enemySpawnPoints.Length];

		waveCountdown = timeBetweenWaves;
		nextWave = lv.enemyWaveFrom;

		state = SpawnState.COUNTING;
	}

	//  use OnEnable() as we disable the spawning of enemies when the rocket launches and re-enable it once we land again
	//  but this didn't call the Start() method when re-enabling but it does call the EOnEnable() method. 
	//  The OnEnable() method is also called prior to the Start() method when the app starts so we can put all the initialising logic in here :)
	private void OnEnable()
	{
		// Debug.Log("EnemyWaveSpawner: OnEnable()");
		lv = LevelController.lvInstance;
		if (lv != null)
			nextWave = lv.enemyWaveFrom;

		waveCountdown = timeBetweenWaves;
		state = SpawnState.COUNTING;
	}



	void Update()
	{

		//    has the wave countdown reached 0?
		if (waveCountdown <= 0)
		{
			//  if we're not already spawning the wave
			if (state != SpawnState.SPAWNING)
			{
				//    start spawning the wave
				StartCoroutine(SpawnWave(waves[nextWave]));
			}
		}
		else
		{

			//  wave countdown has not reached 0 yet so decrement it
			waveCountdown -= Time.deltaTime;
		}


	}


	IEnumerator SpawnWave(Wave _wave)
	{
		state = SpawnState.SPAWNING;

		//	initialise the spawnPointUsed array
		for (int i = 0; i < enemySpawnPoints.Length; i++)
		{
			spawnPointUsed[i] = false;
		}

		//  loop through how many enemies are in this wave
		for (int i = 0; i < _wave.count; i++)
		{
			//  spawn an enemy and wait before spawning the next one
			SpawnEnemy(_wave);
			yield return new WaitForSeconds(1f / _wave.rate);
		}

		//  once we've spawned all the enemies update the state
		state = SpawnState.COUNTING;


		//   now we can start the countdown for the next wave and update which wave we are on.
		waveCountdown = timeBetweenWaves;

		//  now onto the next wave. If we have gone past the final wave then go back to the first one for this level loop.
		nextWave++;
		if (nextWave > lv.enemyWaveTo)
			nextWave = lv.enemyWaveFrom;

		yield break;
	}


	private void SpawnEnemy(Wave _wave)
	{

		int newPoint;

		//	while we're looking for a free spaw point
		while (true)
		{
			//	if we only want to spawn these enemies on the lefthad side of the screen then just use the first half of the spawn points.
			//	there needs to be an even amount of spawn points arranged evenly between the left and right hand sides.
			int spawnPointLimit;
			if (_wave.leftSideOnly)
				spawnPointLimit = (int)enemySpawnPoints.Length / 2;
			else
				spawnPointLimit = enemySpawnPoints.Length;

			newPoint = Random.Range(0, spawnPointLimit);
			if (spawnPointUsed[newPoint] == false)
			{
				spawnPointUsed[newPoint] = true;
				break;
			}
		}

		//  we've got a free spawn point so spawn the enemy at it.
		Transform theEnemy = Instantiate(_wave.enemy, enemySpawnPoints[newPoint].position, Quaternion.identity);
		theEnemy.GetComponent<SpriteRenderer>().color = GameMaster.GetRandomColor();      //	apply a random color here, do not move this to the objects Start()!!
	}

}       //  class
