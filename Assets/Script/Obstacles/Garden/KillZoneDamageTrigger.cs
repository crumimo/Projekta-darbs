using UnityEngine;

public class KillZoneDamageTrigger : MonoBehaviour
{
    private KillZone killZoneParent;

    private void Awake()
    {
        killZoneParent = GetComponentInParent<KillZone>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (killZoneParent != null)
        {
            killZoneParent.OnDamageTriggerEnter(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (killZoneParent != null)
        {
            killZoneParent.OnDamageTriggerExit(collision);
        }
    }
}