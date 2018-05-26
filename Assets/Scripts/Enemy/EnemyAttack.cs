﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.MyCompany.MyGame
{
    
    //Script used in the early stages for testing. But not used anymore because of goap.
    public class EnemyAttack : MonoBehaviour
    {

        public float timeBetweenAttacks = 0.5f;    
        public int attackDamage = 10;            
        Animator anim;                            
        GameObject player;                          
        PlayerHealth playerHealth;                 
        EnemyHealth enemyHealth;                
        bool playerInRange;                       
        float timer;                       

        void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerHealth = player.GetComponent<PlayerHealth>();
            enemyHealth = GetComponent<EnemyHealth>();
            anim = GetComponent<Animator>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == player)
            {
                playerInRange = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject == player)
            {
                playerInRange = false;
            }
        }

        void Update()
        {            
            timer += Time.deltaTime;            
            if (timer >= timeBetweenAttacks && playerInRange && enemyHealth.currentHealth > 0)
            {
                Attack();
            }            
            if (playerHealth.currentHealth <= 0)
            {
                anim.SetTrigger("PlayerDead");
            }
        }

        void Attack()
        {
            timer = 0f;
            if (playerHealth.currentHealth > 0)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
}