using UnityEngine;

[CreateAssetMenu(fileName = "SleepEffect", menuName = "Effects/Sleep")]
public class SleepEffect : EffectBase
{
    public override void Apply(GameObject target)
    {
        Debug.Log("Applying Permanent Sleep Effect");
        PatrolEnemy enemy = target.GetComponent<PatrolEnemy>();
        if (enemy != null)
        {
            enemy.ApplyPermanentSleep(); 
        }
    }
}