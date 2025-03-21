using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public float distanceToActivate = 10f;

    public void ApplyEffect(string effectName)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > distanceToActivate)
        {
            Debug.Log("Player is too far away to apply the effect.");
            return;
        }

        Debug.Log("Applying effect: " + effectName);

        // Проверка на эффект Erosion Touch
        if (effectName == "ErosionTouchEffect")
        {
            EffectManager.Instance.ApplyEffect(effectName, gameObject);
        }
        else
        {
            EffectManager.Instance.ApplyEffect(effectName, player);
        }
    }
}