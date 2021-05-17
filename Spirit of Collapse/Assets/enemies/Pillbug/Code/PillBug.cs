using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PillBug : MonoBehaviour
{
    public GameObject Player;
    public Sprite RollSprite;
    public GameObject DamageBoxPrefab;
    public GameObject Shockwaves;
    public GameObject screen;
    private Rigidbody2D rb;
    public int Speed;
    [SerializeField]
    private bool Following;
    [SerializeField]
    private bool Attack;
    private float AttackCool = 3;
    public int JumpPower;
    private bool Roll;
    private bool Damaged;
    [SerializeField]
    private int Health = 5;
    private SpriteRenderer sr;
    private Animator animator;
    private CircleCollider2D circle;
    private CapsuleCollider2D capsule;
    private bool DJ = true;
    [SerializeField]
    private int DamageCap;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        circle = GetComponent<CircleCollider2D>();
        capsule = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D Ground = Physics2D.Raycast(transform.position, new Vector2(0, 1), -3.5f, LayerMask.GetMask("Platform"));
        if (Damaged == false)
        {
            DJ = true;

            if (Following == true)
            {
                if (Player.transform.position.x > transform.position.x & Player.transform.position.x != transform.position.x)
                {
                    rb.velocity = new Vector2(Speed, rb.velocity.y);
                    transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    rb.velocity = new Vector2(-Speed, rb.velocity.y);
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                animator.SetBool("PillWalk", false);

                if (Mathf.Abs(Player.transform.position.x - transform.position.x) < 10)
                {
                    AttackCool -= Time.deltaTime;
                    if (AttackCool <= 0)
                    {
                        Attack = true;
                        Following = false;
                        AttackCool = 1;
                    }
                }
            }


            if (Attack == true && Ground == true)
            {
                if (Random.value <= 0.5)
                {
                    StartCoroutine(Swipe());
                    Attack = false;
                }
                else
                {
                    StartCoroutine(Jump());
                    Attack = false;

                }
            }
            if (Mathf.Abs(Player.transform.position.x - transform.position.x) > 20 & animator.GetBool("PillJump") == false)
            {
                StartCoroutine(RollStart());

            }
            else if (Roll == true & Mathf.Abs(Player.transform.position.x - transform.position.x) < 2)
            {
                Roll = false;
                StartCoroutine(RollEnd());
            }

            if (Roll == true && Following == false && animator.GetBool("PillBall"))
            {
                if (Mathf.Abs(transform.localScale.x) / transform.localScale.x == 1)
                {
                    transform.Rotate(0, 0, 20f);
                    rb.velocity = new Vector2(Speed * -6, rb.velocity.y);
                }
                else
                {
                    transform.Rotate(0, 0, -20f);
                    rb.velocity = new Vector2(Speed * 6, rb.velocity.y);
                }                
            }

        }
        else
        {
            StartCoroutine(RollEnd());
            if (Player.transform.position.x < transform.position.x & Player.transform.position.x != transform.position.x)
            {
                transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
           if (DJ == true)
            {
                StartCoroutine(Jump());
                DJ = false;
            }
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        RaycastHit2D Ground = Physics2D.Raycast(transform.position, new Vector2(0, 1), -3.5f, LayerMask.GetMask("Platform"));
        if (Following == false && collision.gameObject.CompareTag("Platform") && animator.GetBool("PillJump") == true && Ground)
        {
            GameObject Shockwave = Instantiate(Shockwaves, new Vector3((Mathf.Abs(transform.localScale.x) / transform.localScale.x) * 2 + transform.position.x, transform.position.y - 1.4f, transform.position.z), Quaternion.identity);
            Shockwave.GetComponent<Rigidbody2D>().velocity = new Vector2(10 * (Mathf.Abs(transform.localScale.x) / transform.localScale.x), Shockwave.GetComponent<Rigidbody2D>().velocity.y);
            Destroy(Shockwave, 3);


            GameObject Shockwave2 = Instantiate(Shockwaves, new Vector3((Mathf.Abs(transform.localScale.x) / transform.localScale.x) * -2 + transform.position.x, transform.position.y - 1.4f, transform.position.z), Quaternion.identity);
            Shockwave2.GetComponent<Rigidbody2D>().velocity = new Vector2(-10 * (Mathf.Abs(transform.localScale.x) / transform.localScale.x), Shockwave2.GetComponent<Rigidbody2D>().velocity.y);
            Destroy(Shockwave2, 3);
            if (Damaged == true)
            { StartCoroutine(Jump()); }

            StartCoroutine(Shake());
        }
        if (Roll == true && collision.gameObject.CompareTag("Player"))
        {
            GameObject.FindWithTag("Player").GetComponent<ControlsCSharp>().DamageIncoming = 1;
        }
        if (collision.gameObject.CompareTag("Projectile") == true && Damaged == false && DamageCap == 1 && Damaged == false)
        {
            Damaged = true;
            StartCoroutine(Hurt());
        }
        else if (collision.gameObject.CompareTag("Projectile") == true && Damaged == false)
        {
            DamageCap += 1;
        }
    } 
    

    IEnumerator Shake()
    {
        GameObject.FindWithTag("MainCamera").GetComponent<CameraMovement>().CameraShakes = true;
        yield return new WaitForSeconds(1);
        GameObject.FindWithTag("MainCamera").GetComponent<CameraMovement>().CameraShakes = false;
    }


    IEnumerator Hurt()
    {

        if (DamageCap >= 0)
        {
            Health += -1;
            DamageCap = 0;
        }
        sr.color = new Color(1, 1, 1, 0.5f);
        if (Health <= 0)
        { 
            screen.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
            Destroy(this.gameObject);
            SceneManager.LoadScene("End");
        }

        yield return new WaitForSeconds(5);
        StartCoroutine(RollEnd());
        sr.color = new Color(1, 1, 1, 1);
    }


    IEnumerator Swipe()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject DamageBox = Instantiate(DamageBoxPrefab, new Vector3(-(transform.localScale.x / Mathf.Abs(transform.localScale.x)) * 4.6f + (transform.position.x), 0.4f + transform.position.y, transform.position.z), Quaternion.identity);
        DamageBox.transform.localScale = new Vector3((Mathf.Abs(transform.localScale.x) / transform.localScale.x) * DamageBox.transform.localScale.x * 0.5f,DamageBox.transform.localScale.y * 0.5f,DamageBox.transform.localScale.z *0.5f);
        DamageBox.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2((Mathf.Abs(transform.localScale.x) / transform.localScale.x) * -10f, DamageBox.GetComponent<Rigidbody2D>().velocity.y);
        Destroy(DamageBox, 3f);
        animator.SetBool("PillSlash", true);

        yield return new WaitForSeconds(1f);
        Following = true;
        animator.SetBool("PillSlash", false);

    }


    IEnumerator Jump()
    {
        rb.AddForce(new Vector2(0, JumpPower), ForceMode2D.Impulse);
        animator.SetBool("PillJump", true);
        capsule.enabled = false;
        circle.enabled = true;
        yield return new WaitForSeconds(3.5f);
        Following = true;
        animator.SetBool("PillJump", false);
        capsule.enabled = true;
        circle.enabled = false;

    }


    IEnumerator RollStart()
    {

        Following = false;
        animator.SetBool("PillIntoBall", true);
        rb.velocity = new Vector2(0, rb.velocity.y);
        animator.SetBool("PillBall", true);
        yield return new WaitForSeconds(0.6f);

        Roll = true;
        capsule.enabled = false;
        circle .enabled = true;
    }


    IEnumerator RollEnd()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        capsule.enabled = true;
        circle.enabled = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
        Roll = false; 
        animator.SetBool("PillBall", false);
        if (Damaged == true)
        {
            Damaged = false;
        }

        yield return new WaitForSeconds(0.6f);
        animator.SetBool("PillIntoBall", false);
        Following = true;
    }
}
