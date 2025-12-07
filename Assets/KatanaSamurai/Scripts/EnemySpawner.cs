using UnityEngine;
//using System;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject m_PrefabToSpawn;

    public GameObject[] m_PrefabListToSpawn;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private void Update()
    {

    }

    IEnumerator SpawnEnemies()
    {
        Instantiate(m_PrefabToSpawn);
        yield return new WaitForSeconds(3);
        StartCoroutine(SpawnEnemies());
    }

    public void SpawnSpecificEnemy(int specificEnemy)
    {
        if(specificEnemy == 1)
        {
            int randomTwo = Random.Range(0, 2);

            if(randomTwo == 0)
            {
                Instantiate(m_PrefabListToSpawn[specificEnemy]);
            }
            else
            {
                Instantiate(m_PrefabListToSpawn[3]);
            }
        }
        else
        {
            Instantiate(m_PrefabListToSpawn[specificEnemy]);
        }



    }
}
