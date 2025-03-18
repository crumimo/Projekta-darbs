using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector2 movement;
    private Rigidbody2D rb;
    private bool isDead = false;
    private bool isPaused = false;
    private Animator animator;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isDead && !isPaused)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && !isPaused)
        {
            if (movement != Vector2.zero)
            {
                animator.SetFloat("LastHorizontal", movement.x);
                animator.SetFloat("LastVertical", movement.y);
            }
            
            rb.MovePosition(rb.position + movement.normalized * (speed * Time.fixedDeltaTime));
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
        
        Invoke("Respawn", 1f);
    }

    private void Respawn()
    {
        isDead = false;
        GameState gameState = GameSession.Instance.GameState;
        transform.position = gameState.PlayerPosition;
        animator.SetTrigger("Respawn");

        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        enemyManager.ResetEnemies();
    }

    public void EnableMovement()
    {
        isPaused = false;
    }

    public void DisableMovement()
    {
        isPaused = true;
        movement = Vector2.zero;
        rb.velocity = Vector2.zero;
    }
}