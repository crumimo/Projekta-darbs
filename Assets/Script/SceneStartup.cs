using UnityEngine;
using System.Collections;

public class SceneStartup : MonoBehaviour
{
    public FadeController fadeController;

    private IEnumerator Start()
    {
        
        fadeController.fadeImage.color = new Color(0, 0, 0, 1);
        fadeController.fadeImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        
        yield return StartCoroutine(fadeController.FadeOut());
    }
}