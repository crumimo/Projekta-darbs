using UnityEngine;

public class KillZoneActivator : MonoBehaviour, IEffectable
{
    [Header("References")]
    public KillZone targetKillZone;
    public float activationRadius = 5f;

    [Header("Sprite Settings")]
    public Sprite normalSprite;
    public Sprite activatedSprite;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null && normalSprite != null)
        {
            sr.sprite = normalSprite;
        }
    }

    public void ApplyEffect(EffectBase effect)
    {
        if (effect is VerdantSurge && targetKillZone != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance <= activationRadius)
                {
                    if (sr != null && activatedSprite != null)
                    {
                        sr.sprite = activatedSprite;
                    }
                    targetKillZone.DisableZone();
                    Debug.Log("KillZone deactivated via KillZoneActivator on " + gameObject.name);
                }
                else
                {
                    Debug.Log("Player is out of activation radius for KillZoneActivator.");
                }
            }
        }
    }
    public void ResetActivator()
    {
        if (sr != null && normalSprite != null)
        {
            sr.sprite = normalSprite;
        }
    }

    public bool CanReceiveEffect(Vector3 playerPosition, float effectRadius, EffectBase effect)
    {
        float distance = Vector3.Distance(transform.position, playerPosition);
        return distance <= effectRadius;
    }
}