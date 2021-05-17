using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneOpen : MonoBehaviour
{
    private Image sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
       sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - 0.5f * Time.deltaTime);
    }
}
