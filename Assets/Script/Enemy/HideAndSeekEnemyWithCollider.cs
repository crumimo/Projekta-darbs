using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HideAndSeekEnemyRefactored : MonoBehaviour
{
    [SerializeField] private float GoDuration;
    [SerializeField] private float StopDuration;
    [SerializeField] private SpriteRenderer sprite;

    private bool canMove;

    private void Start()
    {
        StartCoroutine(CheckPlayer());
    }

    private IEnumerator CheckPlayer()
    {
        while (true)
        {
            if (canMove)
            {
                sprite.color = Color.green;
                Debug.Log("Player can move");
                yield return new WaitForSeconds(GoDuration);
            }
            else
            {
                sprite.color = Color.red;
                Debug.Log("Hide!!!" );
                yield return new WaitForSeconds(StopDuration);
            }
            canMove = !canMove;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!canMove)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Found ya!");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
