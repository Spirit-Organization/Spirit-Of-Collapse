using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreens : MonoBehaviour
{
    private bool ButtonDown;
    private Image sr;
    public GameObject images;
    public SceneManager NextScene;
    private bool HasBeenPressed;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<Image>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        ButtonDown = Input.anyKeyDown;
        if (ButtonDown == true || HasBeenPressed == true)
        {
            HasBeenPressed = true;

            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - 0.5f * Time.deltaTime);
            if (sr.color.a < 1)
            {
                this.gameObject.SetActive(false);
                if (images != null)
                {
                    images.SetActive(true);
                }
                else
                {
                    SceneManager.LoadScene(9);
                }
            }
        }
        else { sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a + 0.5f * Time.deltaTime); }     
    }
}
