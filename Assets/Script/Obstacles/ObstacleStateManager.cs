using System.Collections.Generic;
using UnityEngine;

public static class ObstacleStateManager
{
    private static HashSet<int> destroyedObstacles = new HashSet<int>();
    private static HashSet<int> checkpointDestroyedObstacles = new HashSet<int>();
    
    private static Dictionary<int, bool> switchedObstacles = new Dictionary<int, bool>();
    private static Dictionary<int, bool> checkpointSwitchedObstacles = new Dictionary<int, bool>();

    public static void MarkObstacleAsDestroyed(int obstacleID)
    {
        if (!destroyedObstacles.Contains(obstacleID))
        {
            destroyedObstacles.Add(obstacleID);
        }
    }

    public static bool IsObstacleDestroyed(int obstacleID)
    {
        return destroyedObstacles.Contains(obstacleID);
    }

    public static void SaveCheckpoint()
    {
        checkpointDestroyedObstacles = new HashSet<int>(destroyedObstacles);
        checkpointSwitchedObstacles = new Dictionary<int, bool>(switchedObstacles);
    }

    public static void ResetDestroyedObstacles()
    {
        destroyedObstacles.Clear();
    }

    public static void RestoreObstacles()
    {
        foreach (var obstacle in GameObject.FindObjectsOfType<ObstacleManager>())
        {
            if (destroyedObstacles.Contains(obstacle.obstacleID))
            {
                obstacle.gameObject.SetActive(false);
            }
            else
            {
                obstacle.ResetObstacle();
            }
        }
        foreach (var dual in GameObject.FindObjectsOfType<DualObstacle>())
        {
            if (switchedObstacles.TryGetValue(dual.obstacleID, out bool isSwitched))
            {
                dual.ApplySwitchedState(isSwitched);
            }
            else
            {
                dual.ApplySwitchedState(false);
            }
        }
        foreach (var kz in GameObject.FindObjectsOfType<KillZone>(true))
        {
            if (destroyedObstacles.Contains(kz.zoneID))
            {
                kz.gameObject.SetActive(false);
            }
            else
            {
                kz.ResetZone();
            }
        }
        foreach (var activator in GameObject.FindObjectsOfType<KillZoneActivator>(true))
        {
            activator.ResetActivator();
        }

    }
    
    public static void RestoreNests()
    {
        foreach (var nest in GameObject.FindObjectsOfType<HollowNestController>(true))
        {
            nest.ResetNest();
        }
    }


    public static void RestoreCheckpointState()
    {
        destroyedObstacles = new HashSet<int>(checkpointDestroyedObstacles);
        switchedObstacles = new Dictionary<int, bool>(checkpointSwitchedObstacles);
        RestoreObstacles();  
    }
    
    public static void ResetSwitchedObstacles()
    {
        switchedObstacles.Clear();
    }

    public static void MarkObstacleSwitchedState(int obstacleID, bool isSwitched)
    {
        switchedObstacles[obstacleID] = isSwitched;
    }
}