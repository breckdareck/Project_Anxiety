using System.Collections.Generic;
using Project_Anxiety.Game.Units;
using Unity.VisualScripting;
using UnityEngine;

namespace Project_Anxiety.Game.Utility
{
    public class TestEnemySpawner : MonoBehaviour
    {
        [SerializeField] List<GameObject> spawnLocations;
        [SerializeField] Enemy enemyPrefab;
        [SerializeField] int amountToPool;
        [SerializeField] float spawnTime;
        [SerializeField] Transform parent;
        private Spawner<Enemy> enemySpawner;

        private void Awake()
        {
            enemySpawner = new Spawner<Enemy>();
            enemySpawner.SetupSpawner(spawnLocations,enemyPrefab,amountToPool,spawnTime,parent);
            StartCoroutine(enemySpawner.RespawnTimer());
        }
    }
}