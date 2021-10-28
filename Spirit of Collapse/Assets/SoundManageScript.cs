using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManageScript : MonoBehaviour
{

    public static AudioClip playerWooshSound, playerHitSound, playerJumpSound;  
    static AudioSource audioSrc; 


    // Start is called before the first frame update
    void Start()
    {
        playerWooshSound = Resources.Load<AudioClip> ("mixkit-arrow-whoosh-1491.wav");
        playerJumpSound = Resources.Load<AudioClip> ("Jumpwhoosh");

        audioSrc = GetComponent<AudioSource> ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound (string clip)
    {
        switch (clip)
        {
            case "mixkit-arrow-whoosh-1491.wav":
                audioSrc.PlayOneShot (playerWooshSound);
                break;
            case "Jumpwhoosh": 
                audioSrc.PlayOneShot (playerJumpSound);
                break;

        }
    }
}

