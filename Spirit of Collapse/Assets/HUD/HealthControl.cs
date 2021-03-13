using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthControl : MonoBehaviour
{

    public GameObject[] Hearts;
    public Sprite Healthy;
    public Sprite NotHealthy;
    public GameObject Player;
    private int index = 0;
    private int MaxHealth;
    private GameObject Heart6;
    private GameObject Heart7;
    private GameObject Heart8;
    private GameObject Heart9;

    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        MaxHealth = Player.GetComponent<ControlsCSharp>().MaxHealth;
        Heart6 = Hearts[5] as GameObject;
        Heart7 = Hearts[6] as GameObject;
        Heart8 = Hearts[7] as GameObject;
        Heart9 = Hearts[8] as GameObject;
    }

    void Update()
    {   //changes the sprite of the heath depending on the health value
        index = 0;
        MaxHealth = Player.GetComponent<ControlsCSharp>().MaxHealth;

        foreach (GameObject CurrentObject in Hearts)
        {
            index += 1;

            if (Player.GetComponent<ControlsCSharp>().Health >= index)
            {
                CurrentObject.GetComponent<Image>().sprite = Healthy;
            }
            else
            {
                CurrentObject.GetComponent<Image>().sprite = NotHealthy;
            }
        }
        ///shows extra hearts
        if (MaxHealth == 6)
        {
            Heart6.SetActive(true);
            Heart7.SetActive(false);
            Heart8.SetActive(false);
            Heart9.SetActive(false);
        }
        else if (MaxHealth == 7)
        {
            Heart6.SetActive(true);
            Heart7.SetActive(true);
            Heart8.SetActive(false);
            Heart9.SetActive(false);
        }
        else if (MaxHealth == 8)
        {
            Heart6.SetActive(true);
            Heart7.SetActive(true);
            Heart8.SetActive(true);
            Heart9.SetActive(false);
        }
        else if (MaxHealth == 9)
        {
            Heart6.SetActive(true);
            Heart7.SetActive(true);
            Heart8.SetActive(true);
            Heart9.SetActive(true);
        }
        else
        {
            Heart6.SetActive(false);
            Heart7.SetActive(false);
            Heart8.SetActive(false);
            Heart9.SetActive(false);
        }
    }
}
