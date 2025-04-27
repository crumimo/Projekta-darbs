using UnityEngine;

public class HollowNestController : MonoBehaviour, IEffectable
{
    [Header("Nest Settings")]
    public int nestID;
    public GameObject nestVisual;
    public Transform effectCenter;

    void Start()
    {
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
        }
    }
    
    public void ResetNest()
    {
        if(nestVisual != null)
        {
            nestVisual.SetActive(false);
        }
    }
    
    public void ApplyEffect(EffectBase effect)
    {
        if(effect is HollowNest)
        {
            ActivateNest();
        }
    }
    
    public bool CanReceiveEffect(Vector3 playerPosition, float effectRadius, EffectBase effect)
    {
        Vector3 center = effectCenter != null ? effectCenter.position : transform.position;
        float distance = Vector3.Distance(center, playerPosition);
        Debug.Log($"{gameObject.name}: distance from effect center = {distance:F2}, radius = {effectRadius}");
        return distance <= effectRadius;
    }
}
