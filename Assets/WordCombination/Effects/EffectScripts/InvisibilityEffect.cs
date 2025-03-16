using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "InvisibilityEffect", menuName = "Effects/Invisibility")]
public class InvisibilityEffect : ScriptableObject
{
    public float duration;

    public void Apply(GameObject target)
    {
        Debug.Log("Applying Invisibility Effect");
        PatrolEnemyTest enemy = target.GetComponent<PatrolEnemyTest>();
        if (enemy != null)
        {
            enemy.StartCoroutine(InvisibilityCoroutine(enemy));
        }
    }

    private IEnumerator InvisibilityCoroutine(PatrolEnemyTest enemy)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        SpriteRenderer playerSpriteRenderer = player.GetComponent<SpriteRenderer>();

        if (playerSpriteRenderer != null)
        {
            Color originalColor = playerSpriteRenderer.color;
            playerSpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

            // Make enemy ignore the player
            enemy.ignorePlayer = true;

            yield return new WaitForSeconds(duration);

            playerSpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

            // Restore enemy's ability to see the player
            enemy.ignorePlayer = false;
        }
    }
}