using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleWave {
    public int seconds = 90;
    public EnemyInfo[] enemies;
    /*
    public Enemy[] enemyTypes;
    public int[] enemyAmounts;
    [Range(0, 1)] public float[] spawnTime;
    */
}
