using UnityEngine;

public class Fish : MonoBehaviour
{
    public FishBehavior behavior; // Reference to the ScriptableObject
    public Transform playerTransform; // Reference to the player's transform
    public GameObject wordObject; // The word object to be activated

    [HideInInspector]
    public Vector3 originalPosition; // The original position of the fish

    public bool isCorrectCombination { get; private set; } = false; // Flag for the correct combination
    public bool isWithinPlayerTrigger { get; private set; } = false; // Flag for being inside the player's trigger collider
    public bool isReturningToOrigin { get; private set; } = false; // Flag for returning to the original position

    void Start()
    {
        originalPosition = transform.position;
        wordObject.SetActive(false); // Hide the word object at the start
    }

    void Update()
    {
        if (!isCorrectCombination)
        {
            behavior.ExecuteBehavior(this, playerTransform);
        }
        else if (isReturningToOrigin)
        {
            MoveToPosition(originalPosition);
            if (Vector3.Distance(transform.position, originalPosition) < 0.1f)
            {
                isReturningToOrigin = false;
                wordObject.SetActive(false); // Hide the word object after returning
            }
        }
        else if (!isWithinPlayerTrigger)
        {
            MoveToPosition(playerTransform.position);
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, behavior.speed * Time.deltaTime);
    }

    public void ApplyCorrectCombination()
    {
        isCorrectCombination = true;
        wordObject.SetActive(true); // Activate the word object
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCorrectCombination && other.CompareTag("Player"))
        {
            isWithinPlayerTrigger = true;
            isReturningToOrigin = true; // Set the flag to return to the original position
        }
    }
}