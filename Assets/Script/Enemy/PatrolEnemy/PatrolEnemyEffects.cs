using System.Collections;
using UnityEngine;

public class PatrolEnemyEffects : MonoBehaviour
{
    public float effectRadius = 5f; 
    public NotebookManager notebookManager; 
    private PatrolEnemy patrolEnemy;
    private PatrolEnemyVision patrolEnemyVision;
    private bool isInvisibleEffectActive = false;
    private bool isAsleep = false;

    private Transform player;

    private void Start()
    {
        patrolEnemy = GetComponent<PatrolEnemy>();
        patrolEnemyVision = GetComponent<PatrolEnemyVision>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }

        if (notebookManager == null)
        {
            Debug.LogError("NotebookManager not assigned.");
        }
    }

    public void ApplyEffect(string combination)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > effectRadius)
        {
            Debug.Log("Player is too far away to apply the effect.");
            return;
        }

        Debug.Log("Applying effect for combination: " + combination);

        switch (combination)
        {
            case "Lull Mist":
            case "Mist Lull":
                StartCoroutine(InvisibilityEffect(5f));
                notebookManager.AddEntry("Lull Mist", "Комбинация 'Lull Mist' использована на враге. Враг стал невидимым на 5 секунд.");
                break;
            case "Mist Drift":
            case "Drift Mist":
                StartCoroutine(SleepEffect(5f));
                notebookManager.AddEntry("Mist Drift", "Комбинация 'Mist Drift' использована на враге. Враг уснул на 5 секунд.");
                break;
            default:
                Debug.LogWarning("Unknown combination: " + combination);
                break;
        }
    }

    private IEnumerator InvisibilityEffect(float duration)
    {
        if (isInvisibleEffectActive)
            yield break;

        Debug.Log("Invisibility effect applied.");
        isInvisibleEffectActive = true;
        patrolEnemyVision.enabled = false;
        yield return new WaitForSeconds(duration);
        patrolEnemyVision.enabled = true;
        isInvisibleEffectActive = false;
        Debug.Log("Invisibility effect ended.");
    }

    private IEnumerator SleepEffect(float duration)
    {
        if (isAsleep)
            yield break;

        Debug.Log("Sleep effect applied.");
        isAsleep = true;
        patrolEnemy.enabled = false;
        patrolEnemyVision.enabled = false;
        patrolEnemyVision.DisableVisionMesh();
        yield return new WaitForSeconds(duration);
        patrolEnemy.enabled = true;
        patrolEnemyVision.enabled = true;
        patrolEnemyVision.EnableVisionMesh();
        isAsleep = false;
        Debug.Log("Sleep effect ended.");
    }
}