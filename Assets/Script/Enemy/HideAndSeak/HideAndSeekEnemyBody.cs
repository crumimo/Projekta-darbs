using UnityEngine;

public class HideAndSeekEnemyBody : MonoBehaviour, IEffectable {
    private bool effectApplied = false;
    public HideAndSeekEnemyEye enemyEye;
    
    public void ApplyCombinationEffect(EffectBase effect) {
        if (effectApplied) return;
        if (effect is Pureflare) {
            gameObject.SetActive(false);
            if (enemyEye != null) enemyEye.StopEye();
            effectApplied = true;
        }
    }
    
    public void ApplyEffect(EffectBase effect) {
        ApplyCombinationEffect(effect);
    }
}