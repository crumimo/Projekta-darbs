using System.Collections;
using UnityEngine;

public static class SpriteFadeController
{
    public static IEnumerator FadeIn(SpriteRenderer spriteRenderer, float duration)
    {
        if (spriteRenderer == null) yield break;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            SetAlpha(spriteRenderer, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetAlpha(spriteRenderer, 1f);
    }

    public static IEnumerator FadeOut(SpriteRenderer spriteRenderer, float duration)
    {
        if (spriteRenderer == null) yield break;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float alpha = 1 - Mathf.Clamp01(elapsedTime / duration);
            SetAlpha(spriteRenderer, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetAlpha(spriteRenderer, 0f);
    }

    private static void SetAlpha(SpriteRenderer spriteRenderer, float alpha)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
}