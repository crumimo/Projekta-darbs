using UnityEngine;
using UnityEngine.SceneManagement;

public class PatrolEnemyVision : MonoBehaviour
{
    public float visionDistance = 5f;
    public float visionAngle = 45f;
    public LayerMask playerLayer;
    private Transform player;
    public MeshFilter visionMeshFilter; // Сделаем публичным

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (visionMeshFilter == null)
        {
            visionMeshFilter = GetComponentInChildren<MeshFilter>();
        }
    }

    private void Update()
    {
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        if (visionMeshFilter == null || visionMeshFilter.mesh.vertexCount == 0)
            return;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < visionDistance)
        {
            float angleToPlayer = Vector3.Angle(transform.right, directionToPlayer);
            if (angleToPlayer < visionAngle / 2)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, visionDistance, playerLayer);
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Player detected! Restarting scene...");
                    RestartScene();
                }
            }
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void DisableVisionMesh()
    {
        if (visionMeshFilter != null)
        {
            visionMeshFilter.mesh.Clear();
        }
    }

    public void EnableVisionMesh()
    {
        if (visionMeshFilter != null)
        {
            // Recreate the vision mesh here if needed
        }
    }
}