using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SpikeCircleEffect", menuName = "Effects/SpikeCircle")]
public class SpikeCircleEffect : ScriptableObject
{
    public GameObject thornCirclePrefab;
    public float duration = 2f;
    public float rotationSpeed = 100f;

    public void Apply(GameObject target)
    {
        Transform playerTransform = target.transform;
        Debug.Log("Player position for SpikeCircleEffect: " + playerTransform.position);

        GameObject thornCircle = Instantiate(thornCirclePrefab, playerTransform.position, Quaternion.identity);
        ThornCircle thornCircleScript = thornCircle.GetComponent<ThornCircle>();

        if (thornCircleScript != null)
        {
            thornCircleScript.Initialize(playerTransform, duration, rotationSpeed);
        }
    }
}