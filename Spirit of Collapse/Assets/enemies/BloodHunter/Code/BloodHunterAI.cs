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
    float DamageDelay = 0.5f;
    public GameObject EnemyDamagePrefab;
    Vector3 DamagePosition = new Vector3(0f, 0f, 0f);
    private Animator animator;
    private GameObject DamagedOBJ;

    int DamageAmount = 1;


    private void Start()
    {
      
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

        if (Dead == false) //checking if dead
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 3.0f, 1 << LayerMask.NameToLayer("Player")); //checks for player
            Debug.DrawRay(transform.position, direction);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Player")) //if player found start attacking
                {
                    DamageDelay -= Time.deltaTime;

                    if (DamageDelay <= 0.0f)
                    {
                        StartCoroutine(Attack());
                        DamageDelay = 0.5f;
                    }
                }
            }
            else //player not found, changing facing direction
            {
                ChangeDirectionDelay -= Time.deltaTime;

                if (ChangeDirectionDelay <= 0.0f)
                {
                    ChangeDirectionDelay = 3.0f;
                    transform.localScale = Vector3.Scale(new Vector3(-1f, 1f, 1f), transform.localScale);
                }
            }
        }
        else
        {
            StartCoroutine(Death());
        }
    }
    IEnumerator Attack() //starts attacking
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Attack", true);




        yield return new WaitForSeconds(0.7f);

        DamageOBJ();

    }

    void DamageOBJ() //executes attack and resets it
    {
        if (transform.localScale.x > 0)
        {
            DamagePosition = transform.position + new Vector3(2.0f, -1f, 0.0f);
        }
        else
        {
            DamagePosition = transform.position + new Vector3(-2.0f, -1f, 0.0f);
        }
        GameObject DamageBox = Instantiate(EnemyDamagePrefab, DamagePosition, Quaternion.identity); //clones damage box
        Destroy(DamageBox, 0.4f); //destroy damage box
        animator.SetBool("Attack", false);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<ControlsCSharp>().Health -= 1;
        }
        if (col.gameObject.CompareTag("DeathBlock"))
        {
            Dead = true;
        }
        

    }  

    IEnumerator Death()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
        yield return new WaitForSeconds(3);

        
    }
}