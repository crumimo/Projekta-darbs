using UnityEngine;

public interface IEffectable
{
    bool CanReceiveEffect(Vector3 playerPosition, float effectRadius, EffectBase effect);
    bool ApplyEffect(EffectBase effect);
}
