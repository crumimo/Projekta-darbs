using System.Collections;
using UnityEngine;

public class KillZone : MonoBehaviour, IEffectable
{
    [Header("Zone ID for Saving State")]
    public int zoneID;  

    [Header("Effect Settings (Parent)")]
    [SerializeField] private Collider2D effectCollider;

    [Header("Damage Settings (Forwarded from Child)")]
    [SerializeField] private float slowMultiplier = 0.5f;
    [SerializeField] private float deathDelay = 2f;

    private Movement playerMovement;
    private float originalSpeed;
    private Coroutine deathRoutine;
    
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

    public void ApplyEffect(EffectBase effect)
    {
        if (effect is VerdantSurge)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                return;
            }
            if (effectCollider == null)
            {
                return;
            }
            if (effectCollider.OverlapPoint(player.transform.position))
            {
                DisableZone();
            }
        }
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
