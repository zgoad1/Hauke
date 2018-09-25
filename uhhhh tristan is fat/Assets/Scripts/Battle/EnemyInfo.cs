using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyInfo {
    public GameObject enemyPrefab;
    public int amount;
    [Range(0, 1)] public float spawnStart;
    [Range(0, 1)] public float spawnEnd = 1;
    public bool randomDist = false; // whether the enemies should be randomly distributed across the timespan, or uniformly
    [SerializeField] private GameObject spawnPointGroup;
    [HideInInspector] public SpawnPoint[] spawnPoints {
        get {
            return spawnPointGroup.GetComponentsInChildren<SpawnPoint>();
        }
    }
}
