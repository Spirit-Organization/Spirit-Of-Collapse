using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[System.Serializable]
public class Damage : UnityEvent<int, GameObject> { }

public class BloodHunterAI : MonoBehaviour
{
    private bool Dead = false;
    int LocalX = 2;
    Vector3 direction = new Vector3(0f, 0f, 0f);
    private LayerMask PlayerMask;
    bool HitObject = false;
    private Transform HasPlayerTag;
    float AttackDelay = 7.0f;
    float ChangeDirectionDelay = 3.0f;
    float DamageDelay = 2f;
    public GameObject EnemyDamagePrefab;
    Vector3 DamagePosition = new Vector3(0f, 0f, 0f);
    private Animator animator;
    private GameObject DamagedOBJ;
    public GameObject DeathPrefab;
    private GameObject self;
    private GameObject DamageBox;

    private void Start()
    {
        self = this.gameObject;
    }


    void Update()
    {
        if (transform.localScale.x > 0) //gets direction it is facing
        {
            direction = new Vector2(1f, 0f);
        }
        else
        {
            direction = new Vector2(-1f, 0f);
        }
        animator = GetComponent<Animator>();
        if (Dead == false) //checking if dead
        {
            DamageDelay -= Time.deltaTime;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 3.0f, 1 << LayerMask.NameToLayer("Player")); //checks for player
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Player")) //if player found start attacking
                {
                    DamageDelay -= Time.deltaTime;
                    if (DamageDelay <= 0.0f)
                    {
                        DamageDelay = 3.5f;
                        StartCoroutine(Attack());

                        animator = GetComponent<Animator>();
                        animator.SetBool("Attack", true);
                    }
                }
            }
            else //player not found, changing facing direction
            {
                ChangeDirectionDelay -= Time.deltaTime;

                if (ChangeDirectionDelay <= 0.0f)
                {
                    ChangeDirectionDelay = 2.0f;
                    transform.localScale = Vector3.Scale(new Vector3(-1f, 1f, 1f), transform.localScale);
                }
            }
        }
        else
        {
            StartCoroutine(Death());
            if (animator.GetBool("Attack")) { animator.SetBool("Attack", false); }
        }
    }
    IEnumerator Attack() //starts attacking
    {

        yield return new WaitForSeconds(0.6f);

        if (transform.localScale.x > 0)
        {
            DamagePosition = transform.position + new Vector3(2.31f, -1.72f, 0.0f);
        }
        else
        {
            DamagePosition = transform.position + new Vector3(-2.31f, -1.72f, 0.0f);
        }

        if (Dead == false)
        {
            DamageBox = Instantiate(EnemyDamagePrefab, DamagePosition, Quaternion.identity); //clones damage box
        }
        Destroy(DamageBox, 0.4f);

        yield return new WaitForSeconds(0.5f);

        animator.SetBool("Attack", false);


    }

    void OnCollisionEnter2D(Collision2D col)
    {
        
        if (col.gameObject.CompareTag("DeathBlock") || col.gameObject.CompareTag("Projectile"))
        {
            Dead = true;
        }
        

    }  

    IEnumerator Death()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
        gameObject.layer = LayerMask.GetMask("Default");
        if (DamageBox != null) { Destroy(DamageBox,0f); }
        yield return new WaitForSeconds(3);
        Destroy(self,0);
        GameObject Particle = Instantiate(DeathPrefab, transform.position, Quaternion.identity); //clones damage box
        Destroy(Particle, 10f);

    }
}