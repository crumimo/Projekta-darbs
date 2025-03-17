using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID; 
    private bool isActive = false; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            isActive = true; 
            SaveCheckpoint(other.transform);
            Debug.Log("Checkpoint activated: " + checkpointID);
        }
    }

    private void SaveCheckpoint(Transform playerTransform)
    {
        GameState gameState = GameSession.Instance.GameState;
        gameState.PlayerPosition = playerTransform.position;
        gameState.currentCheckpointID = checkpointID;
        WordUIManager.Instance.SaveCheckpoint(); 
    }
}