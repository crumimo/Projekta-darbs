using System.Collections;
using UnityEngine;

public class HideAndSeekEnemy : MonoBehaviour
{
    public float greenLightDuration = 5f; // Длительность зеленого света
    public float redLightDuration = 3f; // Длительность красного света
    public SpriteRenderer spriteRenderer; // Ссылка на SpriteRenderer врага
    public Color greenLightColor = Color.green; // Цвет зеленого света
    public Color redLightColor = Color.red; // Цвет красного света
    public LayerMask obstacleLayer; // Слой укрытий и препятствий
    public LayerMask ignoreLayer; // Слой для игнорирования (включая слой врага)
    private bool isGreenLight = true; // Текущее состояние света
    private Transform player;

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }

        StartCoroutine(SwitchLight());
    }

    private IEnumerator SwitchLight()
    {
        while (true)
        {
            if (isGreenLight)
            {
                // Зеленый свет
                spriteRenderer.color = greenLightColor;
                Debug.Log("Green light - Players can move");
                yield return new WaitForSeconds(greenLightDuration);
            }
            else
            {
                // Красный свет
                spriteRenderer.color = redLightColor;
                Debug.Log("Red light - Players must stop");
                yield return new WaitForSeconds(redLightDuration);
            }

            isGreenLight = !isGreenLight;
        }
    }

    private void Update()
    {
        if (!isGreenLight)
        {
            CheckPlayerVisibility();
        }
    }

    private void CheckPlayerVisibility()
    {
        if (player == null)
        {
            Debug.LogError("Player not assigned.");
            return;
        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        Debug.DrawRay(transform.position, directionToPlayer * distanceToPlayer, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, ~ignoreLayer);

        if (hit.collider != null)
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player caught while red light! Restarting scene...");
                RestartScene();
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }
    }

    private void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}