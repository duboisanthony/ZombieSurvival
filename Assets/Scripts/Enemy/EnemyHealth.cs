using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.MyCompany.MyGame
{
    public class EnemyHealth : Photon.PunBehaviour, IPunObservable
    {

        public int startingHealth = 100;            // Health of the enemy.
        public int currentHealth;                   // Current health of the enemy.


        Animator anim;                              // Animator
        AudioSource enemyAudio;                     // Audio source.
        CapsuleCollider capsuleCollider;            // Capsule collider.
        Rigidbody _rb;
        bool isDead;                                // Bool enemy is dead.
        Zombie ZombieScript;
        GoapAgent goapAgent;

        public bool IsDead
        {
            get { return isDead; }
        }

        void Awake()
        {
            // Setting up the references.
            anim = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
            enemyAudio = GetComponent<AudioSource>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            ZombieScript = GetComponent<Zombie>();
            goapAgent = GetComponent<GoapAgent>();

            // Setting the current health when the enemy first spawns.
            currentHealth = startingHealth;
        }

        void Update()
        {
           
        }


        public void TakeDamage(int amount, Vector3 hitPoint)
        {
            // When enemy killed
            if (isDead)
                return;          

            // Substract the damage from de health
            currentHealth -= amount;

           
            if (currentHealth <= 0)
            {
                // Enemy is dead
                Death();
            }
        }

        //Used for bullets
        public void TakeDamage(int amount)
        {
           
            if (isDead)
                return;

            // Substract the damage from de health
            currentHealth -= amount;
            
            
            if (currentHealth <= 0)
            {
                // Enemy is dead
                Death();
            }
        }


        void Death()
        {
            // Set Dead boolean
            isDead = true;

            //remove from shootable layer
            gameObject.layer = 0;

            // Turn the collider into a trigger so shots can pass through it.
            capsuleCollider.isTrigger = true;
            _rb.detectCollisions = false;

            // Tell the animator that the enemy is dead.
            anim.SetTrigger("Dead");

            //Stop enemy from moving
             _rb.constraints = RigidbodyConstraints.FreezePosition;
            goapAgent.enabled = !goapAgent.enabled;
        }

        //Sync Health with Photon
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(currentHealth);
            }
            else
            {
                this.currentHealth = (int)stream.ReceiveNext();
            }
        }
    }
}