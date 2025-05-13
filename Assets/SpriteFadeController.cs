using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public static class SpriteFadeController
{
    public static IEnumerator FadeIn(SpriteShapeRenderer shapeRenderer, float duration)
    {
        if (shapeRenderer == null) yield break;

        float elapsedTime = 0f;
        Color color = shapeRenderer.material.color;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            color.a = alpha;
            shapeRenderer.material.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        shapeRenderer.material.color = color;
    }

    public static IEnumerator FadeOut(SpriteShapeRenderer shapeRenderer, float duration)
    {
        if (shapeRenderer == null) yield break;

        float elapsedTime = 0f;
        Color color = shapeRenderer.material.color;

        while (elapsedTime < duration)
        {
            float alpha = 1 - Mathf.Clamp01(elapsedTime / duration);
            color.a = alpha;
            shapeRenderer.material.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = 0f;
        shapeRenderer.material.color = color;
    }
}