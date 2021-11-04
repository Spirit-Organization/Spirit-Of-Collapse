using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rays
{
    public Vector2 direction;
    public RaycastHit2D raycast;
    public bool touching = false;
    public float interactionPoint = 0f;
}

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D playerb;
    public float maxSpeed = 5.0f;
    public float jumpSpeed = 5.0f;
    public CapsuleCollider2D cap;
    public float attackSize = 4f;
    public bool wallJump = true;
    private float rightAngle;
    private float leftAngle;
    public bool jumping { get; private set; } = false;
    public bool right { get; private set; } = false;
    public bool left { get; private set; } = false;
    private bool attackBufferRunning = false;
    public float minSlopeAngle = 5;
    public float maxSlopeAngle = 50;
    private CapsuleCollider2D attackHitBox;
    public GameObject attackGameObject;
    public float attackCooldownSeconds;
    public float attackLengthSeconds;
    public float jumpPushOffSpeed;
    public bool slideDownWalls = true;
    public bool secondJump { get; private set; } = false;
    public bool canDoubleJump = true;
    public bool down { get; private set; } = false;
    public int health = 5;
    private PlayerAnimator playerAnimator;


    //Input Values
    private InputAction attack;
    private InputAction jumpInput;
    private PlayerInput inputs;
    public int walls { get; private set; } = 0;
    
    //Directional Values
    private Vector2 directionRoughValue;
    private Vector2 directionSmoothValue;
    private Vector2 directionVelocityCurrent; //just used to make SmoothDamp work
    public float directionSmoothTime = 0.2f; //time it takes to reach value 1.0

    //Raycast Values
    public Dictionary<string, Rays> rayDict = new Dictionary<string, Rays>();
    

    private void Awake()
    {
        SetValues();
    }

    private void OnEnable()
    {       
        inputs.actions.Enable();
    }

    private void OnDisable()
    {
        inputs.actions.Disable();
    }

    void Update()
    {
        directionRoughValue = inputs.actions["Direction"].ReadValue<Vector2>();
        directionSmoothValue = Vector2.SmoothDamp(directionSmoothValue, directionRoughValue, ref directionVelocityCurrent, directionSmoothTime);
        if (health > 0)
        {
            SetRays();
        }
    }

    private void FixedUpdate()
    {
        if (health > 0)
        {
            if (walls == 0)
            {
                playerb.velocity = new Vector2(directionSmoothValue.x * maxSpeed, playerb.velocity.y);
            }
            else if (walls == 1 && directionSmoothValue.x < 0 | slideDownWalls == false)
            {
                playerb.velocity = new Vector2(directionSmoothValue.x * maxSpeed, playerb.velocity.y);
            }
            else if (walls == -1 && directionSmoothValue.x > 0 | slideDownWalls == false)
            {
                playerb.velocity = new Vector2(directionSmoothValue.x * maxSpeed, playerb.velocity.y);
            }
        }
    }

    private void SetRays()
    {
        Vector2 capsuleOrigin = cap.offset + new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

        foreach (Rays rays in rayDict.Values)
        {
            rays.raycast = Physics2D.Linecast(capsuleOrigin, capsuleOrigin + rays.direction * 2);
            if(rays.raycast.collider != null)
            {
                rays.interactionPoint = rays.raycast.distance;
                if (rays.interactionPoint <= (new Vector2(0, 0) - rays.direction).magnitude + 0.01)
                {
                    rays.touching = true;
                }
                else
                {
                    rays.touching = false;
                }
                Debug.DrawLine(capsuleOrigin, capsuleOrigin + rays.direction * 2, Color.red);
            }
            else
            {
                rays.touching = false;
                rays.interactionPoint = 0f;
                Debug.DrawLine(capsuleOrigin, capsuleOrigin + rays.direction * 2, Color.green);
            }
        }
        GetAndUseInputs();
    }

    private void SetValues()
    {
        playerb = gameObject.GetComponent<Rigidbody2D>();
        inputs = gameObject.GetComponent<PlayerInput>();
        attackHitBox = attackGameObject.GetComponent<CapsuleCollider2D>();
        jumpInput = inputs.actions["Jump"];
        attack = inputs.actions["Attack"];
        Physics2D.queriesStartInColliders = false;
        playerAnimator = gameObject.GetComponent<PlayerAnimator>();

        rayDict.Add("Up", new Rays(){direction = new Vector2(0,0.5f) * cap.size});
        rayDict.Add("Down", new Rays(){direction = new Vector2(0, -0.5f) * cap.size});
        rayDict.Add("Left", new Rays() { direction = new Vector2(-0.5f, 0) * cap.size });
        rayDict.Add("Right", new Rays() { direction = new Vector2(0.5f, 0) * cap.size});
        rayDict.Add("Down Right", new Rays() { direction = new Vector2(1.1f, -1) * cap.size / 2 });
        rayDict.Add("Down Left", new Rays() { direction = new Vector2(-1.1f, -1) * cap.size / 2 });
    }

    private void GetAndUseInputs()
    {
        if(jumpInput.WasPressedThisFrame() && jumping == false) // I'm using this to make sure everything works properly. (you will have to wait for slope code to be effective)
        {
            jumping = true;
            StartCoroutine(JumpBuffer());
        }

        //Get Inputs
        if(jumpInput.WasPressedThisFrame() && rayDict["Down"].touching | right | left)//Jump Inputs
        {
            secondJump = true;
            playerb.velocity = new Vector2(playerb.velocity.x, jumpSpeed);
        }
        else if(jumpInput.WasPressedThisFrame() && rayDict["Right"].touching | rayDict["Down Right"].touching && wallJump)
        {
            secondJump = true;
            playerb.velocity = new Vector2(playerb.velocity.x - jumpPushOffSpeed, jumpSpeed);
        }
        else if(jumpInput.WasPressedThisFrame() && rayDict["Left"].touching | rayDict["Down Left"].touching && wallJump)
        {
            secondJump = true;
            playerb.velocity = new Vector2(playerb.velocity.x + jumpPushOffSpeed, jumpSpeed);
        }
        else if(jumpInput.WasPressedThisFrame() && secondJump && canDoubleJump)
        {
            playerb.velocity = new Vector2(playerb.velocity.x, jumpSpeed);
            secondJump = false;
        }
        if(rayDict["Down"].touching && jumpInput.ReadValue<float>() == 0)
        {
            secondJump = false;
        }

        if (attackBufferRunning == false)//attack inputs
        {
            if (attack.WasPressedThisFrame())
            {
                playerAnimator.attack = true;
                attackGameObject.SetActive(true);
                Vector2 origin = cap.offset + (Vector2)gameObject.transform.position;
                RaycastHit2D attackCast = Physics2D.Linecast(origin, origin + directionRoughValue * attackSize);
                float attackRotation = Mathf.Rad2Deg * (Mathf.Atan2(1, 0) - Mathf.Atan2(directionRoughValue.y, directionRoughValue.x) * -1);
                float size = attackSize;
                if (attackCast.collider != null)
                {
                    size = attackCast.distance;
                }
                float attackOffset = (size / 2 - 1) * (-1);
                attackGameObject.transform.localPosition = new Vector3(directionRoughValue.x, directionRoughValue.y, 0);
                attackGameObject.transform.rotation = Quaternion.Euler(0, 0, attackRotation);
                attackHitBox.size = new Vector2(1, size);
                attackHitBox.offset = new Vector2(0, attackOffset);
                StartCoroutine(AttackCooldown());
            }
        }

        //Detect Walls
        if(rayDict["Left"].touching)
        {
            walls = -1;
        }
        else if(rayDict["Right"].touching)
        {
            walls = 1;
        }
        else if(rayDict["Down"].touching == false && rayDict["Down Right"].touching && right == false)
        {
            Vector2 startPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y) + cap.offset + Vector2.down;
            RaycastHit2D wallCast = Physics2D.Linecast(startPos, startPos + Vector2.right * 0.55f);
            if(wallCast.collider != null)
            {
                walls = 1;
            }
            else
            {
                walls = 0;
            }
        }
        else if (rayDict["Down"].touching == false && rayDict["Down Left"].touching && left == false)
        {
            Vector2 startPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y) + cap.offset + Vector2.down;
            RaycastHit2D wallCast = Physics2D.Linecast(startPos, startPos + Vector2.left * 0.55f);
            if (wallCast.collider != null)
            {
                walls = -1;
            }
            else
            {
                walls = 0;
            }
        }
        else
        {
            walls = 0;
        }

        //Slope Detection And Handling
        //This Code will process the directional data to allow for more accurate detection
        RaycastHit2D rightRay = rayDict["Down Right"].raycast;
        RaycastHit2D leftRay = rayDict["Down Left"].raycast;
        RaycastHit2D downRay = rayDict["Down"].raycast;
        down = false;
        if (downRay.distance < 1.35 && Vector2.Angle(downRay.normal, Vector2.up) > minSlopeAngle)
        {
            down = true;
        }
        if (rayDict["Down Right"].touching)
        {
            bool rightInRange = false;
            rightAngle = Vector2.Angle(rightRay.normal, Vector2.up);
            if (rightAngle > minSlopeAngle && rightAngle < maxSlopeAngle)
            {
                rightInRange = true;
            }
            if (rightRay.distance <= 1.28f && rightInRange)
            {
                right = true;
            }
            else if (down && rightInRange)
            {
                right = true;
            }
            else if (down == false)
            {
                right = false;
            }
        }
        else
        {
            right = false;
            rightAngle = 0;
        }
        if (rayDict["Down Left"].touching)
        {
            bool leftInRange = false;
            leftAngle = Vector2.Angle(leftRay.normal, Vector2.up);
            if (leftAngle > minSlopeAngle && leftAngle < maxSlopeAngle)
            {
                leftInRange = true;
            }
            if (rightRay.distance <= 1.28f && leftInRange)
            {
                left = true;
            }
            else if (down && leftInRange)
            {
                left = true;
            }
            else if (down == false)
            {
                left = false;
            }
        }
        else
        {
            left = false;
            leftAngle = 0;
        }
        if (jumping == false)
        {
            if (directionRoughValue.x < 0 && right || directionRoughValue.x > 0 && left)
            {
                playerb.velocity = new Vector2(playerb.velocity.x, (-1 - leftAngle - rightAngle) / 4);
            }
            else if (directionRoughValue.x == 0 && right | left)
            {
                playerb.gravityScale = 0;
                playerb.velocity = new Vector2(playerb.velocity.x, 0);
            }
            else if (playerb.gravityScale == 0)
            {
                playerb.gravityScale = 1;
            }
            else if (right == false && left == false && down == true)
            {
                playerb.velocity = new Vector2(playerb.velocity.x, -1);
            }
        }
    }

    IEnumerator JumpBuffer()
    {
        yield return new WaitForSeconds(.1f);
        jumping = false;
    }

    IEnumerator AttackCooldown()
    {
        attackBufferRunning = true;
        yield return new WaitForSeconds(attackLengthSeconds);
        attackGameObject.transform.localPosition = Vector3.zero;
        attackHitBox.offset = Vector2.zero;
        attackHitBox.size = Vector2.right * 2;
        attackGameObject.SetActive(false);
        playerAnimator.attack = false;
        yield return new WaitForSeconds(attackCooldownSeconds);
        attackBufferRunning = false;
    }
}