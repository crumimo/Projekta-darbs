using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID;
    private bool isActive = false;
    public TextMeshProUGUI checkpointNotification;

    private void Start()
    {
        checkpointNotification.enabled = false;

        if (CheckpointManager.IsCheckpointActivated(checkpointID))
        {
            isActive = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            isActive = true;
            SaveCheckpoint(other.transform);
            Debug.Log("Checkpoint activated: " + checkpointID);
            StartCoroutine(ShowCheckpointNotification());
        }
    }

    private void SaveCheckpoint(Transform playerTransform)
    {
        GameState gameState = GameSession.Instance.GameState;
        gameState.PlayerPosition = playerTransform.position;
        gameState.currentCheckpointID = checkpointID;
        CheckpointManager.ActivateCheckpoint(checkpointID);
        ObstacleStateManager.SaveCheckpoint(); // Save the state of destroyed obstacles
        WordUIManager.Instance.SaveCheckpoint();
    }

    private IEnumerator ShowCheckpointNotification()
    {
        checkpointNotification.enabled = true;
        checkpointNotification.text = "Checkpoint Reached! Progress Saved.";
        yield return new WaitForSeconds(3);
        checkpointNotification.enabled = false;
    }
}