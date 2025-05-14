using UnityEngine;

public class HollowNestController : MonoBehaviour, IEffectable
{
    [Header("Nest Settings")]
    public int nestID;
    public GameObject nestVisual;
    public Transform effectCenter;

    public AudioClip nestAppearSound; 

    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); 
        }
        
        if(nestVisual != null)
        {
            nestVisual.SetActive(false);
        }
    }
    
    public void ActivateNest()
    {
        if(nestVisual != null)
        {
            nestVisual.SetActive(true);
            if (nestAppearSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(nestAppearSound);
            }
        }
    }
    
    public void ResetNest()
    {
        if(nestVisual != null)
        {
            nestVisual.SetActive(false);
        }
    }
    
    public bool ApplyEffect(EffectBase effect)
    {
        if (effect is HollowNest)
        {
            ActivateNest();
            return true;
        }
        return false;
    }

    
    public bool CanReceiveEffect(Vector3 playerPosition, float effectRadius, EffectBase effect)
    {
        Vector3 center = effectCenter != null ? effectCenter.position : transform.position;
        float distance = Vector3.Distance(center, playerPosition);
        Debug.Log($"{gameObject.name}: distance from effect center = {distance:F2}, radius = {effectRadius}");
        return distance <= effectRadius;
    }
}
