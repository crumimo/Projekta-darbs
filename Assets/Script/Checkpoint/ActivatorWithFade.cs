using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorWithFade : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private GameObject[] objectsToDeactivate;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (objectsToActivate != null)
            {
                foreach (GameObject objectt in objectsToActivate)
                {
                    SpriteRenderer spriteRenderer = objectt.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        objectt.SetActive(true); 
                        StartCoroutine(SpriteFadeController.FadeIn(spriteRenderer, 0.5f)); 
                    }
                }
            }

            if (objectsToDeactivate != null)
            {
                foreach (GameObject objectt in objectsToDeactivate)
                {
                    SpriteRenderer spriteRenderer = objectt.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        StartCoroutine(FadeOutAndDisable(objectt, spriteRenderer, 0.5f));
                    }
                }
            }
        }
    }

    private IEnumerator FadeOutAndDisable(GameObject objectToDisable, SpriteRenderer spriteRenderer, float duration)
    {
        yield return StartCoroutine(SpriteFadeController.FadeOut(spriteRenderer, duration)); 
        objectToDisable.SetActive(false); 
    }
}
