using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopController : MonoBehaviour
{
    public float delta = 1.5f;  // Amount to move left and right from the start point
    public float speed = 2.0f;  // Speed it moves from left to right
    Vector3 startPos;   // Starting position of the hoop

    void Start ()
    {
        // Initialize the starting position of the hoop
        startPos = transform.position;  
    }
    
    void Update ()
    {
        // Update the position of the hoop to create a left-to-right movement
        Vector3 v = startPos;  
        v.x += delta * Mathf.Sin(Time.time * speed);  
        transform.position = v;  
    }
}
