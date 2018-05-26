using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to destroy shots going out of the game
public class DestroyByBoundary : MonoBehaviour {
    
        void OnTriggerExit(Collider other)
        {
        if(other.tag == "Bolt")
            Destroy(other.gameObject);
        }
    }
