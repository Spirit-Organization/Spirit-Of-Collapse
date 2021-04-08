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
    private Scene scene;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        m_OrthographicCamera = gameObject.GetComponent<Camera>();
        player = GameObject.FindWithTag("Player").GetComponent<ControlsCSharp>().Player;
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
            if (scene.name == "Scene4") //checks for pillbug bossfight
            {
                transform.position = new Vector3(12.93f, 7.7f, -100f); //sets size for pillbug bossfight
                m_OrthographicCamera.orthographicSize = 15;
            }
            else
            {

                transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2, -100f); //sets for standard camera size and movement
                m_OrthographicCamera.orthographicSize = DefaultSize;
            }
        }

    }

    IEnumerator CameraShake()
    {
        ShakeDelay -= Time.deltaTime; //shake delay

        if (ShakeDelay <= 0.0f)
        {
            ShakeDelay = 0.2f;
            if (scene.name == "Scene4") //checks for stationary scene
            {
                transform.Rotate(new Vector3(0f, 0f, 1f)); //sets for stationary scene
                rb.MovePosition(new Vector2(transform.position.x - 0.5f,transform.position.y - 0.5f));
                yield return new WaitForSeconds(0.1f);
                transform.Rotate(new Vector3(0f, 0f, -1f));
                rb.MovePosition(new Vector2(transform.position.x + 0.5f, transform.position.y + 0.5f));
            }
            else
            {
                transform.Rotate(new Vector3(0f, 0f, 1f)); //sets for normal scene
                rb.MovePosition(new Vector2(player.transform.position.x - 0.5f, player.transform.position.y - 0.5f));
                yield return new WaitForSeconds(0.1f);
                transform.Rotate(new Vector3(0f, 0f, -1f));
                rb.MovePosition(new Vector2(player.transform.position.x + 0.5f, player.transform.position.y + 0.5f));
            }
        }
    }
}
