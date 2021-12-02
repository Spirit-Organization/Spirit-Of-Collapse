 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mud : MonoBehaviour
{
    ControlsCSharp CCS;
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
            CCS.DoubleJump = true;
        }
    }



    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            CCS.Speed = speed / 2;
            CCS.DoubleJump = false;
        }
    }

}