using System.Collections;
using UnityEngine;

public class ObstacleManager : MonoBehaviour, IEffectable
{
    public int obstacleID; 
    public float distanceToActivate = 10f;
    
    [Header("Destruction Options")]
    [SerializeField] private bool ErosionTouch = false; 
    [SerializeField] private bool SpikeCircle = false; 
    [SerializeField] private bool Pureflare = false; 
    [SerializeField] private AudioClip destructionSound; 
    
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
        
        if (ObstacleStateManager.IsObstacleDestroyed(obstacleID))
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void ApplyEffect(EffectBase effect)
    {
        effect.Apply(gameObject);
    }
    
    public void DisableObstacle()
    {
        ObstacleStateManager.MarkObstacleAsDestroyed(obstacleID);
        WordUIManager.Instance.TrackObstacle(this); 
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
            return true;
        if (effect is SpikeCircleEffect && SpikeCircle)
            return true;
        if (effect is Pureflare && Pureflare) 
            return true;
        return false;
    }
}
