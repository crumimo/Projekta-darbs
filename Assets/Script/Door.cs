using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool canEnter;

    [SerializeField] private Transform destination;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (canEnter && Input.GetKeyDown(KeyCode.F))
        {
            Movement playerPos = FindObjectOfType<Movement>();
            playerPos.transform.position = destination.position;
        }
        
        if (other.CompareTag("Player"))
        {
            canEnter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = false;
        } 
    }
}
