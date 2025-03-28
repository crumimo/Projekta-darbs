using UnityEngine;

public class Fish : MonoBehaviour
{
    public FishBehavior behavior; // Reference to the ScriptableObject
    public Transform playerTransform; // Reference to the player's transform
    public GameObject wordObject; // The word object to be activated
    public float distanceToActivate = 10f; // Distance within which the effect can be activated
    public SceneWordData sceneWordData; // Reference to the SceneWordData ScriptableObject

    [HideInInspector]
    public Vector3 originalPosition; // The original position of the fish

    public bool isCorrectCombination { get; private set; } = false; // Flag for the correct combination
    public bool isWithinPlayerTrigger { get; private set; } = false; // Flag for being inside the player's trigger collider
    public bool isReturningToOrigin { get; private set; } = false; // Flag for returning to the original position
    private bool hasDeliveredWord = false; // Flag to indicate if the fish has already delivered the word

    void Start()
    {
        originalPosition = transform.position;
        wordObject.SetActive(false); // Hide the word object at the start

        // Ensure sceneWordData is assigned
        if (sceneWordData == null)
        {
            Debug.LogError("SceneWordData is not assigned in the inspector.");
        }
    }

    void Update()
    {
        if (!isCorrectCombination && !hasDeliveredWord)
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
        else if (!isWithinPlayerTrigger && !hasDeliveredWord)
        {
            MoveToPosition(playerTransform.position);
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, behavior.speed * Time.deltaTime);
    }

    public void ApplyCorrectCombination(string word1, string word2)
    {
        if (IsValidCombination(word1, word2) && !hasDeliveredWord)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player not found!");
                return;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer > distanceToActivate)
            {
                Debug.Log("Player is too far away to apply the effect.");
                return;
            }

            isCorrectCombination = true;
            wordObject.SetActive(true); // Activate the word object
        }
    }

    private bool IsValidCombination(string word1, string word2)
    {
        if (sceneWordData == null)
        {
            Debug.LogError("SceneWordData is not assigned.");
            return false;
        }

        foreach (var combination in sceneWordData.combinations)
        {
            if ((combination.word1 == word1 && combination.word2 == word2) ||
                (combination.word1 == word2 && combination.word2 == word1))
            {
                return true;
            }
        }
        return false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCorrectCombination && other.CompareTag("Player"))
        {
            isWithinPlayerTrigger = true;
            isReturningToOrigin = true; // Set the flag to return to the original position

            // Set the flag to indicate the fish has delivered the word
            hasDeliveredWord = true;
        }
    }
}