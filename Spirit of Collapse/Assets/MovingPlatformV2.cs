using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformV2 : MonoBehaviour
{

    float dirx, moveSpeed = 100f;
    bool moveRight = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > 200f)
            moveRight = false;
        if (transform.position.x < -20f)
            moveRight = true;
        if (moveRight)
            transform.position = new Vector2(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y);
        else
            transform.position = new Vector2(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y);
    }
}
