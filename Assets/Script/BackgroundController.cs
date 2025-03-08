using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private Vector2 startPos;
    private float lenghtX, lenghtY;
    [SerializeField] private GameObject cam;
    [SerializeField] private float paralaxEffect;

    private void Start()
    {
        startPos = transform.position;
        lenghtX = GetComponent<SpriteRenderer>().bounds.size.x;
        lenghtY = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    private void FixedUpdate()
    {
        Vector2 distance = cam.transform.position * new Vector2(paralaxEffect,paralaxEffect); // 0 - doesn't move, 0.5 - moves half speed, 1 - moves with camera

        Vector2 movement = cam.transform.position * (1 - paralaxEffect);

        transform.position = new Vector3(startPos.x + distance.x, startPos.y + distance.y, transform.position.z);

       InfiniteScrolling(movement);
    }

    private void InfiniteScrolling(Vector2 movement)
    {
        if (movement.x > startPos.x + lenghtX)
        {
            startPos.x += lenghtX;
        }
        else if (movement.x < startPos.x - lenghtX)
        {
            startPos.x -= lenghtX;
        }
       
        if (movement.y > startPos.y + lenghtY)
        {
            startPos.y += lenghtY;
        }
        else if (movement.y < startPos.y - lenghtY)
        {
            startPos.y -= lenghtY;
        }
    }
}
