using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyStateManager 
{
    private static HashSet<int> sleepEnemy = new HashSet<int>();
    private static HashSet<int> checkpointSleepEnemy = new HashSet<int>();
    
    private static HashSet<int> killedEnemies = new HashSet<int>();
    private static HashSet<int> checkpointKilledEnemies = new HashSet<int>();
    
    public static void MarkEnemyAsSleep(int obstacleID)
    {
        if (!sleepEnemy.Contains(obstacleID))
        {
            sleepEnemy.Add(obstacleID);
        }
    }
    
    public static void MarkEnemyAsKilled(int enemyID)
    {
        if (!killedEnemies.Contains(enemyID))
        {
            killedEnemies.Add(enemyID);
        }
    }
    
    public static bool IsEnemySleep(int obstacleID)
    {
        return sleepEnemy.Contains(obstacleID);
    }
    
    public static bool IsEnemyKilled(int enemyID)
    {
        return killedEnemies.Contains(enemyID);
    }
    
    public static void SaveCheckpoint()
    {
        checkpointSleepEnemy = new HashSet<int>(sleepEnemy);
        checkpointKilledEnemies = new HashSet<int>(killedEnemies);
    }

    
    public static void ResetEnemyStates()
    {
        sleepEnemy.Clear();
        killedEnemies.Clear();
    }
    
    public static void RestoreEnemy()
    {
        foreach (var enemy in GameObject.FindObjectsOfType<PatrolEnemy>())
        {
            if (killedEnemies.Contains(enemy.enemyID))
            {
                enemy.KillEnemy();
            }
            else if (sleepEnemy.Contains(enemy.enemyID))
            {
                enemy.ApplyPermanentSleep();
            }
            else
            {
                enemy.ResetEnemy();
            }
        }
    }
    
    public static void FullReset()
    {
        sleepEnemy.Clear();
        killedEnemies.Clear();
        checkpointSleepEnemy.Clear();
        checkpointKilledEnemies.Clear();
        
        foreach (var enemy in GameObject.FindObjectsOfType<PatrolEnemy>(true)) 
        {
            enemy.gameObject.SetActive(true);
            enemy.ResetEnemy();
        }
    }
    public static void RestoreCheckpointState()
    {
        sleepEnemy = new HashSet<int>(checkpointSleepEnemy);
        killedEnemies = new HashSet<int>(checkpointKilledEnemies);
        RestoreEnemy();
    }
}
