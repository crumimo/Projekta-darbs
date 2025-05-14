using UnityEngine;

public class KillZoneActivator : MonoBehaviour, IEffectable
{
    [Header("References")]
    public KillZone targetKillZone;
    public float activationRadius = 5f;
    public AudioClip disableSound;
    
    [Header("Sprite Settings")]
    public Sprite normalSprite;
    public Sprite activatedSprite;
    private SpriteRenderer sr;
    
    private AudioSource audioSource;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (sr != null && normalSprite != null)
        {
            sr.sprite = normalSprite;
        }
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); 
        }
    }

    public bool ApplyEffect(EffectBase effect)
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
                    
                    if (disableSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(disableSound);
                    }
                    
                    Debug.Log("KillZone deactivated via KillZoneActivator on " + gameObject.name);
                    return true;
                }
                else
                {
                    Debug.Log("Player is out of activation radius for KillZoneActivator.");
                }
            }
        }
        return false;
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