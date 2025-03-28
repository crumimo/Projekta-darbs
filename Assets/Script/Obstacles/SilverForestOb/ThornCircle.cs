using System.Collections;
using UnityEngine;

public class ThornCircle : MonoBehaviour
{
    private Transform playerTransform;
    private float duration;
    private float rotationSpeed;
    private GameObject barrierObject;

    public void Initialize(Transform playerTransform, float duration, float rotationSpeed)
    {
        this.playerTransform = playerTransform;
        this.duration = duration;
        this.rotationSpeed = rotationSpeed;

        Debug.Log("Initializing ThornCircle at position: " + playerTransform.position);

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
        if (collision.CompareTag("Obstacle"))
        {
            barrierObject = collision.gameObject;
            StartCoroutine(DestroyObstacle(barrierObject));
        }
        else if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.SetActive(false);
        }
    }

    private IEnumerator DestroyObstacle(GameObject barrierObj)
    {
        Animator barrierAnim = barrierObj.GetComponent<Animator>();
        barrierAnim.SetTrigger("Break");
        yield return new WaitForSeconds(1f);
        Destroy(barrierObj);
    }
}