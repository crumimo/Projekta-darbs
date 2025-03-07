using System.Collections;
using UnityEngine;

public class PatrolEnemyEffects : MonoBehaviour
{
    private PatrolEnemy patrolEnemy;
    private PatrolEnemyVision patrolEnemyVision;
    private bool isInvisibleEffectActive = false;

    private void Start()
    {
        patrolEnemy = GetComponent<PatrolEnemy>();
        patrolEnemyVision = GetComponent<PatrolEnemyVision>();
    }

    public void ApplyEffect(string combination)
    {
        switch (combination)
        {
            case "Mist Lull":
                StartCoroutine(InvisibilityEffect(5f));
                break;
            case "Lull Drift":
                StartCoroutine(SleepEffect(5f));
                break;
            // Add more cases for other combinations if needed
            default:
                Debug.LogWarning("Unknown combination: " + combination);
                break;
        }
    }

    private IEnumerator InvisibilityEffect(float duration)
    {
        if (isInvisibleEffectActive)
            yield break;

        isInvisibleEffectActive = true;
        patrolEnemyVision.enabled = false;
        yield return new WaitForSeconds(duration);
        patrolEnemyVision.enabled = true;
        isInvisibleEffectActive = false;
    }

    private IEnumerator SleepEffect(float duration)
    {
        patrolEnemy.enabled = false;
        patrolEnemyVision.enabled = false;
        patrolEnemyVision.DisableVisionMesh();
        yield return new WaitForSeconds(duration);
        patrolEnemy.enabled = true;
        patrolEnemyVision.enabled = true;
        patrolEnemyVision.EnableVisionMesh();
    }
}