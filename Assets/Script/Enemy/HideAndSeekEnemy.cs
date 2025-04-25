using System.Collections;
using UnityEngine;

public class HideAndSeekEnemy : MonoBehaviour
{
    [Header("Light Settings")]
    public float greenLightDuration = 5f;
    public float redLightDuration = 3f;
    public SpriteRenderer spriteRenderer;
    
    [Header("Enemy Sprites")]
    public Sprite openEyeSprite;
    public Sprite closedEyeSprite;

    [Header("Vision Settings")]
    public LayerMask obstacleLayer;

    [Header("Vision Area Display")]
    public Material visionAreaMaterial;
    public Color visionActiveColor = new Color(1f, 0f, 0f, 0.3f);

    private bool isGreenLight = true;
    private Transform player;
    private PolygonCollider2D polyCollider;
    
    private Mesh visionMesh;
    private MeshFilter visionMeshFilter;
    private MeshRenderer visionMeshRenderer;

    private void Awake()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
        if (polyCollider == null) return;

        GameObject visionAreaGO = new GameObject("VisionAreaDisplay");
        visionAreaGO.transform.SetParent(transform);
        visionAreaGO.transform.localPosition = polyCollider.offset;
        visionAreaGO.transform.localRotation = Quaternion.identity;

        visionMeshFilter = visionAreaGO.AddComponent<MeshFilter>();
        visionMeshRenderer = visionAreaGO.AddComponent<MeshRenderer>();
        visionMeshRenderer.material = visionAreaMaterial;

        visionMesh = new Mesh();
        visionMeshFilter.mesh = visionMesh;
        UpdateVisionMesh();
    }

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) return;

        StartCoroutine(SwitchLight());
    }

    private IEnumerator SwitchLight()
    {
        while (true)
        {
            if (isGreenLight)
            {
                spriteRenderer.sprite = closedEyeSprite;
                yield return new WaitForSeconds(greenLightDuration);
            }
            else
            {
                spriteRenderer.sprite = openEyeSprite;
                yield return new WaitForSeconds(redLightDuration);
            }
            isGreenLight = !isGreenLight;
        }
    }

    private void Update()
    {
        if (visionMeshRenderer != null)
        {
            if (!isGreenLight)
            {
                visionMeshRenderer.enabled = true;
                visionAreaMaterial.color = visionActiveColor;
            }
            else
            {
                visionMeshRenderer.enabled = false;
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isGreenLight && other.CompareTag("Player"))
        {
            if (!IsPlayerCovered(other.transform))
            {
                Movement movement = other.GetComponent<Movement>();
                if (movement != null)
                {
                    movement.Die();
                }
            }
        }
    }
    
    private bool IsPlayerCovered(Transform playerTransform)
    {
        if (playerTransform == null) return false;
        Vector2 checkPoint = playerTransform.position;
        Collider2D coverCollider = Physics2D.OverlapPoint(checkPoint, obstacleLayer);
        return coverCollider != null;
    }
    
    private void UpdateVisionMesh()
    {
        if (polyCollider == null || polyCollider.points.Length < 3) return;

        Vector2[] points = polyCollider.points;
        int count = points.Length;
        Vector3[] vertices = new Vector3[count];
        
        Vector3 scale = transform.lossyScale;
        for (int i = 0; i < count; i++)
        {
            vertices[i] = new Vector3(points[i].x * scale.x, points[i].y * scale.y, 0f);
        }
        
        int[] triangles = new int[(count - 2) * 3];
        for (int i = 0; i < count - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        visionMesh.Clear();
        visionMesh.vertices = vertices;
        visionMesh.triangles = triangles;
        visionMesh.RecalculateNormals();
    }
}
