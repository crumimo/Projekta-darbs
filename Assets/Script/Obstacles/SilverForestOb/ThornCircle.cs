using System.Collections;
using UnityEngine;

public class ThornCircle : MonoBehaviour
{
    private Transform playerTransform;
    private float duration;
    private float rotationSpeed;

    public void Initialize(Transform playerTransform, float duration, float rotationSpeed)
    {
        this.playerTransform = playerTransform;
        this.duration = duration;
        this.rotationSpeed = rotationSpeed;

        Debug.Log("Initializing ThornCircle at position: " + playerTransform.position);

        // Start coroutine to scale the object up
        StartCoroutine(ScaleOverTime(0.5f, 1.4f, true)); // Duration of scaling up can be adjusted

        StartCoroutine(DestroyAfterDuration(duration));
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        
        if (playerTransform != null)
        {
            transform.position = playerTransform.position; 
        }
    }

    private IEnumerator DestroyAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration - 0.5f); // Subtract time for scaling down
        StartCoroutine(ScaleOverTime(0.5f, 0f, false)); // Duration of scaling down can be adjusted
        yield return new WaitForSeconds(0.5f); // Wait for scaling down to complete
        Destroy(gameObject);
    }

    private IEnumerator ScaleOverTime(float duration, float targetScale, bool scalingUp)
    {
        Vector3 initialScale = scalingUp ? Vector3.zero : transform.localScale;
        Vector3 finalScale = scalingUp ? new Vector3(targetScale, targetScale, targetScale) : Vector3.zero;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = finalScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            var obstacleManager = collision.GetComponent<ObstacleManager>();
            if (obstacleManager != null && obstacleManager.CanBeDestroyedByEffect(ScriptableObject.CreateInstance<SpikeCircleEffect>()))
            {
                StartCoroutine(DisableObstacle(obstacleManager));
            }
        }
        else if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.SetActive(false);
        }
    }

    private IEnumerator DisableObstacle(ObstacleManager obstacleManager)
    {
        obstacleManager.DisableObstacle();
        yield return null;
    }
}