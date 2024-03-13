using System.Collections;
using System.Collections.Generic;
using Project_Anxiety.Game.Units;
using UnityEngine;

namespace Project_Anxiety.Game.Utility
{
    public class Spawner<T> where T : MonoBehaviour
    {
        List<GameObject> spawnLocations;
        T objectPrefab;
        int amountToPool = 50;
        float spawnTime = 1;
        Transform parent;
        
        public ObjectPool<T> objectPool;
        
        public void SetupSpawner(List<GameObject> spawnLocations, T objectPrefab, int amountToPool, float spawnTime, Transform parent)
        {
            this.spawnLocations = spawnLocations;
            this.objectPrefab = objectPrefab;
            this.amountToPool = amountToPool;
            this.spawnTime = spawnTime;
            this.parent = parent;
            objectPool = new ObjectPool<T>(objectPrefab, amountToPool, parent);
        }

        public IEnumerator RespawnTimer()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(spawnTime);
                if (objectPool.GetObjectListCount() <= 0) continue;
                var spawnedObject = objectPool.GetPooledObject();
                if (spawnedObject == null) continue;
                var randomSpawn = Random.Range(0, spawnLocations.Count);
                spawnedObject.transform.position = spawnLocations[randomSpawn].transform.position;
            }
        }
    }
}