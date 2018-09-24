using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour {

    public BattleWave[] waves;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator BattleLoop() {
        foreach(BattleWave wave in waves) {
            int[] toSpawn = new int[wave.seconds];
            for(int i = 0; i < wave.seconds; i++) {

                yield return new WaitForSeconds(1f);
            }
        }
    }
}

class WaveInfo {

}