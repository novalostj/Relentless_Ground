using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Enemy
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private List<GameObject> enemies;
        [SerializeField] private float spawnInterval = 2f;

        private void Start()
        {
            StartCoroutine(SpawnInterval());
        }

        private IEnumerator SpawnInterval()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval);

                Instantiate(RandomEnemy(), Vector3.zero, quaternion.identity);
            }
        }

        private GameObject RandomEnemy()
        {
            int randomNum = UnityEngine.Random.Range(0, enemies.Count);
            
            GameObject randomEnemy = enemies[randomNum];
            
            return randomEnemy;
        }
        
        
    }
}