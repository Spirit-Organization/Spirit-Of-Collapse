using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{

    private Rigidbody2D rb;
    Vector2 startPos;

    private bool respawns = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision) // Checks for player when detected platform will fall
    {
        if (collision.gameObject.tag == "Player")
        {
            rb.isKinematic = false;
        }

        if(collision.transform.tag == "LillyPad")
        {
            transform.parent = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "LillyPad")
        {
           transform.parent = null;
        }
    }
}

