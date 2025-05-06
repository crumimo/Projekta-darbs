using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;
    public GameState GameState;

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else 
            Destroy(gameObject);
    }

    private void Start()
    {
        FishStateManager.RestoreCheckpointState();
        ObstacleStateManager.RestoreObstacles(); 
        EnemyStateManager.RestoreEnemy(); 
        InteractableStateManager.RestoreCheckpointState(); 
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
        InteractableStateManager.SaveCheckpoint();
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
        InteractableStateManager.RestoreCheckpointState();
    }
    
    public void ResetGameState()
    {
        GameState.PlayerPosition = Vector3.zero; 
        GameState.currentCheckpointID = -1;
    }

}