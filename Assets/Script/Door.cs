using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool canEnter; 

    [SerializeField] private HintPanelController hintPanelController; 
    [SerializeField] private Transform destination; 
    [SerializeField] private FadeController fadeController; 

    private void Update()
    {
        
        if (canEnter && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(EnterDoorWithFade());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hintPanelController.Show("Press F to enter"); 
            canEnter = true; 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hintPanelController.Hide(); 
            canEnter = false; 
        }
    }

    private IEnumerator EnterDoorWithFade()
    {
        hintPanelController.Hide();
        
        yield return StartCoroutine(fadeController.FadeIn());
        
        Movement playerPos = FindObjectOfType<Movement>();
        if (playerPos != null)
        {
            playerPos.transform.position = destination.position;
        }
        
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(fadeController.FadeOut());
    }
}