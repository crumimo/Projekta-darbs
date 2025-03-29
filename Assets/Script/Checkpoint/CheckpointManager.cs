using System.Collections.Generic;
using UnityEngine;

public static class CheckpointManager
{
    private static HashSet<int> activatedCheckpoints = new HashSet<int>();
    private static HashSet<GameObject> destroyedObstacles = new HashSet<GameObject>();

    public static bool IsCheckpointActivated(int checkpointID)
    {
        return activatedCheckpoints.Contains(checkpointID);
    }

    public static void ActivateCheckpoint(int checkpointID)
    {
        if (!activatedCheckpoints.Contains(checkpointID))
        {
            activatedCheckpoints.Add(checkpointID);
        }
    }

    public static void MarkObstacleAsDestroyed(GameObject obstacle)
    {
        if (!destroyedObstacles.Contains(obstacle))
        {
            destroyedObstacles.Add(obstacle);
        }
    }

    public static void RestoreObstacles()
    {
        foreach (var obstacle in destroyedObstacles)
        {
            obstacle.SetActive(false);
        }
    }

    public static void SaveCheckpoint()
    {
        foreach (var obstacle in destroyedObstacles)
        {
            // Additional logic to save the state of each destroyed obstacle, if needed
        }
    }
}