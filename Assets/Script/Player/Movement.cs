using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private float accelerationSmoothFactor = 5f;
    private HashSet<System.Type> activeEffects = new HashSet<System.Type>();
    private Vector2 movement;
    private Rigidbody2D rb;
    private bool isDead = false;
    private bool isPaused = false;
    private Animator animator;
    private float baseSpeed;
    private float baseAnimSpeed;

    public DialogueUI DialogueUI => dialogueUI;
    
    public IInteractable Interactable { get; set; }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        baseSpeed = speed;           // запомнили исходную скорость
        baseAnimSpeed = animator.speed; // обычно = 1
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
    
    private void ClearAllEffects()
    {
        if (HasActiveEffect<GaleStride>())
        {
            speed /= 2f; // или используй конкретное значение, если оно фиксированное
            animator.speed /= 2f;
            UnregisterActiveEffect<GaleStride>();
        }
    }


    public void Die()
    {
        isDead = true;
        movement = Vector2.zero;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Die");
        
        ClearAllEffects();

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
        speed = 5f; 
        animator.speed = 1f;
        StopAllCoroutines();

        StartCoroutine(HandleRespawn());
    }

    private IEnumerator HandleRespawn()
    {
        FadeController fade = FindObjectOfType<FadeController>();
        
        if (fade != null)
        {
            yield return StartCoroutine(fade.FadeIn());
        }
        
        isDead = false;
        speed = baseSpeed;
        animator.speed = baseAnimSpeed;
    
        GameState gameState = GameSession.Instance.GameState;
        transform.position = gameState.PlayerPosition;
        animator.SetTrigger("Respawn");
        
        yield return new WaitForSeconds(0.2f);
        
        if (fade != null)
        {
            yield return StartCoroutine(fade.FadeOut());
        }
        
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
    

    public void RegisterActiveEffect<T>() where T : EffectBase
    {
        activeEffects.Add(typeof(T));
    }

    public void UnregisterActiveEffect<T>() where T : EffectBase
    {
        activeEffects.Remove(typeof(T));
    }

    public bool HasActiveEffect<T>() where T : EffectBase
    {
        return activeEffects.Contains(typeof(T));
    }

}
