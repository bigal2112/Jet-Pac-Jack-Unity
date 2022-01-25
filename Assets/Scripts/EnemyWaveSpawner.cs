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
	}

	public Wave[] waves;
	public int nextWave;
	public float timeBetweenWaves = 5f;
	public float waveCountdown;
	public SpawnState state;

	public Transform[] enemySpawnPoints;

	private LevelController lv;

	void Start()
	{
		// Debug.Log("EnemyWaveSpawner: Start()");
		lv = LevelController.lvInstance;

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
		// Debug.Log("SPAWNING WAVE: " + _wave.name);

		//  loop through how many enemies are in this wave
		for (int i = 0; i < _wave.count; i++)
		{
			//  spawn an enemy and wait before spawning the next one
			SpawnEnemy(_wave.enemy);
			yield return new WaitForSeconds(1f / _wave.rate);
		}

		//  once we've spawned all the enemies update the state
		state = SpawnState.COUNTING;


		//   now we can start the countdown for the next wave and update which wave we are on.
		waveCountdown = timeBetweenWaves;

		//  now onto the next wave. If we have gone past the final wave then go back to the first one for this level loop.
		nextWave++;
		Debug.Log("nextWave:" + nextWave);
		Debug.Log("lv.enemyWaveTo:" + lv.enemyWaveTo);
		if (nextWave > lv.enemyWaveTo)
			nextWave = lv.enemyWaveFrom;

		yield break;
	}


	private void SpawnEnemy(Transform _enemy)
	{

		// Debug.Log("  SPAWNING ENEMY: " + _enemy.name);

		//  find a random spawn point
		Vector3 spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)].position;

		//  spawn the enemy
		Transform theEnemy = Instantiate(_enemy, spawnPoint, Quaternion.identity);


	}

}       //  class
