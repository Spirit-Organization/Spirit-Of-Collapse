using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingplatform : MonoBehaviour
{

    public GameObject platform;
    public float moveSpeed;
    public Transform currentpoint;
    public Transform[] points;
    public int pointSelection;

    // Start is called before the first frame update
    void Start()
    {
        currentpoint = points[pointSelection];
    }

    // Update is called once per frame
    void Update()
    {
        platform.transform.position = Vector3.MoveTowards(platform.transform.position, currentpoint.position, Time.deltaTime * moveSpeed);

        if(platform.transform.position == currentpoint.position)
        {
            pointSelection++;
        }

        if(pointSelection == points.Length)
        {
            pointSelection = 0;
        }
    }    
}
