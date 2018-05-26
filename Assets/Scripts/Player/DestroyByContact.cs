using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.MyCompany.MyGame
{
    //Script to destroy shots when hiting something
    public class DestroyByContact : Photon.PunBehaviour
    {
        public int damagePerShot;
        void OnTriggerEnter(Collider other)
        {
            //If hitting an enemy
            if ((other.gameObject.layer == 9))
            {
                EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();                
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damagePerShot);
                }
                PhotonNetwork.Destroy(gameObject);
            }    
            //If hitting the Payload
            if(other.tag == "Payload")
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }    


    }
}
