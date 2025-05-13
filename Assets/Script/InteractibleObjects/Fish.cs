using UnityEngine;

public class Fish : MonoBehaviour, IEffectable
{
    public int fishID;
    public Transform playerTransform; // Reference to the player's transform
    public float distanceToActivate = 10f; // Distance within which the fish will react
    public float speed = 2f; // Speed at which the fish moves
    public float stopDistance = 0.1f; // Distance to stop movement
    public float returnDistance = 15f; // Distance at which the fish will return to its original position
    public float distanceToAcceptCombination = 5f; // Distance within which the fish will accept the combination
    public GameObject wordObject; // Reference to the word object

    private Vector3 originalPosition; // The original position of the fish
    private bool isReturningToOrigin = false; // Flag for returning to the original position
    private bool isCorrectCombination = false; // Flag for the correct combination
    private bool isMovingAwayFromPlayer = false; // Flag for moving away from the player
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        originalPosition = transform.position;

        bool isCollected = FishStateManager.IsWordCollected(fishID);
        wordObject.SetActive(isCollected);
    
        Debug.Log($"Fish {fishID} started. Word active: {wordObject.activeSelf}");
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (isCorrectCombination)
        {
            // Correct combination applied, return to original position and stay there
            MoveToPosition(originalPosition);
            if (Vector3.Distance(transform.position, originalPosition) < stopDistance)
            {
                isCorrectCombination = false; // Reset combination flag after returning
                wordObject.SetActive(true); // Show the word object
            }
        }
        else if (distanceToPlayer < distanceToActivate)
        {
            // Player is close, move away from player
            isMovingAwayFromPlayer = true;
            isReturningToOrigin = false;
            Vector3 direction = GetHorizontalDirection(playerTransform.position);
            Vector3 targetPosition = transform.position + direction * speed * Time.deltaTime;
            MoveToPosition(targetPosition);
        }
        else if (distanceToPlayer > returnDistance)
        {
            // Player is far, return to original position
            isReturningToOrigin = true;
            isMovingAwayFromPlayer = false;
        }

        if (isReturningToOrigin)
        {
            MoveToPosition(originalPosition);
            if (Vector3.Distance(transform.position, originalPosition) < stopDistance)
            {
                isReturningToOrigin = false;
            }
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        if (Vector3.Distance(transform.position, position) > stopDistance)
        {
            Vector3 direction = (position - transform.position).normalized;

            // Set animation parameters based on direction
            if (direction.x > 0)
            {
                anim.SetBool("isSwimmingRight", true);
                anim.SetBool("isSwimmingLeft", false);
            }
            else if (direction.x < 0)
            {
                anim.SetBool("isSwimmingRight", false);
                anim.SetBool("isSwimmingLeft", true);
            }

            transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
        }
        else
        {
            // Stop animation when fish stops moving
            anim.SetBool("isSwimmingRight", false);
            anim.SetBool("isSwimmingLeft", false);
        }
    }

    public bool ApplyEffect(EffectBase effect)
    {
        effect.Apply(gameObject);
        return true;
    }

    public void ApplyCorrectCombination()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > distanceToAcceptCombination)
        {
            Debug.Log("Player is too far away to apply the combination.");
            return;
        }

        if (!isCorrectCombination)
        {
            isCorrectCombination = true;
            Debug.Log("Correct combination applied. Word object should be activated.");
        
            if (wordObject != null)
            {
                wordObject.SetActive(true);
                Debug.Log("Word object activated successfully.");
            }
        }
        if (!FishStateManager.IsWordCollected(gameObject.GetInstanceID()))
        {
            FishStateManager.MarkWordCollected(gameObject.GetInstanceID());
            wordObject.SetActive(true);
        }
    }

    private Vector3 GetHorizontalDirection(Vector3 targetPosition)
    {
        Vector3 direction = (transform.position - targetPosition).normalized;
        return new Vector3(Mathf.Sign(direction.x), 0, 0); // Move left or right
    }
    
    public void ResetFishState()
    {
        isCorrectCombination = false; 
        isMovingAwayFromPlayer = false; 
        isReturningToOrigin = true; 

        wordObject.SetActive(false); 
    }
    public bool CanReceiveEffect(Vector3 playerPosition, float effectRadius, EffectBase effect)
    {
        return Vector3.Distance(transform.position, playerPosition) <= effectRadius;
    }
}