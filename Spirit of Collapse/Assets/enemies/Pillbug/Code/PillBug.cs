using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillBug : MonoBehaviour
{
    public GameObject Player;
    public Sprite RollSprite;
    public GameObject DamageBoxPrefab;
    public GameObject Shockwaves;
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



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Damaged == false)
        {

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
                GetComponent<Animator>().SetBool("PillWalk", false);

                if (Mathf.Abs(Player.transform.position.x - transform.position.x) < 10)
                {
                    AttackCool -= Time.deltaTime;
                    if (AttackCool <= 0)
                    {
                        Attack = true;
                        Following = false;
                        AttackCool = 2;
                    }
                }
            }


            if (Attack == true)
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
            Debug.Log(Mathf.Abs(Player.transform.position.x - transform.position.x));
            if (Mathf.Abs(Player.transform.position.x - transform.position.x) > 20 & GetComponent<Animator>().GetBool("PillJump") == false)
            {
                StartCoroutine(RollStart());

            }
            else if (Roll == true & Mathf.Abs(Player.transform.position.x - transform.position.x) < 2)
            {
                Roll = false;
                StartCoroutine(RollEnd());
            }
            if (Roll == true)
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
        IEnumerator Swipe()
        {
            GameObject DamageBox = Instantiate(DamageBoxPrefab, new Vector3((Mathf.Abs(transform.localScale.x) / transform.localScale.x) * 1 + transform.position.x, 0, 0), Quaternion.identity);
            Destroy(DamageBox, 0.5f);
            GetComponent<Animator>().SetBool("PillSlash", true);

            yield return new WaitForSeconds(1f);
            Following = true;
            GetComponent<Animator>().SetBool("PillSlash", false);

        }
        IEnumerator Jump()
        {
            rb.AddForce(new Vector2(0, JumpPower), ForceMode2D.Impulse);
            GetComponent<Animator>().SetBool("PillJump", true);
            GetComponent<CapsuleCollider2D>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = true;
            yield return new WaitForSeconds(3.5f);
            Following = true;
            GetComponent<Animator>().SetBool("PillJump", false);
            GetComponent<CapsuleCollider2D>().enabled = true;
            GetComponent<CircleCollider2D>().enabled = false;

        }
        IEnumerator RollStart()
        {
            GetComponent<Animator>().SetBool("PillIntoBall", true);
            Following = false;
            GetComponent<Animator>().SetBool("PillBall", true);

            yield return new WaitForSeconds(0.9f);
            Roll = true;
            GetComponent<CapsuleCollider2D>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = true;
        }

        IEnumerator RollEnd()
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            GetComponent<CapsuleCollider2D>().enabled = true;
            GetComponent<CircleCollider2D>().enabled = false;
            GetComponent<Animator>().SetBool("PillBall", false);
            rb.velocity = new Vector2(0, rb.velocity.y);
            Roll = false;

            yield return new WaitForSeconds(0.6f);
            GetComponent<Animator>().SetBool("PillIntoBall", false);
            Following = true;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            RaycastHit2D Ground = Physics2D.Raycast(transform.position, new Vector2(0, 1), -3.5f, LayerMask.GetMask("Platform"));

            if (Following == false && collision.gameObject.CompareTag("Platform") && GetComponent<Animator>().GetBool("PillJump") == true && Ground)
            {
                GameObject Shockwave = Instantiate(Shockwaves, new Vector3((Mathf.Abs(transform.localScale.x) / transform.localScale.x) * 2 + transform.position.x, transform.position.y - 1.4f, transform.position.z), Quaternion.identity);
                Shockwave.GetComponent<Rigidbody2D>().velocity = new Vector2(10 * (Mathf.Abs(transform.localScale.x) / transform.localScale.x), Shockwave.GetComponent<Rigidbody2D>().velocity.y);
                Destroy(Shockwave, 3);


                GameObject Shockwave2 = Instantiate(Shockwaves, new Vector3((Mathf.Abs(transform.localScale.x) / transform.localScale.x) * -2 + transform.position.x, transform.position.y - 1.4f, transform.position.z), Quaternion.identity);
                Shockwave2.GetComponent<Rigidbody2D>().velocity = new Vector2(-10 * (Mathf.Abs(transform.localScale.x) / transform.localScale.x), Shockwave2.GetComponent<Rigidbody2D>().velocity.y);
                Destroy(Shockwave2, 3);


                StartCoroutine(Shake());
            }
            if (Roll == true && collision.gameObject.CompareTag("Player"))
            {
                GameObject.FindWithTag("Player").GetComponent<ControlsCSharp>().DamageIncoming = 1;
            }
            if (collision.gameObject.CompareTag("Projectile") == true)
            {
                Damaged = true;

                StartCoroutine(Hurt());
            }
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

        yield return new WaitForSeconds(1);

    }
}
