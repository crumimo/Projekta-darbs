using System.Collections;
using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints; // Points to patrol
    public float moveSpeed = 2f; // Movement speed
    public float waitTime = 2f; // Time to wait at each point

    [Header("Vision Settings")]
    public float visionAngle = 45f; // Vision angle when moving
    public float visionAngleIdle = 90f; // Vision angle when idle
    public float visionDistance = 5f; // Vision distance
    public float rotationSpeed = 2f; // Speed of turning when idle
    public float visionChangeSpeed = 2f; // Speed of vision angle change
    public LayerMask playerLayer;
    public MeshFilter visionMeshFilter;

    [Header("Effect Settings")]
    public float effectRadius = 5f;

    private Transform player;
    private int currentPointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private Quaternion targetRotation;
    private Mesh visionMesh;
    private float currentVisionAngle;
    private bool playerGotHit = false;
    private bool isAsleep = false;
    private bool isLookingAtPlayer = false;

    public bool ignorePlayer = false; // Variable to ignore player

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

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
        if (isAsleep)
        {
            return;
        }

        if (isLookingAtPlayer)
        {
            // Maintain the look at player for a while
            return;
        }

        if (isWaiting)
        {
            Idle();
        }
        else
        {
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

    public void ApplyEffect(string effectName)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > effectRadius)
        {
            Debug.Log("Player is too far away to apply the effect.");
            return;
        }

        Debug.Log("Applying effect: " + effectName);
        EffectManager.Instance.ApplyEffect(effectName, gameObject);
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

        // Set flag to maintain look at player
        isLookingAtPlayer = true;

        // Wait for 1 second
        yield return new WaitForSeconds(1.0f);

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
        }

        // Reset flag
        isLookingAtPlayer = false;
    }
}