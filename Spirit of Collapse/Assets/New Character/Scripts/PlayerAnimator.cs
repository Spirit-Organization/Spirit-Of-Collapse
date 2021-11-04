using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    private Dictionary<string, Rays> dictionary;
    private PlayerMovement playerMovement;
    public float playSpeed { get; private set; }
    private SpriteRenderer animator;
    private Rigidbody2D player;
    private PlayerInput input;
    private Animator animations;
    private bool jumpHelper;
    public bool canAnimJump;
    [System.NonSerialized]
    public bool attack;
    private bool forcefall = false;
    // 1 = idle, 2 = walk, 3 = jump, 4 = fall, 5 = second jump, 6 = death, 7 = wall slide, 8 = stagger, 9 = attack

    void Awake()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        animator = gameObject.transform.Find("Animator").gameObject.GetComponent<SpriteRenderer>();
        animations = gameObject.transform.Find("Animator").gameObject.GetComponent<Animator>();
        player = gameObject.GetComponent<Rigidbody2D>();
        input = gameObject.GetComponent<PlayerInput>();
        dictionary = playerMovement.rayDict;
        attack = false;
    }

    void LateUpdate()
    {
        Vector2 direction = input.actions["Direction"].ReadValue<Vector2>();
        float jump = input.actions["Jump"].ReadValue<float>();
        bool corner = false;
        if(playerMovement.right || playerMovement.left || playerMovement.down)
        {
            corner = true;
        }

        if(playerMovement.canDoubleJump && playerMovement.secondJump == false && canAnimJump)
        {
            StartCoroutine(DoubleBuff());
        }
        canAnimJump = playerMovement.secondJump;

        if (direction.x < 0)
        {
            animator.flipX = false;
        }
        else if(player.velocity.x > 0)
        {
            animator.flipX = true;
        }

        if (playerMovement.health < 0)
        {
            animations.SetInteger("Animation To Play", 6);
        }
        else if(attack)
        {
            animations.SetInteger("Animation To Play", 9);
            animations.SetFloat("Attack Speed", 2.4f / playerMovement.attackLengthSeconds);
            forcefall = true;
        }
        else if(jump > 0 && dictionary["Down"].touching | corner)
        {
            animations.SetInteger("Animation To Play", 3);
        }
        else if (direction.x != 0 && player.velocity.y == 0 | corner)
        {
            animations.SetInteger("Animation To Play", 2);
        }
        else if(jump > 0 && jumpHelper)
        {
            animations.SetInteger("Animation To Play", 5);
        }
        else if(player.velocity.y < 0 || forcefall)
        {
            animations.SetInteger("Animation To Play", 4);
            forcefall = false;
            
        }
        else if(direction.x == 0 && player.velocity.y == 0)
        {
            animations.SetInteger("Animation To Play", 1);
        }
        else if (playerMovement.health < 1)
        {
            animations.SetInteger("Animation To Play", 8);
        }
    }

    IEnumerator DoubleBuff()
    {
        jumpHelper = true;

        yield return new WaitForEndOfFrame();

        jumpHelper = false;
    }
}
