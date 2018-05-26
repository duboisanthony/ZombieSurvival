using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class used by the bullet shot by the player
//For moving forward
public class Mover : MonoBehaviour
{
    public float speed;
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();        
    }

    void Update()
    {
        rb.velocity = transform.forward * speed;
    }
}
