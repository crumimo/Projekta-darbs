using UnityEngine;

[CreateAssetMenu(fileName = "FlourishingVeilEffect", menuName = "Effects/FlourishingVeil")]
public class FlourishingVeilEffect : EffectBase
{
    public GameObject shieldPrefab; // Prefab of the shield object
    public float duration = 5f; // Duration of the shield

    public override void Apply(GameObject target)
    {
        if (target.CompareTag("Player"))
        {
            Transform playerTransform = target.transform;
            Debug.Log("Player position for FlourishingVeilEffect: " + playerTransform.position);

            GameObject shield = Instantiate(shieldPrefab, playerTransform.position, Quaternion.identity);
            Shield shieldScript = shield.GetComponent<Shield>();

            if (shieldScript != null)
            {
                shieldScript.Initialize(playerTransform, duration);
            }
        }
        Debug.Log($"{this.GetType().Name} applied to {target.name}");
    }
}