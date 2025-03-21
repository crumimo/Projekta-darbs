using UnityEngine;

public class Altar : MonoBehaviour
{
    private bool isPlayerInRange = false; // Флаг нахождения игрока в зоне взаимодействия

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isPlayerInRange)
        {
            ActivateCollectedWords();
        }
    }

    void ActivateCollectedWords()
    {
        Debug.Log("Activating all collected words.");
        WordUIManager.Instance.ResetCollectedWords();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}