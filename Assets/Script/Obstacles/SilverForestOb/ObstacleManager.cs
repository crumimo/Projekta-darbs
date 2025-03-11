using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public static ObstacleManager Instance;
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

    // Apply the combination effects to enemies or create thorn circle
    public void ApplyCombinationEffect(string combination)
    {
        if (combination == "Thorn Drift" || combination == "Drift Thorn")
        {
            GameObject thornCircle = Instantiate(thornCirclePrefab, playerTransform.position, Quaternion.identity);
            ThornCircle thornCircleScript = thornCircle.GetComponent<ThornCircle>();
            thornCircleScript.playerTransform = playerTransform; // Установка ссылки на трансформ игрока
        }
        else
        {
            // Find all enemies in the scene
            PatrolEnemyEffects[] enemies = FindObjectsOfType<PatrolEnemyEffects>();
            foreach (PatrolEnemyEffects enemy in enemies)
            {
                enemy.ApplyEffect(combination);
            }
        }
    }
}