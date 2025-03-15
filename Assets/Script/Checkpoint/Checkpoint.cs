using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID; // Уникальный идентификатор чекпоинта
    private bool isActive = false; // Состояние чекпоинта

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActive)
        {
            isActive = true; // Активируем чекпоинт
            SaveCheckpoint(collision.transform);
            Debug.Log("Checkpoint activated: " + checkpointID);
        }
    }

    private void SaveCheckpoint(Transform playerTransform)
    {
        GameState gameState = GameSession.Instance.GameState;
        gameState.PlayerPosition = playerTransform.position;
        gameState.currentCheckpointID = checkpointID;
    }
}