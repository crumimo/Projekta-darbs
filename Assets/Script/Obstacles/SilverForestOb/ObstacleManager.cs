using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public bool requiresThornDrift;
    public bool requiresMistThorn;
    public GameObject thornCirclePrefab; 
    public Transform playerTransform; 
    

    public void ApplyEffect(string combination)
    {
        if (requiresThornDrift && (combination == "Thorn Drift" || combination == "Drift Thorn"))
        {
            GameObject thornCircle = Instantiate(thornCirclePrefab, playerTransform.position, Quaternion.identity);
            ThornCircle thornCircleScript = thornCircle.GetComponent<ThornCircle>();
            thornCircleScript.playerTransform = playerTransform;
        }
        else if (requiresMistThorn && (combination == "Mist Thorn" || combination == "Thorn Mist"))
        {
            Debug.Log("Mist Thorn combination applied to obstacle: " + gameObject.name);
            Destroy(gameObject); 
        }
        else
        {
            Debug.LogWarning("Unknown or unsupported combination for obstacle: " + combination);
        }
    }
}