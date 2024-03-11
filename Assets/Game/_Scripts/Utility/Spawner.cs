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
        ObjectPool<T> objectPool;
        
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
                T spawnedObject = objectPool.GetPooledObject();
                if (spawnedObject != null)
                {
                    int randomSpawn = Random.Range(0, spawnLocations.Count);

                    float startTime = Time.realtimeSinceStartup;
                    while (Time.realtimeSinceStartup - startTime < spawnTime)
                    {
                        yield return null; // Yield until the specified spawnTime has passed
                    }

                    spawnedObject.transform.position = spawnLocations[randomSpawn].transform.position;
                    spawnedObject.gameObject.SetActive(true);

                    // If the spawned object has a Health component, revive it
                    Health healthComponent = spawnedObject.GetComponent<Health>();
                    if (healthComponent != null)
                    {
                        healthComponent.Revive();
                    }
                }
                else
                {
                    // If no pooled object is available, wait for 1 second before trying again
                    yield return new WaitForSecondsRealtime(1f);
                }
            }
        }
    }
}