using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mud : MonoBehaviour
{
    ControlsCSharp controlsCSharp;
    public float TrueJump;
    public int speed;

    private void Update()
    {

        TrueJump = gameObject.GetComponent<ControlsCSharp>().TrueJump;
        
       // possibly need to add definition on what touching ground means
            }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            controlsCSharp.DoubleJump = true;
        }
    }



    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            controlsCSharp.Speed = speed;
            controlsCSharp.DoubleJump = false;
        }
    }

}