using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour
{
    public GameObject mapPanel; // Map panel
    public RectTransform playerIcon; // RectTransform of the player icon on the map
    public Transform playerTransform; // Player's transform
    private Vector3 lastPlayerPosition; // Last player position
    private Vector3 iconStartPosition; // Initial position of the player icon on the map

    public RectTransform mapRectTransform; // RectTransform of the map panel
    public RectTransform playerIconRectTransform; // RectTransform of the player icon
    public Vector3 mapWorldOrigin; // World origin point (0,0,0) on the map
    public Vector3 mapWorldSize; // World size (width, height, depth) on the map

    public FadeController fadeController; // Fade in and fade out controller
    private bool mapVisible = false;
    private bool isMapTransitioning = false;
    private bool isMapActivated = false; // Flag to track if the map is activated

    private Movement playerMovement; // Player movement script

    private void Start()
    {
        mapPanel.SetActive(false); // Hide the map at the start
        lastPlayerPosition = playerTransform.position; // Store the initial player position
        iconStartPosition = playerIconRectTransform.anchoredPosition; // Store the initial position of the player icon
        playerMovement = playerTransform.GetComponent<Movement>(); // Get the player movement script
    }

    private void Update()
    {
        if (isMapActivated && Input.GetKeyDown(KeyCode.M) && !isMapTransitioning)
        {
            ToggleMap();
        }
        
        // Update the player icon position every frame
        UpdatePlayerIconPosition();
    }

    private void ToggleMap()
    {
        if (mapVisible)
        {
            StartCoroutine(CloseMapWithFade());
        }
        else
        {
            StartCoroutine(OpenMapWithFade());
        }
    }

    private IEnumerator OpenMapWithFade()
    {
        isMapTransitioning = true;
        yield return StartCoroutine(fadeController.FadeIn());

        mapPanel.SetActive(true);

        yield return StartCoroutine(fadeController.FadeOut());
        isMapTransitioning = false;

        mapVisible = true;
        playerMovement.DisableMovement(); // Disable player control
    }

    private IEnumerator CloseMapWithFade()
    {
        isMapTransitioning = true;
        yield return StartCoroutine(fadeController.FadeIn());

        mapPanel.SetActive(false);

        yield return StartCoroutine(fadeController.FadeOut());
        isMapTransitioning = false;

        mapVisible = false;
        playerMovement.EnableMovement(); // Enable player control
    }

    private void UpdatePlayerIconPosition()
    {
        // Normalize player position within the world bounds
        Vector3 normalizedPlayerPosition = new Vector3(
            (playerTransform.position.x - mapWorldOrigin.x) / mapWorldSize.x,
            (playerTransform.position.y - mapWorldOrigin.y) / mapWorldSize.y,
            0
        );

        // Convert normalized player position to map coordinates
        Vector2 mapPosition = new Vector2(
            normalizedPlayerPosition.x * mapRectTransform.rect.width,
            normalizedPlayerPosition.y * mapRectTransform.rect.height
        );

        // Move the player icon on the map accordingly
        playerIconRectTransform.anchoredPosition = mapPosition;

        // Update the last player position
        lastPlayerPosition = playerTransform.position;
    }

    public void ActivateMap()
    {
        isMapActivated = true; // Activate the map
    }
}