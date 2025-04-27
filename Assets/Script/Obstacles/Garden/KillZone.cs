using System.Collections;
using UnityEngine;

public class KillZone : MonoBehaviour, IEffectable
{
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
    }
    
    public bool CanReceiveEffect(Vector3 playerPosition, float effectRadius, EffectBase effect)
    {
        Vector2 zoneCenter = effectCollider != null ? effectCollider.bounds.center : transform.position;
        float distance = Vector2.Distance(zoneCenter, playerPosition);
        Debug.Log($"{gameObject.name}: KillZone center = {zoneCenter}, playerPosition = {playerPosition}, distance = {distance:F2}, effectRadius = {effectRadius}");
        return distance <= effectRadius;
    }

}
