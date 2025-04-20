using System.Collections;
using UnityEngine;

public class PatrolEnemy : MonoBehaviour, IEffectable
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints; // Points to patrol
    public float moveSpeed = 2f; // Movement speed
    public float waitTime = 2f;

    [Header("Vision Settings")]
    public float visionAngle = 45f; // Vision angle when moving
    public float visionAngleIdle = 90f; // Vision angle when idle
    public float visionDistance = 5f; // Vision distance
    public float rotationSpeed = 2f; // Speed of turning when idle
    public float visionChangeSpeed = 2f; // Speed of vision angle change
    public LayerMask playerLayer;
    public MeshFilter visionMeshFilter;

    [Header("Effect Settings")]
    public bool permanentSleep = false;

    private Transform player;
    private int currentPointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private Quaternion targetRotation;
    private Mesh visionMesh;
    private float currentVisionAngle;
    private bool playerGotHit = false;
    public bool isAsleep = false; 
    private bool isLookingAtPlayer = false;

    public bool ignorePlayer = false; // Variable to ignore player

    private Animator animator;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();

        // Set initial target rotation
        targetRotation = transform.rotation;
        currentVisionAngle = visionAngle;

        // Initialize vision mesh
        if (visionMeshFilter == null)
        {
            visionMeshFilter = GetComponentInChildren<MeshFilter>();
        }
    }

    private void Update()
    {
        if (isAsleep || permanentSleep) 
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Sleep", true);
            return; 
        }
        
        if (isAsleep)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Sleep", true);
            return; 
        }

        if (isLookingAtPlayer)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Sleep", false);
            // Maintain the look at player for a while
            return;
        }

        if (isWaiting)
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Sleep", false);
            Idle();
        }
        else
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Sleep", false);
            Patrol();
        }

        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (!ignorePlayer)
        {
            CheckForPlayer();
        }

        // Update vision mesh
        UpdateVisionMesh();
    }

    private void Patrol()
    {
        // Move to the next point
        Transform targetPoint = patrolPoints[currentPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
        
        if (transform.position.x > targetPoint.position.x)
        {
            animator.SetTrigger("Left");
        }
        else
        {
            animator.SetTrigger("Right");
        }

        // Check if reached the point
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            isWaiting = true;
        }

        // Rotate towards the movement direction
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        targetRotation = Quaternion.Euler(0, 0, angle);
        currentVisionAngle = Mathf.Lerp(currentVisionAngle, visionAngle, Time.deltaTime * visionChangeSpeed);
    }

    private void Idle()
    {
        waitTimer += Time.deltaTime;
        if (waitTimer >= waitTime)
        {
            isWaiting = false;
            waitTimer = 0f;
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
        else
        {
            // Turn left and right when waiting
            float angle = Mathf.Sin(Time.time * rotationSpeed) * visionAngleIdle;
            targetRotation = Quaternion.Euler(0, 0, angle);
            currentVisionAngle = Mathf.Lerp(currentVisionAngle, visionAngleIdle, Time.deltaTime * visionChangeSpeed);
        }
    }

    private void UpdateVisionMesh()
    {
        if (visionMeshFilter == null) return;

        int segments = 20;
        float angleStep = currentVisionAngle / segments;

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -currentVisionAngle / 2 + angleStep * i;
            Vector3 direction = Quaternion.Euler(0, 0, currentAngle) * Vector3.right * visionDistance;
            vertices[i + 1] = direction;

            if (i < segments)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        visionMeshFilter.mesh.Clear();
        visionMeshFilter.mesh.vertices = vertices;
        visionMeshFilter.mesh.triangles = triangles;
        visionMeshFilter.mesh.RecalculateNormals();
    }

    private void CheckForPlayer()
    {
        if (visionMeshFilter == null || visionMeshFilter.mesh.vertexCount == 0 || player == null)
            return;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < visionDistance)
        {
            float angleToPlayer = Vector3.Angle(transform.right, directionToPlayer);
            if (angleToPlayer < currentVisionAngle / 2)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, visionDistance, playerLayer);
                if (hit.collider != null && hit.collider.CompareTag("Player") && !playerGotHit)
                {
                    playerGotHit = true;
                    Debug.Log("Enemy detected player!");
                    player.GetComponent<Movement>().Die();
                }
            }
        }
    }
    public void ApplyEffect(EffectBase effect)
    {
        effect.Apply(gameObject);
    }
    public void ApplyEffect(ScriptableObject effect)
    {
        var applyMethod = effect.GetType().GetMethod("Apply");
        if (applyMethod != null)
        {
            applyMethod.Invoke(effect, new object[] { gameObject });
            Debug.Log($"{effect.GetType().Name} applied to {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"Effect of type {effect.GetType().Name} does not have an Apply method or is not applicable to PatrolEnemy.");
        }
    }

    public void ResetEnemyState()
    {
        playerGotHit = false;
    }

    public void LookAtPlayerAndKill()
    {
        StartCoroutine(LookAtPlayerCoroutine());
    }

    private IEnumerator LookAtPlayerCoroutine()
    {
        // Rotate to face the player immediately
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Log the rotation action
        Debug.Log("Enemy rotated to face the player.");

        // Set flag to maintain look at player
        isLookingAtPlayer = true;

        // Wait for 1 second
        yield return new WaitForSeconds(1f);

        // Check if the player is in the vision cone
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < visionDistance)
        {
            float angleToPlayer = Vector3.Angle(transform.right, directionToPlayer);
            if (angleToPlayer < currentVisionAngle / 2)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, visionDistance, playerLayer);
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Player detected by Lull Drift combination.");
                    player.GetComponent<Movement>().Die();
                }
                else
                {
                    Debug.Log("Player is not in the vision cone after Lull Drift combination.");
                }
            }
            else
            {
                Debug.Log("Player is outside the vision angle.");
            }
        }
        else
        {
            Debug.Log("Player is outside the vision distance.");
        }

        // Reset flag
        isLookingAtPlayer = false;

        // Complete the coroutine
        yield break;
    }

    private void OnDrawGizmos()
    {
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.red; 
            foreach (Transform point in patrolPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.2f); 
                }
            }
        }
    }
    
    public void ApplyPermanentSleep()
    {
        permanentSleep = true;
        visionMeshFilter.gameObject.SetActive(false);
        Debug.Log($"{gameObject.name} is now in permanent sleep.");
    }
}