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
        playerWooshSound = Resources.Load<AudioClip> ("Swing whoosh");
        playerJumpSound = Resources.Load<AudioClip> ("Jumpwhoosh-3");

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
            case "Swing whoosh":
                audioSrc.PlayOneShot (playerWooshSound, 0.1f);
                break;
            case "Jumpwhoosh-3": 
                audioSrc.PlayOneShot (playerJumpSound, 0.1f);
                break;

        }
    }
}

