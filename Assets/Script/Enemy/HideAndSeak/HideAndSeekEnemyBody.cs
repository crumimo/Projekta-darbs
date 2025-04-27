using System.Collections.Generic;
using UnityEngine;

public class HideAndSeekEnemyBody : MonoBehaviour, IEffectable
{
    public int enemyID;
    private bool effectApplied = false;
    
    [Header("Disable Effect Options")]
    
    public bool disableWithPureflare = true;
    
    public bool disableWithWhisperseed = false;
    public bool bodyDisabledByEffect = false;

    public List<HideAndSeekEnemyEye> enemyEyes;  

    public void ApplyCombinationEffect(EffectBase effect)
    {
        if (effectApplied)
            return;
    
        if ((effect is Pureflare && disableWithPureflare) ||
            (effect is Whisperseed && disableWithWhisperseed))
        {
            gameObject.SetActive(false);
            bodyDisabledByEffect = true; 
            if (enemyEyes != null)
            {
                foreach (var eye in enemyEyes)
                {
                    if (eye != null)
                        eye.StopEye();  
                }
            }
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
        bodyDisabledByEffect = false;
        effectApplied = false;
        if (enemyEyes != null)
        {
            foreach (var eye in enemyEyes)
            {
                if (eye != null)
                {
                    eye.ResetEnemyState();
                    eye.enabled = true;
                }
            }
        }
    }

    
    public bool CanReceiveEffect(Vector3 playerPosition, float effectRadius, EffectBase effect)
    {
        return Vector3.Distance(transform.position, playerPosition) <= effectRadius;
    }
}
