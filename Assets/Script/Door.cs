using TMPro;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool canEnter;

    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Transform destination;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (canEnter && Input.GetKeyDown(KeyCode.F))
        {
            hintPanel.SetActive(false);
            text.text = "";
            Movement playerPos = FindObjectOfType<Movement>();
            playerPos.transform.position = destination.position;
        }
        
        if (other.CompareTag("Player"))
        {
            hintPanel.SetActive(true);
            text.text = "Press F to enter";
            canEnter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hintPanel.SetActive(false);
            text.text = "";
            canEnter = false;
        } 
    }
}
