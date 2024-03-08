using System.Collections;
using System.Collections.Generic;
using Project_Anxiety.Game.Units;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnLocations;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int amountToPool;
    [SerializeField] private float spawnTime;
    [SerializeField] private Transform parent;
    [SerializeField] private List<GameObject> enemyPool;

    public void Awake()
    {
        enemyPool = new List<GameObject>();
        GameObject tmp;
        for (var i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(enemyPrefab, parent);
            tmp.name = enemyPrefab.name;
            tmp.SetActive(false);
            enemyPool.Add(tmp);
        }

        StartCoroutine(RespawnTimer());
    }

    private GameObject GetPooledEnemy()
    {
        for (var i = 0; i < amountToPool; i++)
            if (!enemyPool[i].activeSelf)
                return enemyPool[i];

        return null;
    }

    private IEnumerator RespawnTimer()
    {
        while (true)
        {
            var respawnEnemy = GetPooledEnemy();
            if (respawnEnemy != null)
            {
                var randomSpawn = Random.Range(0, spawnLocations.Count);

                yield return new WaitForSeconds(spawnTime);

                respawnEnemy.transform.position = spawnLocations[randomSpawn].transform.position;
                respawnEnemy.SetActive(true);
                respawnEnemy.GetComponent<Health>().Revive();
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }
}