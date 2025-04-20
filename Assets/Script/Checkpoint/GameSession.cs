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
        ObstacleStateManager.RestoreObstacles(); 
        EnemyStateManager.RestoreEnemy();// Restore the state of obstacles
    }
    public bool HasAnyCheckpoint()
    {
        return GameState.currentCheckpointID != 0; 
    }
    public bool CheckpointActivated(int checkpointID)
    {
        return CheckpointManager.IsCheckpointActivated(checkpointID);
    }

    public void ActivateCheckpoint(int checkpointID)
    {
        CheckpointManager.ActivateCheckpoint(checkpointID);
        ObstacleStateManager.SaveCheckpoint();
        EnemyStateManager.SaveCheckpoint();
    }

    public void ResetDestroyedObstacles()
    {
        ObstacleStateManager.ResetDestroyedObstacles();
    }
    
    public void ResetEnemy()
    {
        EnemyStateManager.ResetEnemyStates();
    }

    public void RestoreObstacles()
    {
        ObstacleStateManager.RestoreObstacles();
    }
    
    public void RestoreEnemy()
    {
        EnemyStateManager.RestoreEnemy();
    }

    public void RestoreCheckpointState()
    {
        ObstacleStateManager.RestoreCheckpointState(); 
        EnemyStateManager.RestoreCheckpointState();
    }
}