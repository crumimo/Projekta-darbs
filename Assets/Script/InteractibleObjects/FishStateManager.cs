using System.Collections.Generic;
using UnityEngine;

public static class FishStateManager
{
    private static HashSet<int> fishWithCollectedWords = new HashSet<int>();
    private static HashSet<int> checkpointFishState = new HashSet<int>();

    public static void MarkWordCollected(int fishID)
    {
        fishWithCollectedWords.Add(fishID);
    }

    public static bool IsWordCollected(int fishID)
    {
        return fishWithCollectedWords.Contains(fishID);
    }

    public static void SaveCheckpoint()
    {
        checkpointFishState = new HashSet<int>(fishWithCollectedWords);
    }

    public static void RestoreCheckpointState()
    {
        fishWithCollectedWords = new HashSet<int>(checkpointFishState);
    }

    


}