using System.Collections;
using UnityEngine;

public class ThornCircle : MonoBehaviour
{
    public float duration = 2f; // Продолжительность существования круга шипов
    public float rotationSpeed = 100f; // Скорость вращения круга шипов
    public Transform playerTransform; // Ссылка на трансформ игрока

    private void Start()
    {
        StartCoroutine(DestroyAfterDuration(duration));
    }

    private void Update()
    {
        // Вращение круга шипов
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        // Обновление позиции круга шипов в соответствии с позицией игрока
        if (playerTransform != null)
        {
            transform.position = playerTransform.position;
        }
    }

    private IEnumerator DestroyAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);

        if (collision.CompareTag("Obstacle"))
        {
            Debug.Log("Destroying obstacle: " + collision.gameObject.name);
            Destroy(collision.gameObject); // Разрушить препятствие
        }
        else if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Destroying enemy: " + collision.gameObject.name);
            Destroy(collision.gameObject); // Убить врага
        }
    }
}