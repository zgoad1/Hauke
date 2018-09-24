using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyInfo {
    public Enemy enemyType;
    public int amount;
    [Range(0, 1)] public float spawnTime;
}
