using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHUDReActivate : MonoBehaviour
{
    private int Health;
    private GameObject Player;
    public GameObject TopHud;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        Health = Player.GetComponent<ControlsCSharp>().Health;
    }

    // Update is called once per frame
    void Update()
    {
        Health = Player.GetComponent<ControlsCSharp>().Health;
        if (Health > 0)
        {
            TopHud.SetActive(true);
        }
    }
}
