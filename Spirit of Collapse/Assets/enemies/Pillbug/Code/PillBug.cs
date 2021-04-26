using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillBug : MonoBehaviour
{
    public GameObject Player;
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
    [SerializeField]
    private bool Rolling;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
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

            if ((Mathf.Abs(Player.transform.position.x) - Mathf.Abs(transform.position.x)) < 10)
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


            if (Rolling == true)
            {
                Roll();
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
        yield return new WaitForSeconds(4f);
        Following = true;
        GetComponent<Animator>().SetBool("PillJump", false);

    }
    void Roll()
    {
        GetComponent<Animator>().SetBool("PillRoll", true);
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = true;
        rb.velocity = new Vector2(Speed * 5, rb.velocity.y);
        if (Mathf.Abs(transform.localScale.x / transform.localScale.x) == 1)
        {
            transform.Rotate(0, 0, 20f);
        }
        else
        {
            transform.Rotate(0, 0, -20f);
        }
        GetComponent<Animator>().SetBool("PillRoll", false);

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Following == false && collision.gameObject.CompareTag("Platform") && GetComponent<Animator>().GetBool("PillJump") == true)
        {
            GameObject Shockwave = Instantiate(Shockwaves,new Vector3( (Mathf.Abs(transform.localScale.x)/transform.localScale.x) * 2 + transform.position.x, transform.position.y - 2.7f,transform.position.z), Quaternion.identity);
            Shockwave.GetComponent<Rigidbody2D>().velocity = new Vector2( 10 * (Mathf.Abs(transform.localScale.x) / transform.localScale.x), Shockwave.GetComponent<Rigidbody2D>().velocity.y);
            Destroy(Shockwave, 3);


            GameObject Shockwave2 = Instantiate(Shockwaves, new Vector3((Mathf.Abs(transform.localScale.x) / transform.localScale.x) * -2 + transform.position.x, transform.position.y - 2.7f, transform.position.z), Quaternion.identity);
            Shockwave2.GetComponent<Rigidbody2D>().velocity = new Vector2(-10 * (Mathf.Abs(transform.localScale.x) / transform.localScale.x), Shockwave2.GetComponent<Rigidbody2D>().velocity.y);
            Destroy(Shockwave2, 3);
            

            StartCoroutine(Shake());
        }
    }
    IEnumerator Shake()
    {
        GameObject.FindWithTag("MainCamera").GetComponent<CameraMovement>().CameraShakes = true;
        yield return new WaitForSeconds(1);
        GameObject.FindWithTag("MainCamera").GetComponent<CameraMovement>().CameraShakes = false;
    }
}
