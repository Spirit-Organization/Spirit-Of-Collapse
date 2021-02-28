using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsCSharp : MonoBehaviour {

    public GameObject Player;
    public int Health;
    public int MaxHealth;
    public int Speed;
    public int JumpPower;
    public bool DoubleJump;
    public bool WallJump;
    private bool TouchingWater;
    private bool TouchingWall;
    private bool TouchingGround;
    private float Movement;
    private Rigidbody2D rb;
    private int JumpPerWall;
    private Animator animator;
    private float Horizontal;
    private float NormalMovement;
    private bool Jump;
    private float TrueJump;
    [SerializeField]
    private int JumpAmount = 0;
    private bool jumping;

    void Start()
    {
        
        Player = this.gameObject;
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Horizontal = Input.GetAxis("Horizontal");
        Jump = Input.GetButtonDown("Jump");

    }


    void FixedUpdate()
    {
        Jump = Input.GetButtonDown("Jump");
        Horizontal = Input.GetAxis("Horizontal");

        RaycastHit2D Watered = Physics2D.Raycast(transform.position + new Vector3(0f, 1.5f, 0f), new Vector2(0f, -3.0f), 1 << LayerMask.NameToLayer("Water"));//checks for water
        if (Watered.collider != null)
        {
            if (Watered.collider.gameObject.CompareTag("Water"))
            {
                TouchingWater = true;
            }
            else
            {
                TouchingWater = false;
            }

        }

        if (TouchingWater)
        {
            Movement = Horizontal * (Speed / 2) * 10 * Time.deltaTime;
        } // sets movement for water
        else
        {
            Movement = Horizontal * Speed * 10 * Time.deltaTime;

        }//sets movement


        rb.velocity = new Vector2(Movement, rb.velocity.y);//velocity moves object

        RaycastHit2D Grounded = Physics2D.CircleCast(transform.position + new Vector3(0, 0, 0), 0.5f, new Vector2(0, -1), 1, 1 << LayerMask.NameToLayer("Water") | 1 << LayerMask.NameToLayer("Platform"));
        if (Grounded.collider != null)// checks for ground
        {
            JumpPerWall = 0;
            TouchingGround = true;
            if (rb.velocity.y/ Mathf.Abs(rb.velocity.y) == -1) 
            {
                animator.SetBool("Jump", true);//sets jump animation
            }
        }
        else
        {
            TouchingGround = false;
            animator.SetBool("Jump", false);//reverts jump animation

        }

        if (Movement != 0)
        {
            float NormalScale = transform.localScale.x / Mathf.Abs(transform.localScale.x); //flips sprite
            NormalMovement = Movement / Mathf.Abs(Movement);
            if (NormalScale == NormalMovement)
            {
                transform.localScale = Vector3.Scale(new Vector3(-1f, 1f, 1f), transform.localScale);
            }
            else
            {
                transform.localScale = Vector3.Scale(new Vector3(1f, 1f, 1f), transform.localScale);
            }

            RaycastHit2D Walled = Physics2D.CircleCast(transform.position + new Vector3(0, 0, 0), 0.5f, new Vector2(0, -1), LayerMask.GetMask("Wall"));
            if (Walled.collider != null)//checks if touching wall
            {
                if (Walled.collider.gameObject.CompareTag("Wall"))
                {
                    TouchingWall = true;

                }
                else
                {
                    TouchingWall = false;
                    if (TouchingGround)
                    {

                        AnimationWalk();
                    }
                }
            }
        }
        else { AnimationWalk(); }

        void AnimationWalk()
        {


            if (Mathf.Abs(Horizontal) > 0.2d) //wont give non zero number
            {
                animator.SetFloat("Speed", 0.3f);
            }
            else
            { animator.SetFloat("Speed", 0.1f); }

            ShowHearts();
        }
    }
    void Update()
    {
        JumpStart();
    }

    void JumpStart()
    {
        if (DoubleJump == true && JumpAmount == 1 && Jump || TouchingGround && JumpAmount == 0 && Jump)
        {

            jumping = true;
            if (TouchingWater)
            {
                TrueJump = JumpPower / 2;
            }
            else
            {
                TrueJump = JumpPower;
            }
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            JumpAmount = JumpAmount + 1;
            if (DoubleJump == true)
            {

                if (JumpAmount == 2)
                {
                    JumpAmount = 0;
                }
            }
            else
            {
                JumpAmount = 0;
            }

            rb.AddForce(new Vector2(0f, TrueJump), ForceMode2D.Impulse);


        }    
    }


    void ShowHearts()
    {

    }
}
