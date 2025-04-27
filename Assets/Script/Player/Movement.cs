using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private float accelerationSmoothFactor = 5f;
    private Vector2 movement;
    private Rigidbody2D rb;
    private bool isDead = false;
    private bool isPaused = false;
    private Animator animator;
    
    public DialogueUI DialogueUI => dialogueUI;
    
    public IInteractable Interactable { get; set; }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (dialogueUI.IsOpen) return;
        if (!isDead && !isPaused)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            
            float dampTime = 0.20f;
            animator.SetFloat("Horizontal", movement.x, dampTime, Time.deltaTime);
            animator.SetFloat("Vertical", movement.y, dampTime, Time.deltaTime);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
    
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interactable?.Interact(this);
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && !isPaused)
        {
            Vector2 targetVelocity = movement.normalized * speed;
            rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, accelerationSmoothFactor * Time.fixedDeltaTime);
            
            if (movement != Vector2.zero)
            {
                animator.SetFloat("LastHorizontal", movement.x);
                animator.SetFloat("LastVertical", movement.y);
            }
        }
    }

    public void Die()
    {
        isDead = true;
        movement = Vector2.zero;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Die");

        WordUIManager.Instance.ResetToCheckpoint();
        WordUIManager.Instance.RestoreCollectedWordsOnScene();
        WordUIManager.Instance.UpdateButtons();
        
        if (!GameSession.Instance.CheckpointActivated(GameSession.Instance.GameState.currentCheckpointID))
        {
            EnemyStateManager.FullReset();
            GameSession.Instance.ResetDestroyedObstacles();
            InteractableStateManager.ResetInteractableStates();
            ObstacleStateManager.RestoreObstacles();
            ObstacleStateManager.ResetSwitchedObstacles();
            ObstacleStateManager.RestoreNests();
            
            foreach (InteractableObject obj in GameObject.FindObjectsOfType<InteractableObject>())
            {
                obj.ResetInteractableObject();
            }
        
            foreach (Fish fish in GameObject.FindObjectsOfType<Fish>())
            {
                fish.ResetFishState();
            }
        }
        else
        {
            EnemyStateManager.RestoreCheckpointState();
            FishStateManager.RestoreCheckpointState();
            InteractableStateManager.RestoreCheckpointState();
        }
    
        Invoke("Respawn", 1f);
    }

    private void Respawn()
    {
        isDead = false;
        GameState gameState = GameSession.Instance.GameState;
        transform.position = gameState.PlayerPosition;
        animator.SetTrigger("Respawn");
        
        FishStateManager.RestoreCheckpointState();
        
        if (GameSession.Instance.CheckpointActivated(gameState.currentCheckpointID))
        {
            InteractableStateManager.RestoreCheckpointState();
        }
    }


    public void EnableMovement()
    {
        isPaused = false;
    }

    public void DisableMovement()
    {
        isPaused = true;
        animator.SetFloat("Speed", 0f);
        movement = Vector2.zero;
        rb.velocity = Vector2.zero;
    }
}
