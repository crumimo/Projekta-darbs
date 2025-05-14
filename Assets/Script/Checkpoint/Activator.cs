using System.Collections;
using UnityEngine;

public class Activator : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private GameObject[] objectsToDeactivate;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (objectsToActivate != null)
            {
                foreach (GameObject objectt in objectsToActivate)
                {
                    objectt.SetActive(true);
                }
            }

            if (objectsToDeactivate != null)
            {
                foreach (GameObject objectt in objectsToDeactivate)
                { 
                    objectt.SetActive(false);
                }
            }
        }
    }
    
}