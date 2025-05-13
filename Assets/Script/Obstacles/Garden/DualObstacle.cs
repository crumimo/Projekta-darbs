using UnityEngine;
using System.Collections;

public class DualObstacle : MonoBehaviour, IEffectable
{
    [Header("Obstacle Parts")]
    public GameObject activeObstacle;
    public GameObject inactiveObstacle;

    private bool isSwitched = false;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    [Header("Activation Settings")]
    public float activationRadius = 5f;

    public int obstacleID;

    void Start()
    {
        InitializeObstacles();
    }
    
    private void InitializeObstacles()
    {
        if (activeObstacle != null) activeObstacle.SetActive(true);
        if (inactiveObstacle != null) inactiveObstacle.SetActive(true);
        
        SetAlpha(activeObstacle, isSwitched ? 0f : 1f);
        SetAlpha(inactiveObstacle, isSwitched ? 1f : 0f);
        
        if (isSwitched)
        {
            if (activeObstacle != null) activeObstacle.SetActive(false);
        }
        else
        {
            if (inactiveObstacle != null) inactiveObstacle.SetActive(false);
        }
    }

    private void SetAlpha(GameObject obj, float alpha)
    {
        if (obj == null) return;
        var renderer = obj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color c = renderer.color;
            c.a = alpha;
            renderer.color = c;
        }
    }

    private void UpdateObstacleStateImmediate()
    {
        if (activeObstacle != null) activeObstacle.SetActive(!isSwitched);
        if (inactiveObstacle != null) inactiveObstacle.SetActive(isSwitched);
    }

    public void ToggleObstacles()
    {
        isSwitched = !isSwitched;

        ObstacleStateManager.MarkObstacleSwitchedState(obstacleID, isSwitched);
        Debug.Log($"Obstacle state switched: {isSwitched}");
    }

    public bool ApplyEffect(EffectBase effect)
    {
        if (effect is VerdantSurge)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector3.Distance(player.transform.position, transform.position);
                if (distance <= activationRadius)
                {
                    ToggleObstacles();
                    Debug.Log("VerdantSurgeEffect applied, obstacles toggled within radius.");
                    return true;
                }
                else
                {
                    Debug.Log("Player is too far from obstacle. Effect not applied.");
                }
            }
        }
        return false;
    }

    public void ApplySwitchedState(bool isSwitched)
    {
        this.isSwitched = isSwitched;
        UpdateObstacleStateImmediate();
    }

    public bool CanReceiveEffect(Vector3 playerPosition, float effectRadius, EffectBase effect)
    {
        return Vector3.Distance(transform.position, playerPosition) <= effectRadius;
    }
}
