using UnityEngine;
using UnityEngine.UI;

public class MapUIController : MonoBehaviour
{
    public Canvas mapCanvas; // Канва с картой
    public GameObject playerIcon; // Иконка игрока на карте

    private bool isMapVisible = false; // Состояние видимости карты

    void Start()
    {
        if (mapCanvas != null)
        {
            mapCanvas.enabled = false; // Изначально карта выключена
        }
        
        if (playerIcon != null)
        {
            playerIcon.SetActive(false); // Изначально иконка игрока выключена
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }
    }

    void ToggleMap()
    {
        isMapVisible = !isMapVisible;
        if (mapCanvas != null)
        {
            mapCanvas.enabled = isMapVisible;
        }

        if (playerIcon != null)
        {
            playerIcon.SetActive(isMapVisible);
        }
    }
}