//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PayloadAura : MonoBehaviour {

//    public GameObject payload;
//    private List<Collider> _zombiesList;
//    //private List<Player> _playerList;
//	// Use this for initialization
//	void Start () {
//        _zombiesList = new List<Collider>();
//	}
	
//	// Update is called once per frame
//	void Update () {
//        if (_zombiesList.Count <= 0)
//            payload.GetComponent<Payload>().canMove = true;
//        else
//            payload.GetComponent<Payload>().canMove = false;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        switch (other.gameObject.tag)
//        {
//            case "Node":
//                Debug.Log("HitNode : " + other.gameObject);
//                break;
//            case "Zombie":
//                Debug.Log("Zob " + other.gameObject + "arrived");
//                _zombiesList.Add(other);
//                break;
//        }
//    }

//    private void OnTriggerStay(Collider other)
//    {
//        switch (other.gameObject.tag)
//        {
//            case "Node":
//                //Debug.Log("TriggerStayNode : " + other.gameObject);
//                break;
//            case "Zombie":
//                //_zombiesList.Remove(other);
//                break;
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        switch (other.gameObject.tag)
//        {
//            case "Node":
//                //Debug.Log("TriggerStayNode : " + other.gameObject);
//                break;
//            case "Zombie":
//                Debug.Log("Zob " + other.gameObject + "leaves.");
//                _zombiesList.Remove(other);
//                break;
//        }
//    }
//}
