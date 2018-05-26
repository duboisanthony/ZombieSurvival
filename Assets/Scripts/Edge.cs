using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour {

    public GameObject startNode;
    public GameObject endNode;

    public float weight;
	// Use this for initialization
	void Start () {
        weight = (endNode.transform.position - startNode.transform.position).magnitude;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
