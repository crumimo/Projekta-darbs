using UnityEngine;

public class ThornCircleManager : MonoBehaviour
{
    public static ThornCircleManager Instance;
    public GameObject thornCirclePrefab; // Префаб круга шипов
    public Transform playerTransform; // Ссылка на трансформ игрока

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ApplyThornCircleEffect()
    {
        GameObject thornCircle = Instantiate(thornCirclePrefab, playerTransform.position, Quaternion.identity);
        ThornCircle thornCircleScript = thornCircle.GetComponent<ThornCircle>();
        thornCircleScript.playerTransform = playerTransform; // Установка ссылки на трансформ игрока
    }
}