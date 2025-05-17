using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour
{
    public GameObject mapPanel; 
    public RectTransform playerIcon;
    public Transform playerTransform;
    private Vector3 lastPlayerPosition;
    private Vector3 iconStartPosition;

    public RectTransform mapRectTransform;
    public RectTransform playerIconRectTransform;
    public Vector3 mapWorldOrigin;
    public Vector3 mapWorldSize;

    public FadeController fadeController; 
    private bool mapVisible = false;
    private bool isMapTransitioning = false;
    private bool isMapActivated = false; 

    private Movement playerMovement; 

    private void Start()
    {
        mapPanel.SetActive(false); 
        lastPlayerPosition = playerTransform.position;
        iconStartPosition = playerIconRectTransform.anchoredPosition;
        playerMovement = playerTransform.GetComponent<Movement>();
    }

    private void Update()
    {
        if (isMapActivated && Input.GetKeyDown(KeyCode.M) && !isMapTransitioning)
        {
            ToggleMap();
        }

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
        playerMovement.DisableMovement();
    }

    private IEnumerator CloseMapWithFade()
    {
        isMapTransitioning = true;
        yield return StartCoroutine(fadeController.FadeIn());

        mapPanel.SetActive(false);

        yield return StartCoroutine(fadeController.FadeOut());
        isMapTransitioning = false;

        mapVisible = false;
        playerMovement.EnableMovement();
    }

    private void UpdatePlayerIconPosition()
    {
        Vector3 normalizedPlayerPosition = new Vector3(
            (playerTransform.position.x - mapWorldOrigin.x) / mapWorldSize.x,
            (playerTransform.position.y - mapWorldOrigin.y) / mapWorldSize.y,
            0
        );

        Vector2 mapPosition = new Vector2(
            normalizedPlayerPosition.x * mapRectTransform.rect.width,
            normalizedPlayerPosition.y * mapRectTransform.rect.height
        );

        playerIconRectTransform.anchoredPosition = mapPosition;
        lastPlayerPosition = playerTransform.position;
    }

    public void ActivateMap()
    {
        isMapActivated = true; 
    }
}
