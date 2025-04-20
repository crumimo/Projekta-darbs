using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "InvisibilityEffect", menuName = "Effects/Invisibility")]
public class InvisibilityEffect : EffectBase
{
    public float duration = 4f;

    public override void Apply(GameObject target)
    {
        Debug.Log("Applying Invisibility Effect");
        PatrolEnemy enemy = target.GetComponent<PatrolEnemy>();
        if (enemy != null)
        {
            enemy.StartCoroutine(InvisibilityCoroutine(enemy));
        }
    }

    private IEnumerator InvisibilityCoroutine(PatrolEnemy enemy)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        SpriteRenderer playerSpriteRenderer = player.GetComponent<SpriteRenderer>();

        if (playerSpriteRenderer != null)
        {
            Color originalColor = playerSpriteRenderer.color;
            playerSpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

            // Make enemy ignore the player
            enemy.ignorePlayer = true;
            
            yield return new WaitForSeconds(duration - 1f); 
            
            float blinkDuration = 1f; 
            float elapsedTime = 0f;

            while (elapsedTime < blinkDuration)
            {
                float alpha = Mathf.PingPong(elapsedTime * 3f, 0.5f) + 0.5f; 
                playerSpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                
                elapsedTime += Time.deltaTime;
                yield return null; 
            }
            
            playerSpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
            
            enemy.ignorePlayer = false;

            Debug.Log("Invisibility Effect ended and state reset.");
        }
        else
        {
            Debug.LogWarning("Player SpriteRenderer not found!");
        }
    }
}