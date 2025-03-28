using UnityEngine;

public class JellyfishMovement : MonoBehaviour
{
    public float floatSpeed = 0.5f; 
    public float floatAmplitude = 0.5f; 
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}