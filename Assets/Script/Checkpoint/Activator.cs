using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class Activator : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private GameObject[] objectsToDeactivate;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (objectsToActivate != null)
            {
                foreach (GameObject objectt in objectsToActivate)
                {
                    SpriteShapeRenderer shapeRenderer = objectt.GetComponent<SpriteShapeRenderer>();
                    if (shapeRenderer != null)
                    {
                        objectt.SetActive(true); // Включаем объект
                        StartCoroutine(SpriteFadeController.FadeIn(shapeRenderer, 0.5f)); // Плавно проявляем
                    }
                }
            }

            if (objectsToDeactivate != null)
            {
                foreach (GameObject objectt in objectsToDeactivate)
                {
                    SpriteShapeRenderer shapeRenderer = objectt.GetComponent<SpriteShapeRenderer>();
                    if (shapeRenderer != null)
                    {
                        StartCoroutine(FadeOutAndDisable(objectt, shapeRenderer, 0.5f)); // Плавно исчезает, затем отключается
                    }
                }
            }
        }
    }

    private IEnumerator FadeOutAndDisable(GameObject objectToDisable, SpriteShapeRenderer shapeRenderer, float duration)
    {
        yield return StartCoroutine(SpriteFadeController.FadeOut(shapeRenderer, duration)); // Ждём завершения анимации
        objectToDisable.SetActive(false); // Отключаем объект после исчезновения
    }
}