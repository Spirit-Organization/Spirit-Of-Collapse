using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManageScript : MonoBehaviour
{

    public static AudioClip playerWooshSound, playerHitSound, playerJumpSound, playerWalkSound;
    static AudioSource audioSrc; 
    private Animator animations;
    bool isMoving = false;
 

    // Start is called before the first frame update
    void Start()
    {
        playerWooshSound = Resources.Load<AudioClip> ("Swing whoosh");
        playerJumpSound = Resources.Load<AudioClip> ("Jumpwhoosh-3");
        playerWalkSound = Resources.Load<AudioClip> ("walking-1");
        audioSrc = GetComponent<AudioSource> ();
        animations = gameObject.transform.Find("Animator").gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()//animations.GetInteger("Animation To Play")
    {  
        if (animations.GetInteger("Animation To Play")== 2)
        {
            isMoving = true;
        } 
        else 
        {
            isMoving = false;
        }
    }

    public static void PlaySound (string clip)
    {
        switch (clip)
        {
            case "Swing whoosh":
                audioSrc.PlayOneShot (playerWooshSound, 0.1f);
                break;
            case "Jumpwhoosh-3": 
                audioSrc.PlayOneShot (playerJumpSound, 0.1f);
                break;

        }
    }
}

