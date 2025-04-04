using UnityEngine;

[CreateAssetMenu(fileName = "SpikeCircleEffect", menuName = "Effects/SpikeCircle")]
public class SpikeCircleEffect : EffectBase
{
    public GameObject thornCirclePrefab;
    public float duration = 2f;
    public float rotationSpeed = 100f;

    public override void Apply(GameObject target)
    {
        if (target.CompareTag("Player"))
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
        Debug.Log($"{this.GetType().Name} applied to {target.name}");
    }
}