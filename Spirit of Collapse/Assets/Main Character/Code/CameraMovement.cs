using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMovement : MonoBehaviour
{
    private float ShakeDelay;
    private Rigidbody2D rb;
    private Camera m_OrthographicCamera;
    private GameObject player;
    public float DefaultSize;
    public bool CameraShakes = false;
    public float BoxXSize;
    public float BoxYSize;
    private Scene scene;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        m_OrthographicCamera = gameObject.GetComponent<Camera>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().gameObject;
        scene = SceneManager.GetActiveScene();
    }

    void Update()
    {
        if (CameraShakes == true) //checks for camera shaking
        {
            StartCoroutine(CameraShake()); //starts camera shake
        }
        else
        {

            if (player.transform.position.x > transform.position.x + BoxXSize)
            {
                transform.position = new Vector3(player.transform.position.x - BoxXSize, transform.position.y, -100f);

            }
            if (player.transform.position.x < transform.position.x - BoxXSize)
            {
                transform.position = new Vector3(player.transform.position.x + BoxXSize, transform.position.y, -100f);

            }

            if (player.transform.position.y > transform.position.y + BoxYSize)
            {
                transform.position = new Vector3(transform.position.x, player.transform.position.y - BoxYSize, -100f);

            }
            if (player.transform.position.y < transform.position.y - BoxYSize)
            {
                transform.position = new Vector3(transform.position.x, player.transform.position.y + BoxYSize, -100f);

            }


            m_OrthographicCamera.orthographicSize = DefaultSize;
        }
    } 

    IEnumerator CameraShake()
    {
        ShakeDelay -= Time.deltaTime; //shake delay

        if (ShakeDelay <= 0.0f)
        {
            ShakeDelay = 0.2f;
         
                transform.Rotate(new Vector3(0f, 0f, 1f)); //sets for normal scene
                rb.MovePosition(new Vector2(player.transform.position.x - 0.5f, player.transform.position.y - 0.5f));
                yield return new WaitForSeconds(0.1f);
                transform.Rotate(new Vector3(0f, 0f, -1f));
                rb.MovePosition(new Vector2(player.transform.position.x + 0.5f, player.transform.position.y + 0.5f));
            
        }
    }
}
