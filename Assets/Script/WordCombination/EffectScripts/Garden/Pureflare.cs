using UnityEngine;

[CreateAssetMenu(fileName = "PureflareEffect", menuName = "Effects/Pureflare")]
public class Pureflare : EffectBase {
    public override void Apply(GameObject target) {
        HideAndSeekEnemyBody body = target.GetComponent<HideAndSeekEnemyBody>();
        if (body != null) body.ApplyCombinationEffect(this);
    }
}