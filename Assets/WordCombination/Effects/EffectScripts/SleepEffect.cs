using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SleepEffect", menuName = "Effects/Sleep")]
public class SleepEffect : ScriptableObject
{
    public float duration;

    public void Apply(GameObject target)
    {
        Debug.Log("Applying Sleep Effect");
        PatrolEnemyTest enemy = target.GetComponent<PatrolEnemyTest>();
        if (enemy != null)
        {
            enemy.StartCoroutine(SleepCoroutine(enemy));
        }
    }

    private IEnumerator SleepCoroutine(PatrolEnemyTest enemy)
    {
        Debug.Log("Enemy is now asleep");
        enemy.enabled = false;
        enemy.visionMeshFilter.gameObject.SetActive(false);

        yield return new WaitForSeconds(duration);

        enemy.enabled = true;
        enemy.visionMeshFilter.gameObject.SetActive(true);
        Debug.Log("Enemy woke up");
    }
}