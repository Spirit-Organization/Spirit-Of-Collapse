using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsCSharp : MonoBehaviour
{

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
    private bool Jump;
    private float TrueJump;
    [SerializeField]
    private int JumpAmount = 0;
    private bool Attack;
    private bool Attack2 = false;
    private bool attacking = false;

    void Start()
    {

        Player = this.gameObject;
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Horizontal = Input.GetAxis("Horizontal");
        Jump = Input.GetButtonDown("Jump");
        Attack = Input.GetButtonDown("Fire1");
    }


    void Update()
    {

        Jump = Input.GetButtonDown("Jump");
        Horizontal = Input.GetAxis("Horizontal");
        Attack = Input.GetButtonDown("Fire1");

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


        rb.velocity = new Vector2(Movement, rb.velocity.y);//velocity moves object00

        RaycastHit2D Grounded = Physics2D.CircleCast(transform.position + new Vector3(0, 0, 0), 0.7f, new Vector2(0, -0.7f), 1, 1 << LayerMask.NameToLayer("Water") | 1 << LayerMask.NameToLayer("Platform"));
        if (Grounded.collider != null)// checks for ground
        {

            JumpPerWall = 0;
            TouchingGround = true;
            animator.SetBool("Fall", false);//reverts fall animation
            animator.SetBool("Jump", false);
        }
        else
        {
            TouchingGround = false;

            if (rb.velocity.y / Mathf.Abs(rb.velocity.y) == -1 || rb.velocity.y / Mathf.Abs(rb.velocity.y) == 0) //checks for downward velocity
            {
                animator.SetBool("Fall", true);//sets fall animation
                animator.SetBool("Jump", false);//reverts jump animation
            }
            else if (rb.velocity.y / Mathf.Abs(rb.velocity.y) == 1) // checks for upward velocity
            {
                animator.SetBool("Fall", false);//reverts fall animation
                animator.SetBool("Jump", true);//sets jump animation
            }

        }

        if (Movement != 0)
        {
            float NormalScale = transform.localScale.x / Mathf.Abs(transform.localScale.x); //flips sprite
            float NormalMovement = Movement / Mathf.Abs(Movement);
            if (NormalScale == NormalMovement)
            {
                transform.localScale = Vector3.Scale(new Vector3(-1f, 1f, 1f), transform.localScale);
            }
            else
            {
                transform.localScale = Vector3.Scale(new Vector3(1f, 1f, 1f), transform.localScale);
            }

            RaycastHit2D Walled = Physics2D.CircleCast(transform.position + new Vector3(0, 0, 0), 0.7f, new Vector2(0, -0.7f), LayerMask.GetMask("Wall"));
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


            if (Mathf.Abs(Horizontal) > 0.2d) //sets walk animation
            {
                animator.SetFloat("Speed", 0.3f);
            }
            else
            { animator.SetFloat("Speed", 0.1f); }
        }
        JumpStart(); 
        AttackStart();
    }


    void JumpStart()
    {
        if (DoubleJump == true && JumpAmount == 1 && Jump == true || TouchingGround && JumpAmount == 0 && Jump == true)
        {

            Jump = false;
            if (TouchingWater) //checks if touching water
            {
                TrueJump = JumpPower / 2; //decreases jump hight if in water
            }
            else
            {
                TrueJump = JumpPower;
            }
            if (TouchingGround && JumpAmount == 1) //resets jump counter if touching ground
            {
                JumpAmount = 0;
            }

            JumpAmount += 1;

            rb.velocity = new Vector2(rb.velocity.x, 0f); //sets horizontal velocity to zero


            if (DoubleJump == true) //checks if you can double jump
            { 
                if (JumpAmount == 2) //checks if you have jumped twice, and if so, resets your counter
                {
                    JumpAmount = 0;
                }
            }
            else //if double jump is not enabled always reset counter
            {
                JumpAmount = 0;
            }

            rb.AddForce(new Vector2(0f, TrueJump), ForceMode2D.Impulse); //forces player up
            animator.SetBool("Jump", true); //sets jump animation
            animator.SetBool("Fall", false); //reverts fall animation
        }
    }


    void AttackStart()
    {
        if (Attack == true) //checks if player is pressing a button for attacking
        {
            Attack = false; //resets button for attacking manually just in case
            if (attacking == true) //checks if player is already attacking
            {
                Attack2 = true;

            }
            else
            {
                attacking = true;
                animator.SetBool("Attack1", true); //sets attack animation 1 to true
                animator.SetBool("Attack2", false); //sets attack animation 2 to false
                StartCoroutine(Attack1Done());
            }
        }
    }
    IEnumerator Attack1Done()
    {
        yield return new WaitForSeconds(0.65f);

        if (Attack2 == true) //checks if there is a 2nd attack
        {
            animator.SetBool("Attack2", true); //sets attack animation 2 to true
            Attack2 = false; //resets for next attack
            animator.SetBool("Attack1", false); // sets attack animation 1 to false
            attacking = false; //resets attacking to false
            StartCoroutine(Attack2Done()); //ends 2nd attack animation
        }
        else //stops attack 1 animation if no 2nd attack
        {
            animator.SetBool("Attack1", false);
            attacking = false;
        }
    }
    IEnumerator Attack2Done()
    {
        yield return new WaitForSeconds(0.7f);
        animator.SetBool("Attack2", false);// sets attack animation 2 to false
    }



}