using System.Collections;
using UnityEngine;

public class ThornCircle : MonoBehaviour
{
    public float duration = 2f; 
    public float rotationSpeed = 100f; 
    public Transform playerTransform; 

    private void Start()
    {
        StartCoroutine(DestroyAfterDuration(duration));
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        
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
            Destroy(collision.gameObject); 
        }
        else if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Destroying enemy: " + collision.gameObject.name);
            collision.gameObject.SetActive(false);
        }
    }
}