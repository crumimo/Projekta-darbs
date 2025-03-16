using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private PatrolEnemyVision[] patrolEnemies;
    
    
    private void Start()
    {
        patrolEnemies = FindObjectsOfType<PatrolEnemyVision>();
    }

    public void ResetEnemies()
    {
        foreach (var enemy in patrolEnemies)
        {
            enemy.ResetEnemyState();
        }
    }
}
