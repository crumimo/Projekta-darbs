using UnityEngine;

[CreateAssetMenu(fileName = "FishBehavior", menuName = "Behaviors/FishBehavior")]
public class FishBehavior : ScriptableObject
{
    public float playerDetectionRadius = 5f; // The radius for detecting the player
    public float safeDistance = 10f; // The distance to move away from the player
    public float speed = 2f; // The speed of the fish movement
    public string wordToSpawn; // The word to spawn

    public void ExecuteBehavior(Fish fish, Transform playerTransform)
    {
        float distanceToPlayer = Vector3.Distance(fish.transform.position, playerTransform.position);

        if (!fish.isWithinPlayerTrigger && !fish.isReturningToOrigin)
        {
            if (distanceToPlayer < playerDetectionRadius)
            {
                // Player is too close, fish move away
                Vector3 direction = (fish.transform.position - playerTransform.position).normalized;
                fish.MoveToPosition(fish.transform.position + direction * safeDistance);
            }
            else if (distanceToPlayer > safeDistance)
            {
                // Player is far away, fish move closer to the original position
                fish.MoveToPosition(fish.originalPosition);
            }
        }
    }
}