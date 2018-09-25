using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour {

    public BattleWave[] waves;
	public EventQueue whenDone;
	private Vector3 randDist = Vector3.zero;
    
	void Start () {
        StartCoroutine("BattleLoop");
	}

    private void SpawnEnemy(EnemyInfo enemy, int amount) {
        if(enemy.spawnPoints.Length < 1) {
            Debug.LogError("BATTLE: Unable to spawn enemy - no spawnpoints specified");
        } else {
            int randIndex = (int)Mathf.Floor(Random.Range(0, enemy.spawnPoints.Length));    // get random spawnpoint index
            Vector3 ePos = enemy.spawnPoints[randIndex].transform.position;                 // use this spawnpoint to spawn an enemy
            for(int i = 0; i < amount; i++) {
				randDist.x = Random.Range(0, enemy.spawnPoints[randIndex].radius);			// slightly randomize spawn location dependent on spawnpoint radius
				randDist.y = Random.Range(0, enemy.spawnPoints[randIndex].radius);
				Instantiate(enemy.enemyPrefab, ePos + randDist, Quaternion.identity);
            }
        }
    }

    private IEnumerator BattleLoop() {
        foreach(BattleWave wave in waves) {
            // Need info for each enemy type on:
            // - type of enemy to spawn
            // - number to spawn per second (array)
            WaveInfo[] waveInfos = new WaveInfo[wave.enemies.Length];
            // Initialize WaveInfos
            for(int i = 0; i < waveInfos.Length; i++) {
                waveInfos[i] = new WaveInfo(wave.enemies[i], wave.seconds);
            }
            for(int i = 0; i < wave.seconds; i++) {         // each second...
                for(int j = 0; j < waveInfos.Length; j++) { // for each type of enemy to be spawned...
                    // spawn enemies as specified
                    SpawnEnemy(waveInfos[j].enemyInfo, waveInfos[j].enemiesPerSec[i]);
                }
                yield return new WaitForSeconds(1f);
            }
            Debug.Log("BATTLE: Wave finished");
        }
    }
}

class WaveInfo {
    public EnemyInfo enemyInfo; // type of enemy to spawn
    public int[] enemiesPerSec; // how many enemies to spawn each second, where second number = index in array

    public WaveInfo(EnemyInfo enemy, int waveSeconds) {
        enemyInfo = enemy;
        enemiesPerSec = new int[waveSeconds];

        int minIndex = (int)Mathf.Floor(enemy.spawnStart * waveSeconds);    // seconds until enemies start to spawn
        int maxIndex = (int)Mathf.Floor(enemy.spawnEnd * waveSeconds);      // seconds until enemies stop spawning

        if(enemy.randomDist) {
            // randomly distribute enemies across the array
            for(int i = 0; i < enemy.amount; i++) {
                int rand = (int)Mathf.Floor(Random.Range(minIndex, maxIndex));  // get random index to increment
                enemiesPerSec[rand]++;
            }
        } else {
            // uniformly distribute enemies
            float timespan = maxIndex - minIndex;           // amount of seconds over which enemies will spawn
            float eps = enemy.amount / timespan;            // enemies per second, likely featuring a decimal
            int interval = (int)Mathf.Round(1 / (eps % 1)); // take that decimal and convert it to an interval between spawning single enemies
                                                            // e.g., 0.2 would be one enemy every 5 seconds
            for(int i = minIndex; i < maxIndex; i++) {
                enemiesPerSec[i] = (int)Mathf.Floor(eps);
                if((i + 1) % interval == 0) enemiesPerSec[i]++; // i + 1 in case i is 0, because 0 % interval is always 0
                                                                // before it was fixed, this caused the first index of the array to always increment
            }
        }
        string s = "";
        int total = 0;
        for(int i = 0; i < enemiesPerSec.Length - 1; i++) {
            s += "" + enemiesPerSec[i] + ", ";
            total += enemiesPerSec[i];
        }
        s += "" + enemiesPerSec[enemiesPerSec.Length - 1];
        total += enemiesPerSec[enemiesPerSec.Length - 1];
        Debug.Log("BATTLE: enemy array (length: " + enemiesPerSec.Length + ", total: " + total + "): " + s);
    }
}