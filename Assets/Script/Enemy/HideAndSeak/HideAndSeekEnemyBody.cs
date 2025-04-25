using UnityEngine;

public class HideAndSeekEnemyBody : MonoBehaviour, IEffectable
{
    public int enemyID;
    private bool effectApplied = false;
    public HideAndSeekEnemyEye enemyEye;

    public void ApplyCombinationEffect(EffectBase effect)
    {
        if (effectApplied)
            return;
        if (effect is Pureflare)
        {
            gameObject.SetActive(false);
            if (enemyEye != null)
                enemyEye.StopEye();
            effectApplied = true;
            EnemyStateManager.MarkEnemyAsSleep(enemyID);
        }
    }

    public void ApplyEffect(EffectBase effect)
    {
        ApplyCombinationEffect(effect);
    }

    public void KillEnemy()
    {
        gameObject.SetActive(false);
        EnemyStateManager.MarkEnemyAsKilled(enemyID);
    }

    public void ApplyPermanentSleep()
    {
        gameObject.SetActive(false);
        EnemyStateManager.MarkEnemyAsSleep(enemyID);
    }

    public void ResetEnemy()
    {
        gameObject.SetActive(true);
        effectApplied = false;
        if (enemyEye != null)
        {
            enemyEye.ResetEnemyState();
            enemyEye.enabled = true;
        }
    }
}