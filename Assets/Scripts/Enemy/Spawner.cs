using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Particles;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private List<GameObject> enemies;
        [SerializeField] private float spawnInterval = 10f;
        [SerializeField] private int spawnAmount = 1;
        [SerializeField] private float spawnRadius = 2f;

        public float LocalSpawnAmount { get; private set; }
        public float LocalTimer { get; private set; }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
#endif
        private void Start()
        {
            LocalSpawnAmount = spawnAmount;
        }


        private void Update()
        {
            LocalTimer -= Time.deltaTime;
            
            if (LocalTimer <= 0)
            {
                SummonMonsters();
                LocalTimer += spawnInterval;
                LocalSpawnAmount++;
            }
            else if (transform.childCount == 0)
            {
                SummonMonsters();
                LocalTimer += spawnInterval;
                LocalSpawnAmount++;
            }
        }

        private void SummonMonsters()
        {
            for (int i = 0; i < LocalSpawnAmount; i++)
            {
                Vector2 randomCirclePoint = Random.insideUnitCircle;
                Vector3 localSpawnPosition = new Vector3(randomCirclePoint.x, 0, randomCirclePoint.y) * spawnRadius;
                GameObject spawnedEnemy = Instantiate(RandomEnemy(), transform.position + localSpawnPosition, quaternion.identity);
                spawnedEnemy.transform.parent = transform;
            }
        }
        
        private GameObject RandomEnemy()
        {
            int randomNum = Random.Range(0, enemies.Count);
            
            GameObject randomEnemy = enemies[randomNum];
            
            return randomEnemy;
        }
        
        
    }
}