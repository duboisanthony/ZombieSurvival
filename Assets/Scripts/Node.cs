using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    // Use this for initialization
    public Edge[] edgeList;
    public bool endNode;
    
    void Start () {
        edgeList = GetComponentsInChildren<Edge>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
