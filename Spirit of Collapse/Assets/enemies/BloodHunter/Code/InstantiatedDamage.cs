using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatedDamage : MonoBehaviour
{
    public int DamageToInflict;


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<ControlsCSharp>().DamageIncoming = DamageToInflict;
            Destroy(this.gameObject);
        }
    }
}