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
    public int DefaultSize;
    public bool CameraShakes = false;
    private Scene scene;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        m_OrthographicCamera = gameObject.GetComponent<Camera>();
        player = GameObject.Find("Player").GetComponent<ControlsCSharp>().Player;
        scene = SceneManager.GetActiveScene();

    }

    void Update()
    {
        if (CameraShakes == true)
        { 
            StartCoroutine(CameraShake());
        }
        else 
        {
            if (scene.name == "Scene4")
            {
                transform.position = new Vector3(12.93f, 7.7f, -100f);
                m_OrthographicCamera.orthographicSize = 15;
            }
            else
            {

                transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2, -100f);
                m_OrthographicCamera.orthographicSize = DefaultSize;
            }
        }
    }

    IEnumerator CameraShake()
    {
        ShakeDelay -= Time.deltaTime;

        if (ShakeDelay <= 0.0f)
        {
            ShakeDelay = 0.2f;
            transform.Rotate(new Vector3(0f, 0f, 1f));
            rb.MovePosition(new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f));
            yield return new WaitForSeconds(0.1f);
            transform.Rotate(new Vector3(0f, 0f, -1f));
            rb.MovePosition(new Vector2(transform.position.x + 0.5f, transform.position.y + 0.5f));
        }
    }
}
