using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    private int PlayerHealth;
    private SpriteRenderer sr;
    private int MaxHealth;
    // Start is called before the first frame update
    void Start()
    {
        sr = this.gameObject.GetComponent<SpriteRenderer>();
        PlayerHealth = GameObject.FindWithTag("Player").GetComponent<ControlsCSharp>().Health;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerHealth = GameObject.FindWithTag("Player").GetComponent<ControlsCSharp>().Health;
        MaxHealth = GameObject.FindWithTag("Player").GetComponent<ControlsCSharp>().MaxHealth;
        if (PlayerHealth < 1)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a + 0.5f * Time.deltaTime);
            StartCoroutine(Respawn());
        }
  
    }

    
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(4f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - 0.5f * Time.deltaTime);
    }
}
