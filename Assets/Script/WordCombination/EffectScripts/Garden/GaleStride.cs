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
        // Get the Movement and Animator components from the target (player)
        Movement playerMovement = target.GetComponent<Movement>();
        Animator playerAnimator = target.GetComponent<Animator>();

        if (playerMovement != null)
        {
            // Save the original movement speed
            float originalSpeed = playerMovement.speed;
            // Increase the player's speed
            playerMovement.speed = originalSpeed * speedMultiplier;

            // Optionally, adjust the player's animation speed
            if (playerAnimator != null)
            {
                // Save the original animator speed
                float originalAnimSpeed = playerAnimator.speed;
                // Increase the animator speed accordingly
                playerAnimator.speed = originalAnimSpeed * speedMultiplier;
                
                // Start a coroutine to restore both speed and animator speed after the duration
                playerMovement.StartCoroutine(RestoreSpeedAfterDelay(playerMovement, originalSpeed, playerAnimator, originalAnimSpeed));
            }
            else
            {
                // If no Animator is found, restore only the movement speed
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
            // Restore the original movement speed
            playerMovement.speed = originalSpeed;
        }
        if (playerAnimator != null)
        {
            // Restore the original animator speed
            playerAnimator.speed = originalAnimSpeed;
        }
    }
}
