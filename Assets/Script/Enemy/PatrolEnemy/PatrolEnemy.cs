using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public Transform[] patrolPoints; // Points to patrol
    public float moveSpeed = 2f; // Movement speed
    public float waitTime = 2f; // Time to wait at each point
    public float visionAngle = 45f; // Vision angle when moving
    public float visionAngleIdle = 90f; // Vision angle when idle
    public float visionDistance = 5f; // Vision distance
    public float rotationSpeed = 2f; // Speed of turning when idle
    public float visionChangeSpeed = 2f; // Speed of vision angle change
    private PatrolEnemyVision patrolEnemyVision; // Reference to PatrolEnemyVision script
    private PatrolEnemyEffects patrolEnemyEffects; // Reference to PatrolEnemyEffects script

    private int currentPointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private Quaternion targetRotation;
    private Mesh visionMesh;
    private float currentVisionAngle;

    private void Start()
    {
        // Set initial target rotation
        targetRotation = transform.rotation;
        currentVisionAngle = visionAngle;

        // Get references to PatrolEnemyVision and PatrolEnemyEffects scripts
        patrolEnemyVision = GetComponent<PatrolEnemyVision>();
        if (patrolEnemyVision != null)
        {
            patrolEnemyVision.visionDistance = visionDistance;
            patrolEnemyVision.visionAngle = visionAngle;
        }

        patrolEnemyEffects = GetComponent<PatrolEnemyEffects>();
    }

    private void Update()
    {
        if (isWaiting)
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

                // Update PatrolEnemyVision angle
                if (patrolEnemyVision != null)
                {
                    patrolEnemyVision.visionAngle = currentVisionAngle;
                }
            }
        }
        else
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

            // Update PatrolEnemyVision angle
            if (patrolEnemyVision != null)
            {
                patrolEnemyVision.visionAngle = currentVisionAngle;
            }
        }

        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Update vision mesh
        UpdateVisionMesh();
    }

    private void UpdateVisionMesh()
    {
        if (patrolEnemyVision == null || patrolEnemyVision.visionMeshFilter == null) return;

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

        patrolEnemyVision.visionMeshFilter.mesh.Clear();
        patrolEnemyVision.visionMeshFilter.mesh.vertices = vertices;
        patrolEnemyVision.visionMeshFilter.mesh.triangles = triangles;
        patrolEnemyVision.visionMeshFilter.mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        // Draw patrol points
        Gizmos.color = Color.red;
        foreach (Transform point in patrolPoints)
        {
            Gizmos.DrawSphere(point.position, 0.2f);
        }
    }
}