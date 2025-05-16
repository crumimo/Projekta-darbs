using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KillZone : MonoBehaviour
{
    [Header("Zone ID for Saving State")]
    public int zoneID;  

    [Header("Effect Settings")]
    [SerializeField] private Collider2D effectCollider;

    [Header("Damage Settings")]
    [SerializeField] private float slowMultiplier = 0.5f;
    [SerializeField] private float deathDelay = 2f;

    [Header("UI Settings")]
    [SerializeField] private Canvas killZoneCanvas;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeSpeed = 1f;
    
    private Movement playerMovement;
    private float originalSpeed;
    private Coroutine deathRoutine;
    private Coroutine fadeRoutine;

    void Start()
    {
        if (killZoneCanvas != null)
        {
            killZoneCanvas.gameObject.SetActive(false);
        }
    }

    public void OnDamageTriggerEnter(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerMovement = collision.GetComponent<Movement>();
            if (playerMovement != null)
            {
                originalSpeed = playerMovement.speed;
                playerMovement.speed *= slowMultiplier;
                deathRoutine = StartCoroutine(DeathCountdown());
            }

            if (killZoneCanvas != null && fadeImage != null)
            {
                killZoneCanvas.gameObject.SetActive(true);
                fadeRoutine = StartCoroutine(FadeInEffect());
            }
        }
    }

    public void OnDamageTriggerExit(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (playerMovement != null)
            {
                playerMovement.speed = originalSpeed;
            }

            if (deathRoutine != null)
            {
                StopCoroutine(deathRoutine);
                deathRoutine = null;
            }

            playerMovement = null;

            if (killZoneCanvas != null && fadeImage != null)
            {
                if (fadeRoutine != null)
                {
                    StopCoroutine(fadeRoutine);
                }
                StartCoroutine(FadeOutEffect());
            }
        }
    }

    private IEnumerator DeathCountdown()
    {
        yield return new WaitForSeconds(deathDelay);
        if (playerMovement != null)
        {
            playerMovement.Die();
        }
    }

    private IEnumerator FadeInEffect()
    {
        Color color = fadeImage.color;
        float alpha = 0f;
        fadeImage.color = new Color(color.r, color.g, color.b, alpha);

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }

    private IEnumerator FadeOutEffect()
    {
        Color color = fadeImage.color;
        float alpha = fadeImage.color.a;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        killZoneCanvas.gameObject.SetActive(false);
    }

    public void DisableZone()
    {
        if (deathRoutine != null)
        {
            StopCoroutine(deathRoutine);
            deathRoutine = null;
        }

        if (playerMovement != null)
        {
            playerMovement.speed = originalSpeed;
            playerMovement = null;
        }

        if (killZoneCanvas != null)
        {
            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }
            StartCoroutine(FadeOutEffect());
        }

        gameObject.SetActive(false);
        
        ObstacleStateManager.MarkObstacleAsDestroyed(zoneID);
        Debug.Log($"KillZone '{gameObject.name}' disabled and saved with ID {zoneID}.");
    }

    public void ResetZone()
    {
        gameObject.SetActive(true);
        Debug.Log($"KillZone '{gameObject.name}' reset/enabled.");
    }

    public bool CanReceiveEffect(Vector3 playerPosition, float effectRadius, EffectBase effect)
    {
        Vector2 center = effectCollider != null ? (Vector2)effectCollider.bounds.center : (Vector2)transform.position;
        float distance = Vector2.Distance(center, playerPosition);
        Debug.Log($"{gameObject.name}: KillZone center = {center}, player = {playerPosition}, distance = {distance:F2}, radius = {effectRadius}");
        return distance <= effectRadius;
    }
}
