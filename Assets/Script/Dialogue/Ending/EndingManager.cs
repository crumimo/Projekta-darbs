using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    [SerializeField] private GameObject endingNPC, NPCToDisable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (EndingCounter.ingredientsCollected == 2)
            {
                endingNPC.SetActive(true);
                NPCToDisable.SetActive(false);
                this.gameObject.SetActive(false);
            }
        }
    }
}
