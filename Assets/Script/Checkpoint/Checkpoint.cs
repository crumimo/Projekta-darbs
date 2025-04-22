using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Sprite activeSprite;
    public int checkpointID;
    private bool isActive = false;
    public TextMeshProUGUI checkpointNotification;
    private SpriteRenderer sprite;

    private void Start()
    {
        checkpointNotification.enabled = false;
        sprite = GetComponent<SpriteRenderer>();

        if (CheckpointManager.IsCheckpointActivated(checkpointID))
        {
            isActive = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            sprite.sprite = activeSprite;
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
        ObstacleStateManager.SaveCheckpoint();
        EnemyStateManager.SaveCheckpoint(); 
        WordUIManager.Instance.SaveCheckpoint();
        FishStateManager.SaveCheckpoint();
        InteractableStateManager.SaveCheckpoint(); 
    }

    private IEnumerator ShowCheckpointNotification()
    {
        checkpointNotification.enabled = true;
        checkpointNotification.text = "Checkpoint Reached! Progress Saved.";
        yield return new WaitForSeconds(3);
        checkpointNotification.enabled = false;
    }
}