using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    private Dictionary<string, Rays> dictionary;
    private PlayerMovement playerMovement;
    public string animationToPlay { get; private set; }
    public float playSpeed;
    private SpriteRenderer animator;
    private Rigidbody2D player;
    private PlayerInput input;
    private Dictionary<string, int> animationDict = new Dictionary<string, int>()
    {
        {"Idle", 1},{"Walk", 2},{"Wall Walk", 3},{"Wall Slide", 4},{"Wall Jump", 5},{"Jump", 6},{"Fall", 7},{"Slope Idle", 8}, {"Slope Walk", 9}, {"Slope Walk Down", 9}
    };
    private Animator animations;

    void Awake()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        animator = gameObject.transform.Find("Animator").gameObject.GetComponent<SpriteRenderer>();
        animations = gameObject.transform.Find("Animator").gameObject.GetComponent<Animator>();
        player = gameObject.GetComponent<Rigidbody2D>();
        input = gameObject.GetComponent<PlayerInput>();
        StartCoroutine(AnimationUpdate());
    }

    IEnumerator AnimationUpdate()//Updates 20 times per Second
    {
        Vector2 direction = input.actions["Direction"].ReadValue<Vector2>();
        dictionary = playerMovement.rayDict;
        if (direction.x < 0)
        {
            animator.flipX = false;
        }
        else if(player.velocity.x > 0)
        {
            animator.flipX = true;
        }

        if(player.velocity.y > 0)
        {
            if(playerMovement.right || playerMovement.left)
            {
                animationToPlay = "Slope Walk";
            }
            else if(dictionary["Left"].touching || dictionary["Right"].touching)
            {
                animationToPlay = "Wall Jump";
            }
            else
            {
                animationToPlay = "Jump";
            }
        }
        else if(player.velocity.y < 0)
        {
            if(playerMovement.right || playerMovement.left)
            {
                animationToPlay = "Slope Walk Down";
            }
            else if(playerMovement.down)
            {
                animationToPlay = "Walk";
            }
            else if(dictionary["Left"].touching || dictionary["Right"].touching)
            {
                animationToPlay = "Wall Slide";
            }
            else
            {
                animationToPlay = "Fall";
            }
        }
        else if(direction.x == 0)
        {
            animationToPlay = "Idle";
        }
        else
        {
            if (dictionary["Left"].touching || dictionary["Right"].touching)
            {
                animationToPlay = "Wall Walk";
            }
            else
            {
                animationToPlay = "Walk";
            }
        }

        animations.SetInteger("Animation To Play", animationDict[animationToPlay]);
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(AnimationUpdate());
    }
}
