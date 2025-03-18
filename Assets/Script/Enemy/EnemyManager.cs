using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private PatrolEnemy[] patrolEnemies;
    
    private void Start()
    {
        patrolEnemies = FindObjectsOfType<PatrolEnemy>();
    }

    public void ResetEnemies()
    {
        foreach (var enemy in patrolEnemies)
        {
            enemy.ResetEnemyState();
        }
    }

    public void PauseEnemies()
    {
        foreach (var enemy in patrolEnemies)
        {
            enemy.enabled = false;
        }
    }

    public void ResumeEnemies()
    {
        foreach (var enemy in patrolEnemies)
        {
            enemy.enabled = true;
        }
    }
}