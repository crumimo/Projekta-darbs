using System.Collections;
using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField] private FadeController fadeController; 
    [SerializeField] private AudioClip transitionSound; 
    [SerializeField] private GameObject[] objectsToDisable; 
    [SerializeField] private GameObject[] objectsToEnable;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float fadeDuration = 0.5f; 

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (fadeController == null)
        {
            fadeController = FindObjectOfType<FadeController>(); 
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(PerformSceneTransition());
            WordUIManager.Instance.ClearCollectedWords();
        }
    }

    private IEnumerator PerformSceneTransition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Movement playerMovement = player.GetComponent<Movement>();
        if (playerMovement != null)
        {
            playerMovement.DisableMovement();
        }

        yield return StartCoroutine(fadeController.FadeIn());

        if (transitionSound != null)
        {
            audioSource.PlayOneShot(transitionSound);
        }

        yield return new WaitForSeconds(0.5f); 
        
        if (objectsToDisable != null)
        {
            player.transform.position = spawnPoint.position;
            foreach (GameObject obj in objectsToDisable)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
        
        if (objectsToEnable != null)
        {
            foreach (GameObject obj in objectsToEnable)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }

        yield return new WaitForSeconds(0.5f); 
        
        yield return StartCoroutine(fadeController.FadeOut());
        
        if (playerMovement != null)
        {
            playerMovement.EnableMovement();
        }
    }
}
