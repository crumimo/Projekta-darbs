using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private PatrolEnemy[] patrolEnemies;
    private HideAndSeekEnemyEye[] hideAndSeekEnemies;
    
    private void Start()
    {
        patrolEnemies = FindObjectsOfType<PatrolEnemy>();
        hideAndSeekEnemies = FindObjectsOfType<HideAndSeekEnemyEye>();
    }

    public void ResetEnemies()
    {
        foreach (var enemy in patrolEnemies)
        {
            enemy.ResetEnemyState();
        }
        foreach (var enemy in hideAndSeekEnemies)
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
        foreach (var enemy in hideAndSeekEnemies)
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
        foreach (var enemy in hideAndSeekEnemies)
        {
            enemy.enabled = true;
        }
    }
}