using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBugCs : MonoBehaviour
{
    public GameObject Player;
    public GameObject DeathPrefab;
    private Rigidbody2D rb;
    public int Speed;
    private Animator animator;
    [SerializeField]
    private bool Following = true;
    private CircleCollider2D circle;
    private CapsuleCollider2D capsule;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circle = GetComponent<CircleCollider2D>();
        capsule = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Following == true)
        {
            if (Player.transform.position.x > transform.position.x && Player.transform.position.x != transform.position.x)
            {
                rb.velocity = new Vector2(Speed, rb.velocity.y);
                transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                rb.velocity = new Vector2(-Speed, rb.velocity.y);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            animator.SetBool("PillWalk", true);
        }
        else if (Mathf.Abs(Player.transform.position.x - transform.position.x) > 2 && Mathf.Abs(Player.transform.position.x - transform.position.x) < 15)
        {
            if (Mathf.Abs(transform.localScale.x) / transform.localScale.x == 1)
            {
                transform.Rotate(0, 0, 20f);
                rb.velocity = new Vector2(Speed * -3, rb.velocity.y);
            }
            else
            {
                transform.Rotate(0, 0, -20f);
                rb.velocity = new Vector2(Speed * 3, rb.velocity.y);
            }
        }
        Debug.Log(Mathf.Abs(Player.transform.position.x - transform.position.x));
        if (Mathf.Abs(Player.transform.position.x - transform.position.x) > 7 && animator.GetBool("PillWalk") == true && Mathf.Abs(Player.transform.position.x - transform.position.x) < 14)
        {
            StartCoroutine(RollStart());
        }
        if (Mathf.Abs(Player.transform.position.x - transform.position.x) < 2)
        {
            StartCoroutine(RollEnd());
        }
    }
    IEnumerator RollStart()
    {
        animator.SetBool("PillWalk", false);
        animator.SetBool("PillIntoBall", true);
        rb.velocity = new Vector2(0, rb.velocity.y);
        animator.SetBool("PillBall", true);
        yield return new WaitForSeconds(0.6f);


        capsule.enabled = false;
        circle.enabled = true;
        Following = false;
    }


    IEnumerator RollEnd()
    {
        Following = true;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        capsule.enabled = true;
        circle.enabled = false;
        rb.velocity = new Vector2(0, rb.velocity.y);

        animator.SetBool("PillBall", false);
        yield return new WaitForSeconds(0.6f);
        animator.SetBool("PillIntoBall", false);
        animator.SetBool("PillWalk", true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile") == true)
        {
            Destroy(this.gameObject);
            GameObject Particle = Instantiate(DeathPrefab, transform.position, Quaternion.identity); 
            Destroy(Particle, 10f);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<ControlsCSharp>().DamageIncoming = 1;
        }
    }
}
