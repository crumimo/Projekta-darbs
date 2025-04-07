using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "GaleStrideEffect", menuName = "Effects/GaleStride")]
public class GaleStrideEffect : EffectBase
{
    public float speedMultiplier; // Speed increase multiplier
    public float duration; // Duration of the effect

    public override void Apply(GameObject target)
    {
        Debug.Log("Applying Gale Stride Effect");
        Movement playerMovement = target.GetComponent<Movement>();
        if (playerMovement != null)
        {
            playerMovement.StartCoroutine(GaleStrideCoroutine(playerMovement));
        }
    }

    private IEnumerator GaleStrideCoroutine(Movement playerMovement)
    {
        // Increase the player's speed
        float originalSpeed = playerMovement.speed;
        playerMovement.speed *= speedMultiplier;

        // Wait for the effect duration
        yield return new WaitForSeconds(duration);

        // Restore the original player's speed
        playerMovement.speed = originalSpeed;
    }
}