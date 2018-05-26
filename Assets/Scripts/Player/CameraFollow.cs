using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script for the camera to follow the player
public class CameraFollow : MonoBehaviour {

        public GameObject target;            
        public float smoothing = 5f;        // Speed of the camera.
        Vector3 offset;                   

        void Start()
        {
        target = GameObject.FindGameObjectWithTag("Player");            
        offset = transform.position - target.transform.position;
        }

        void FixedUpdate()
        {            
            Vector3 targetCamPos = target.transform.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
    }