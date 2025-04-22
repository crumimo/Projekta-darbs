using System.Collections.Generic;
using UnityEngine;

public static class InteractableStateManager
{
    private static HashSet<int> openedInteractables = new HashSet<int>();
    
    private static HashSet<int> checkpointOpenedInteractables = new HashSet<int>();

    public static void MarkInteractableOpened(int id)
    {
        openedInteractables.Add(id);
    }

    public static bool IsInteractableOpened(int id)
    {
        return openedInteractables.Contains(id);
    }

    public static void SaveCheckpoint()
    {
        checkpointOpenedInteractables = new HashSet<int>(openedInteractables);
    }

    public static void RestoreCheckpointState()
    {
        openedInteractables = new HashSet<int>(checkpointOpenedInteractables);
        
        foreach (InteractableObject interactable in GameObject.FindObjectsOfType<InteractableObject>())
        {
            if (openedInteractables.Contains(interactable.interactableID))
            {
                interactable.ForceOpen();
            }
        }
    }

    public static void ResetInteractableStates()
    {
        openedInteractables.Clear();
    }
}