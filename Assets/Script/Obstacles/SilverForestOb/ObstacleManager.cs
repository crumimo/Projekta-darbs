using System.Collections;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public int obstacleID; // Unique ID for each obstacle
    public float distanceToActivate = 10f;
    
    [SerializeField] private bool ErosionTouch = false; 
    [SerializeField] private bool SpikeCircle = false; 
    [SerializeField] private AudioClip destructionSound; // Sound effect for destruction
    
    private Animator barrierAnim;
    private AudioSource audioSource;

    private void Start()
    {
        barrierAnim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Check if the obstacle was previously destroyed
        if (ObstacleStateManager.IsObstacleDestroyed(obstacleID))
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void ApplyEffect(ScriptableObject effect)
    {
        var applyMethod = effect.GetType().GetMethod("Apply");
        if (applyMethod != null)
        {
            applyMethod.Invoke(effect, new object[] { gameObject });
            Debug.Log($"{effect.GetType().Name} applied to {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"Effect of type {effect.GetType().Name} does not have an Apply method or is not applicable to ObstacleManager.");
        }
    }

    public void DisableObstacle()
    {
        ObstacleStateManager.MarkObstacleAsDestroyed(obstacleID);
        WordUIManager.Instance.TrackObstacle(this); // Track obstacle
        barrierAnim.SetTrigger("Break");
        if (destructionSound != null)
        {
            audioSource.PlayOneShot(destructionSound);
        }
        StartCoroutine(DeactivateAfterAnimation());
    }

    private IEnumerator DeactivateAfterAnimation()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

    public void ResetObstacle()
    {
        gameObject.SetActive(true);
    }
    
    public bool CanBeDestroyedByEffect(ScriptableObject effect)
    {
        if (effect is ErosionTouchEffect && ErosionTouch)
        {
            return true;
        }
        if (effect is SpikeCircleEffect && SpikeCircle)
        {
            return true;
        }
        return false;
    }
}