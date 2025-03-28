using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SleepEffect", menuName = "Effects/Sleep")]
public class SleepEffect : EffectBase
{
    public float duration;

    public override void Apply(GameObject target)
    {
        Debug.Log("Applying Sleep Effect");
        PatrolEnemy enemy = target.GetComponent<PatrolEnemy>();
        if (enemy != null)
        {
            enemy.StartCoroutine(SleepCoroutine(enemy));
        }
    }

    private IEnumerator SleepCoroutine(PatrolEnemy enemy)
    {
        Debug.Log("Enemy is now asleep");
        enemy.isAsleep = true;
        enemy.visionMeshFilter.gameObject.SetActive(false);

        yield return new WaitForSeconds(duration);

        enemy.isAsleep = false;
        enemy.visionMeshFilter.gameObject.SetActive(true);
        Debug.Log("Enemy woke up");
    }
}