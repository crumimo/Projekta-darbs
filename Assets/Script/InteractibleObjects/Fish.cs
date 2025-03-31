using UnityEngine;

public class Fish : MonoBehaviour
{
    public Transform playerTransform; // Reference to the player's transform
    public float distanceToActivate = 10f; // Distance within which the fish will react
    public float speed = 2f; // Speed at which the fish moves
    public float stopDistance = 0.1f; // Distance to stop movement
    public float returnDistance = 15f; // Distance at which the fish will return to its original position
    public float distanceToAcceptCombination = 5f; // Distance within which the fish will accept the combination

    private Vector3 originalPosition; // The original position of the fish
    private bool isReturningToOrigin = false; // Flag for returning to the original position
    private bool isCorrectCombination = false; // Flag for the correct combination
    private bool isMovingToPlayer = false; // Flag for moving towards the player
    private FishBehavior fishBehavior; // Reference to the FishBehavior instance
    public GameObject wordObject; // The word object to be activated
    
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        originalPosition = transform.position;

        // Ensure each fish has its own FishBehavior instance
        fishBehavior = ScriptableObject.CreateInstance<FishBehavior>();
        fishBehavior.SetWordObject(wordObject, playerTransform); // Set the word object and player transform in the behavior
        fishBehavior.SetFish(this); // Set the fish instance in the behavior
    }

    void Update()
    {

        if (isMovingToPlayer)
        {
            MoveToPosition(playerTransform.position);
            if (Vector3.Distance(transform.position, playerTransform.position) < stopDistance)
            {
                isMovingToPlayer = false;
                isReturningToOrigin = true; // Start returning to the original position
            }
        }
        else if (isReturningToOrigin)
        {
            MoveToPosition(originalPosition);
            if (Vector3.Distance(transform.position, originalPosition) < stopDistance)
            {
                isReturningToOrigin = false;
                isCorrectCombination = false; // Reset combination flag after returning
            }
        }
        else if (!isCorrectCombination)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer < distanceToActivate)
            {
                // Player is close, move away from player
                Vector3 direction = GetOrthogonalDirection(playerTransform.position);
                Vector3 targetPosition = transform.position + direction * speed * Time.deltaTime;
                MoveToPosition(targetPosition);
            }
            else if (distanceToPlayer > returnDistance)
            {
                // Player is far, return to original position
                MoveToPosition(originalPosition);
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
            Debug.Log("Correct combination applied.");
            fishBehavior.ActivateWord(); // Activate the word object after the correct combination
        }
    }

    public void StartMovingToPlayer()
    {
        isMovingToPlayer = true;
        isReturningToOrigin = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"OnTriggerEnter2D: {other.name}");

        if (isCorrectCombination && other.CompareTag("Player"))
        {
            isMovingToPlayer = false;
            isReturningToOrigin = true; // Set the flag to return to the original position
        }
    }

    private Vector3 GetOrthogonalDirection(Vector3 targetPosition)
    {
        Vector3 direction = (transform.position - targetPosition).normalized;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return new Vector3(Mathf.Sign(direction.x), 0, 0); // Move left or right
        }
        else
        {
            return new Vector3(0, Mathf.Sign(direction.y), 0); // Move up or down
        }
    }
}