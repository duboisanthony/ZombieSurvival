using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MyCompany.MyGame
{

    public class Zombie : Photon.PunBehaviour, IPunObservable
    {
        private Animator _anim;
        private Rigidbody _rb;
        public GameObject target;
        public GameObject currentTarget;
        public GameObject[] PlayerTargets;
        //closest object
        public GameObject closestObject;
        bool playerInRange;
        float timer;
        //rotation speed
        public float rotationDegreesPerSecond = 360;
        public GameObject Payload;
        public float timeBetweenAttacks = 0.5f;
        public int attackDamage = 10;
        public float arriveMagnitude = 5.0f;
        public float acceleration;        

        void Awake()
        {
            // Setting up the references.
            _anim = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();            
            Payload = GameObject.FindGameObjectWithTag("Payload");            
        }

        // Use this for initialization
        void Start()
        {

        }
        
        // Update is called once per frame
        void Update()
        {
            CheckProximity();
            currentTarget = closestObject;
            timer += Time.deltaTime;
            if (timer >= timeBetweenAttacks && playerInRange)
            {
                Attack();
            }            
            Seek();
        }

        //Seek function
        void Seek()
        {
            if (!playerInRange)
            {
                Vector3 direction = new Vector3(currentTarget.transform.position.x - transform.position.x, 0.0f, currentTarget.transform.position.z - transform.position.z);
                direction = Vector3.ClampMagnitude(direction, 1.0f);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rotationDegreesPerSecond * Time.deltaTime);
                transform.position += transform.forward * Time.deltaTime * acceleration;
                _anim.SetBool("isWalking", true);
            }            
        }

        //Attack Function
        void Attack()
        {
            timer = 0f;
            if (currentTarget.tag == "Player") { 
            if (currentTarget.GetComponent<PlayerHealth>().currentHealth > 0)
            {
                currentTarget.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
            }
        }
        if (currentTarget.tag == "Payload") { 
            if (currentTarget.GetComponent<Payload>().currentHealth > 0)
            {
                currentTarget.GetComponent<Payload>().TakeDamage(attackDamage);
                }
}
        }

        //Check for collision for attacking
        void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject.tag == "Player") || (other.gameObject.tag == "Payload"))
            {
                playerInRange = true;
                _anim.SetBool("isWalking", false);
                _anim.SetBool("isAttacking", true);
            }
        }

        //Out of range
        void OnTriggerExit(Collider other)
        {
            if ((other.gameObject.tag == "Player") || (other.gameObject.tag == "Payload"))
            {
                playerInRange = false;
                _anim.SetBool("isAttacking", false);
                _rb.constraints &= ~RigidbodyConstraints.FreezeAll;
            }
        }
        
        //Calculate distance between targets
        private void CheckProximity()
        {
            var objectsWithTag = GameObject.FindGameObjectsWithTag("Player");

            for (int i = 0; i < objectsWithTag.Length; i++)
            {
                if (closestObject == null)
                {
                    closestObject = objectsWithTag[i];
                }
                //compares distances
                if (Vector3.Distance(transform.position, objectsWithTag[i].transform.position) <= Vector3.Distance(transform.position, closestObject.transform.position))
                {
                    closestObject = objectsWithTag[i];
                }
            }
            if(Vector3.Distance(transform.position, Payload.transform.position) <= Vector3.Distance(transform.position, closestObject.transform.position))
            {
                closestObject = Payload;
            }
        }

        //Not used
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
            }
            else
            {
            }
        }
    }
}