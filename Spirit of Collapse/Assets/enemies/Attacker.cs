using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    [Header("For Patrolling")]
    [SerializeField] float moveSpeed;
    private float moveDirection = 1;
    private bool facingRight = true;
    [SerializeField] Transform WallCheckPoint;
    [SerializeField] Transform GroundCheckPoint;
    [SerializeField] float circleRadius;
    [SerializeField] LayerMask PlatformLayer;
    private bool checkingGround;
    private bool checkingWall;

    [Header("Other")]
    private Rigidbody2D carlRB;
    void Start()
    {
        carlRB = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        checkingGround = Physics2D.OverlapCircle(GroundCheckPoint.position, circleRadius, PlatformLayer);
        checkingGround = Physics2D.OverlapCircle(WallCheckPoint.position, circleRadius, PlatformLayer);
        Patrolling();
    }

    void Patrolling()
    {
        if (!checkingGround || checkingWall)
        {
            if (facingRight)
            {
                Flip();
            }
            else if (!facingRight)
            {
                Flip();
            }
        }
        carlRB.velocity = new Vector2(moveSpeed * moveDirection, carlRB.velocity.y);
    }

    void Flip()
    {
        moveDirection *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(GroundCheckPoint.position, circleRadius);
        Gizmos.DrawWireSphere(WallCheckPoint.position, circleRadius);
    }
}