using System.Collections;
using UnityEngine;

public class KillZone : MonoBehaviour, IEffectable
{
    [SerializeField] private float slowMultiplier = 0.5f;  
    [SerializeField] private float deathDelay = 2f;         

    private Movement playerMovement;
    private float originalSpeed;
    private Coroutine deathRoutine;

    
    private void OnTriggerEnter2D(Collider2D collision)
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
    
    private void OnTriggerExit2D(Collider2D collision)
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
    
    public void DisableZone()
    {
        gameObject.SetActive(false);
        Debug.Log(gameObject.name + " has been disabled by VerdantSurgeEffect.");
    }
    
    public void ApplyEffect(EffectBase effect)
    {
        if (effect is VerdantSurge)
        {
            DisableZone();
        }
    }
}
