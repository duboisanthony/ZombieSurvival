using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    //Class for shooting shots
    public class Shoot : Photon.PunBehaviour
    {
        
        public float timeBetweenBullets = 0.15f;       //Shooting speed
        public float range = 100f;                      //Max Range
        float timer;                                    
        Ray shootRay;                                  
        RaycastHit shootHit;                           
        int shootableMask;                              
        public LineRenderer gunLine;                 
        public AudioSource gunAudio;                         
        float effectsDisplayTime = 0.2f;              
        public Animator anim;
        public bool shooting = false;
        public GameObject boltPrefab;

        void Awake()
        {
            // Create a layer mask for the Shootable layer.
            shootableMask = LayerMask.GetMask("Shootable");            
            gunLine = GetComponent<LineRenderer>();
            gunAudio = GetComponent<AudioSource>();
            anim = transform.parent.GetComponent<Animator>();
        }

        void Update()
        {
            if (photonView.isMine == false && PhotonNetwork.connected == true)
            {
                return;
            }
            timer += Time.deltaTime;
            shooting = false;
            if (photonView.isMine)
            {
                // When fire button is pressed
                if (Input.GetButton("Fire1") && timer >= timeBetweenBullets)
                {
                    _Shoot();
                }
                //Check attack speed
                if (timer >= timeBetweenBullets * effectsDisplayTime)
                {
                    DisableEffects();
                }
            }
        }

        public void DisableEffects()
        {
            gunLine.enabled = false;
            shooting = false;
        }

        void _Shoot()
        {
            timer = 0f;
            gunAudio.Play();
            gunLine.SetPosition(0, transform.position);
            shootRay.origin = transform.position + new Vector3(1f, 0f, 0.5f);
            shootRay.direction = transform.forward;

            //Instantiate a shot through the network
            PhotonNetwork.Instantiate(this.boltPrefab.name, this.gameObject.transform.position, this.gameObject.transform.rotation, 0);
            if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
            {
                gunLine.SetPosition(1, shootHit.point);
            }
            else
            {
                gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
            }
        }       
    }
}