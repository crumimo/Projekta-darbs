using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;

    public GameState GameState;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ObstacleStateManager.RestoreObstacles(); // Restore the state of obstacles
    }

    public bool CheckpointActivated(int checkpointID)
    {
        return CheckpointManager.IsCheckpointActivated(checkpointID);
    }

    public void ActivateCheckpoint(int checkpointID)
    {
        CheckpointManager.ActivateCheckpoint(checkpointID);
        ObstacleStateManager.SaveCheckpoint(); // Save the state of destroyed obstacles
    }

    public void ResetDestroyedObstacles()
    {
        ObstacleStateManager.ResetDestroyedObstacles();
    }

    public void RestoreObstacles()
    {
        ObstacleStateManager.RestoreObstacles();
    }

    public void RestoreCheckpointState()
    {
        ObstacleStateManager.RestoreCheckpointState(); // Восстановление состояния препятствий из чекпоинта
    }
}