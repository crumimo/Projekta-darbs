using System.Collections;
using UnityEngine;

public class HideAndSeekEnemyEye : MonoBehaviour
{
    [Header("Timing Settings")]
    public float greenLightDuration = 5f;
    public float redLightDuration = 3f;
    
    [Header("Visual Settings")]
    public SpriteRenderer spriteRenderer;
    public Sprite openEyeSprite;
    public Sprite closedEyeSprite;
    public Material visionAreaMaterial;
    public Color visionActiveColor = new Color(1f, 0f, 0f, 0.3f);

    [Header("Collision Settings")]
    public LayerMask obstacleLayer;

    [Header("Mode Settings")]
    public bool alwaysWatch = false;

    private bool isGreenLight = true;
    private Transform player;
    private PolygonCollider2D polyCollider;
    private Mesh visionMesh;
    private MeshFilter visionMeshFilter;
    private MeshRenderer visionMeshRenderer;
    private Coroutine switchLightCoroutine;
    private bool effectApplied = false;
    
    private bool forceVisionOff = false;
    
    private HideAndSeekEnemyBody parentEnemyBody;

    void Awake()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
        if (polyCollider != null)
        {
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
        parentEnemyBody = GetComponentInParent<HideAndSeekEnemyBody>();
    }

    void Start()
    {
        visionMeshRenderer.sortingLayerName = "Foreground";
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (alwaysWatch)
        {
            isGreenLight = false;
            spriteRenderer.sprite = openEyeSprite;
            if (visionMeshRenderer != null)
            {
                visionMeshRenderer.enabled = true;
                visionAreaMaterial.color = visionActiveColor;
            }
        }
        else if (player != null && Time.timeScale != 0 && !effectApplied)
        {
            switchLightCoroutine = StartCoroutine(SwitchLight());
        }
    }

    IEnumerator SwitchLight()
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

    void Update()
    {
        if (forceVisionOff)
        {
            if (visionMeshRenderer != null)
                visionMeshRenderer.enabled = false;
            return;
        }
        
        if (parentEnemyBody != null && !parentEnemyBody.gameObject.activeInHierarchy)
        {
            if (visionMeshRenderer != null)
                visionMeshRenderer.enabled = false;
            return;
        }

        if (alwaysWatch)
        {
            if (visionMeshRenderer != null)
            {
                visionMeshRenderer.enabled = true;
                visionAreaMaterial.color = visionActiveColor;
            }
        }
        else
        {
            if (visionMeshRenderer != null)
            {
                if (!isGreenLight && !effectApplied)
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
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isGreenLight && other.CompareTag("Player") && !IsPlayerCovered(other.transform))
        {
            Movement movement = other.GetComponent<Movement>();
            if (movement != null)
                movement.Die();
            
        }
    }

    bool IsPlayerCovered(Transform t)
    {
        if (t == null)
            return false;
        Vector2 pos = t.position;
        Collider2D cover = Physics2D.OverlapPoint(pos, obstacleLayer);
        return cover != null;
    }

    void UpdateVisionMesh()
    {
        if (polyCollider == null || polyCollider.points.Length < 3)
            return;
        Vector2[] points = polyCollider.points;
        int count = points.Length;
        Vector3[] vertices = new Vector3[count];
        Vector3 scale = transform.lossyScale;
        for (int i = 0; i < count; i++)
            vertices[i] = new Vector3(points[i].x * scale.x, points[i].y * scale.y, 0f);
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

    void OnDisable()
    {
        if (Time.timeScale == 0)
            return;
        if (switchLightCoroutine != null)
        {
            StopCoroutine(switchLightCoroutine);
            switchLightCoroutine = null;
        }
    }

    void OnEnable()
    {
        if (parentEnemyBody != null && !parentEnemyBody.gameObject.activeInHierarchy)
        {
            if (visionMeshRenderer != null)
                visionMeshRenderer.enabled = false;
            return;
        }
        if (!alwaysWatch && !effectApplied && switchLightCoroutine == null && player != null && Time.timeScale != 0)
            switchLightCoroutine = StartCoroutine(SwitchLight());
    }

    public void ResetEnemyState()
    {
        forceVisionOff = false;
        
        if (parentEnemyBody != null && !parentEnemyBody.gameObject.activeInHierarchy)
        {
            return;
        }
    
        if (switchLightCoroutine != null)
        {
            StopCoroutine(switchLightCoroutine);
            switchLightCoroutine = null;
        }
        effectApplied = false;
        isGreenLight = alwaysWatch ? false : true;
        spriteRenderer.sprite = alwaysWatch ? openEyeSprite : closedEyeSprite;
        if (visionMeshRenderer != null)
            visionMeshRenderer.enabled = alwaysWatch;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;
        if (!alwaysWatch && player != null && Time.timeScale != 0)
            switchLightCoroutine = StartCoroutine(SwitchLight());
        enabled = true;
    }

    

    public void StopEye()
    {
        effectApplied = true;
        forceVisionOff = true;
        if (switchLightCoroutine != null)
        {
            StopCoroutine(switchLightCoroutine);
            switchLightCoroutine = null;
        }
        spriteRenderer.sprite = closedEyeSprite;
        if (visionMeshRenderer != null)
            visionMeshRenderer.enabled = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
        enabled = false;
    }
}
