using UnityEngine;

public class SilverFCombinationHandler : MonoBehaviour
{
    public void ApplyCombinationEffect(string combination)
    {
        PatrolEnemyEffects[] enemies = FindObjectsOfType<PatrolEnemyEffects>();
        foreach (PatrolEnemyEffects enemy in enemies)
        {
            enemy.ApplyEffect(combination);
        }
        
        DialogueTrigger[] npcTriggers = FindObjectsOfType<DialogueTrigger>();
        foreach (DialogueTrigger npcTrigger in npcTriggers)
        {
            npcTrigger.ApplyEffect(combination);
        }
        
        ObstacleManager[] obstacles = FindObjectsOfType<ObstacleManager>();
        foreach (ObstacleManager obstacle in obstacles)
        {
            obstacle.ApplyEffect(combination);
        }
    }
}