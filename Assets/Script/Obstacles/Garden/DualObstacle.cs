using UnityEngine;

public class DualObstacle : MonoBehaviour, IEffectable
{
    [Header("Obstacle Parts")]
    public GameObject activeObstacle; 
    public GameObject inactiveObstacle; 

    private bool isSwitched = false; 

    [Header("Activation Settings")]
    public float activationRadius = 5f; 

    public int obstacleID;
    void Start()
    {
        UpdateObstacleState();
    }

    private void UpdateObstacleState()
    {
        if (activeObstacle != null) activeObstacle.SetActive(!isSwitched);
        if (inactiveObstacle != null) inactiveObstacle.SetActive(isSwitched);
    }

    public void ToggleObstacles()
    {
        isSwitched = !isSwitched;
        UpdateObstacleState();
        
        ObstacleStateManager.MarkObstacleSwitchedState(obstacleID, isSwitched);
        Debug.Log($"Obstacle state switched: {isSwitched}");
    }


    public void ApplyEffect(EffectBase effect)
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
                }
                else
                {
                    Debug.Log("Player is too far from obstacle. Effect not applied.");
                }
            }
        }
    }
    
    public void ApplySwitchedState(bool isSwitched)
    {
        this.isSwitched = isSwitched;
        UpdateObstacleState();
    }

    public bool CanReceiveEffect(Vector3 playerPosition, float effectRadius, EffectBase effect)
    {
        return Vector3.Distance(transform.position, playerPosition) <= effectRadius;
    }
}