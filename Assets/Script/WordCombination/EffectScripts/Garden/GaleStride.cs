using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "GaleStrideEffect", menuName = "Effects/GaleStride")]
public class GaleStride : EffectBase
{
    [Tooltip("Multiplier for increasing the player's speed.")]
    public float speedMultiplier = 2f;
    
    [Tooltip("Duration of the speed boost effect (in seconds).")]
    public float duration = 5f;

    public override void Apply(GameObject target)
    {
        Movement playerMovement = target.GetComponent<Movement>();
        Animator playerAnimator = target.GetComponent<Animator>();

        if (playerMovement != null)
        {
            if (playerMovement.HasActiveEffect<GaleStride>())
            {
                Debug.Log("GaleStride is already active, skipping duplicate application.");
                return;
            }

            playerMovement.RegisterActiveEffect<GaleStride>();

            float originalSpeed = playerMovement.speed;
            playerMovement.speed = originalSpeed * speedMultiplier;

            if (playerAnimator != null)
            {
                float originalAnimSpeed = playerAnimator.speed;
                playerAnimator.speed = originalAnimSpeed * speedMultiplier;
            
                playerMovement.StartCoroutine(RestoreSpeedAfterDelay(playerMovement, originalSpeed, playerAnimator, originalAnimSpeed));
            }
            else
            {
                playerMovement.StartCoroutine(RestoreSpeedAfterDelay(playerMovement, originalSpeed));
            }
        }
        else
        {
            Debug.LogError("Movement component not found on the target GameObject.");
        }
    }


    private IEnumerator RestoreSpeedAfterDelay(Movement playerMovement, float originalSpeed, Animator playerAnimator = null, float originalAnimSpeed = 1f)
    {
        yield return new WaitForSeconds(duration);

        if (playerMovement != null)
        {
            playerMovement.speed = originalSpeed;
            playerMovement.UnregisterActiveEffect<GaleStride>();
        }
        if (playerAnimator != null)
        {
            playerAnimator.speed = originalAnimSpeed;
        }
    }

}
