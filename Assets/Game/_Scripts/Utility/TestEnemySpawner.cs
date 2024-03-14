using System;
using System.Collections.Generic;
using Project_Anxiety.Game.Units;
using Sirenix.OdinInspector;
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
        Spawner<Enemy> enemySpawner;

        private void Awake()
        {
            enemySpawner = new Spawner<Enemy>();
            enemySpawner.SetupSpawner(spawnLocations,enemyPrefab,amountToPool,spawnTime,parent);
            
            StartCoroutine(enemySpawner.RespawnTimer());
        }

        private void OnEnable()
        {
            enemySpawner.objectPool.OnObjectPulled += OnObjectPulledAction;
            enemySpawner.objectPool.OnObjectReturned += OnObjectReturnedAction;
        }

        private void OnDisable()
        {
            enemySpawner.objectPool.OnObjectPulled -= OnObjectPulledAction;
            enemySpawner.objectPool.OnObjectReturned -= OnObjectReturnedAction;
        }

        private void OnObjectPulledAction(Enemy obj)
        {
            Debug.Log("Object pulled from pool: " + obj.name);
            if (obj.assignedObjectPool == null || obj.assignedObjectPool != enemySpawner.objectPool)
                obj.assignedObjectPool = enemySpawner.objectPool;
            
            obj.Init();
        }

        private void OnObjectReturnedAction(Enemy obj)
        {
            Debug.Log("Object returned to pool: " + obj.name);
        }
        
    }
}