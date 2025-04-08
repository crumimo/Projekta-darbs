using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour
{
    private Transform playerTransform;
    private float duration;
    private SpriteRenderer spriteRenderer;
    public static bool isShieldActive; // Static variable to track shield state

    public void Initialize(Transform playerTransform, float duration)
    {
        this.playerTransform = playerTransform;
        this.duration = duration;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Start coroutine to fade in the shield
        StartCoroutine(FadeOverTime(0.5f, 1f)); // Duration of fading in can be adjusted

        // Start coroutine to destroy the shield after duration
        StartCoroutine(DestroyAfterDuration(duration));
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.position;
        }
    }

    private IEnumerator DestroyAfterDuration(float duration)
    {
        isShieldActive = true; // Activate shield
        yield return new WaitForSeconds(duration - 0.5f); // Subtract time for fading out
        StartCoroutine(FadeOverTime(0.5f, 0f)); // Duration of fading out can be adjusted
        yield return new WaitForSeconds(0.5f); // Wait for fading out to complete
        isShieldActive = false; // Deactivate shield
        Destroy(gameObject);
    }

    private IEnumerator FadeOverTime(float duration, float targetAlpha)
    {
        float startAlpha = spriteRenderer.color.a;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, targetAlpha);
    }
}