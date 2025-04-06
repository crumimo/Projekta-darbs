using System.Collections.Generic;
using UnityEngine;

public static class ObstacleStateManager
{
    private static HashSet<int> destroyedObstacles = new HashSet<int>();
    private static HashSet<int> checkpointDestroyedObstacles = new HashSet<int>();

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
    }

    public static void RestoreCheckpointState()
    {
        destroyedObstacles = new HashSet<int>(checkpointDestroyedObstacles);
        RestoreObstacles();  // Добавлено для восстановления состояния препятствий после восстановления из чекпоинта
    }
}