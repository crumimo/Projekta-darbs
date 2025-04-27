using UnityEngine;

public interface IEffectable
{
    void ApplyEffect(EffectBase effect);
    bool CanReceiveEffect(Vector3 playerPosition, float effectRadius, EffectBase effect);
    
}