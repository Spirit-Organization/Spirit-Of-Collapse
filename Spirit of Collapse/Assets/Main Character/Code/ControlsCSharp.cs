using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsCSharp : MonoBehaviour
{

    public GameObject Player;
    public GameObject WeaponPrefab;
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
    private int JumpAmount = 0;
    private bool Attack;
    private bool Attack2 = false;
    private bool attacking = false;
    public int DamageIncoming;
    private float DamagesDelay = 3;
    private Vector3 WeaponPosition;
    private float AttackCooldown = 0.5f;
    private SpriteRenderer sr;
    private RaycastHit2D Grounded;

    void Start()
    {

        Player = this.gameObject;
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        Horizontal = Input.GetAxis("Horizontal");
        Jump = Input.GetButtonDown("Jump");
        Attack = Input.GetButtonDown("Fire1");
    }

    void Update()
    {
        if (Health > 0)
        {
            Jump = Input.GetButtonDown("Jump");
            Horizontal = Input.GetAxis("Horizontal");
            Attack = Input.GetButtonDown("Fire1");

            RaycastHit2D Watered = Physics2D.Raycast(transform.position + new Vector3(0f, 1.5f, 0f), new Vector2(0f, -12.0f), 1 << LayerMask.NameToLayer("Water"));//checks for water
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

            if (Health > 0) { rb.velocity = new Vector2(Movement, rb.velocity.y); }//velocity moves object

            if (SceneManager.GetActiveScene().name == "")
            {
                Grounded = Physics2D.CircleCast(transform.position + new Vector3(0, -3, 0), 2.8f, new Vector2(0, -2.8f), 1, 1 << LayerMask.NameToLayer("Water") | 1 << LayerMask.NameToLayer("Platform"));
            }
            else
            {
                Grounded = Physics2D.CircleCast(transform.position + new Vector3(0, 0, 0), 0.7f, new Vector2(0, -0.7f), 1, 1 << LayerMask.NameToLayer("Water") | 1 << LayerMask.NameToLayer("Platform"));

            }
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
            if (Health > 0)
            {
                AttackCooldown -= Time.deltaTime;
                JumpStart();
                AttackStart();

            }
            DamagesDelay -= Time.deltaTime;

            if (DamagesDelay <= 0.0f && DamageIncoming > 0)
            {
                DamagesDelay = 3.0f;
                sr.color = new Color(1, 1, 1, 0.5f);
                Health -= DamageIncoming;
                DamageIncoming = 0;
                StartCoroutine(DamageRecovery());
            }
            else
            {
                DamageIncoming = 0;
            }
        }
        else
        {

            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
            rb.bodyType = RigidbodyType2D.Static;
            animator.SetBool("Jump", false);
            animator.SetBool("Fall", false);
            animator.SetFloat("Speed", 0.1f);
            animator.SetBool("Attack1", false);
            StartCoroutine(Respawn());
        }
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
        if (AttackCooldown <= 0)
        {
            if (Attack == true) //checks if player is pressing a button for attacking
            {
                Attack = false; //resets button for attacking manually just in case

                attacking = true;
                animator.SetBool("Attack1", true); //sets attack animation 1 to true           

                StartCoroutine(Attack1Done());
                AttackCooldown = 0.5f;
            }
        } 
    } 
    
    IEnumerator Attack1Done()
    {
        yield return new WaitForSeconds(0.2f);
        if (transform.localScale.x > 0)
        {
            WeaponPosition = transform.position + new Vector3(-1.0f, 0f, 0.0f);
        }
        else
        {
            WeaponPosition = transform.position + new Vector3(1.0f, 0f, 0.0f);
        }

        GameObject DamageBox = Instantiate(WeaponPrefab, WeaponPosition, Quaternion.identity); //clones damage box
        Destroy(DamageBox, 0.2f);


        yield return new WaitForSeconds(0.15f);

        animator.SetBool("Attack1", false);
        attacking = false;
    }
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(4f);
        
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        rb.bodyType = RigidbodyType2D.Dynamic;
        Health = MaxHealth;
        //stuff for respawning
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    IEnumerator DamageRecovery()
    {
        yield return new WaitForSeconds(2);
        sr.color = new Color(1, 1, 1, 1f);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("DeathBlock")) 
        {
            Health = 0;
        }
    }
}