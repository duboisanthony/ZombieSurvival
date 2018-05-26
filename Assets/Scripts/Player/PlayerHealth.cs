using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class PlayerHealth : Photon.PunBehaviour, IPunObservable
    {

        public int BaseHealth = 100;                                // Health of the player
        public int currentHealth;                                   // Current health of the player
        public Image damageImage;                                   // Damage image
        public float flashSpeed = 5f;                               // fading speed
        public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // color of damage image

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        static public GameManager Instance;

        PlayerHealth ThisHealth;

        Animator anim;                                              // Animator
        PlayerController2 playerMovement;                           // PlayerController
        Shoot playerShooting;                                       // PlayerShooting script.
        bool isDead;                                                // Dead bool
       public bool damaged;                                               // IsDamaged bool

       // public GUIText gameOverText;

        [Tooltip("The Player's UI GameObject Prefab")]
        public GameObject PlayerUiPrefab;

        void Awake()
        {
            ThisHealth = this;
            anim = GetComponent<Animator>();
            playerMovement = GetComponent<PlayerController2>();
            playerShooting = GetComponentInChildren<Shoot>();
            currentHealth = BaseHealth;
        }

        void Start()
        {
            //instantiate player UI
            if (PlayerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerUiPrefab, GameObject.FindObjectOfType<Canvas>().transform) as GameObject;
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

#if UNITY_MIN_5_4
// Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        };
#endif
        }

#if !UNITY_MIN_5_4
        /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
#endif


        void CalledOnLevelWasLoaded(int level)
        {
           
        }

        void Update()
        {            
            if (damaged)
            {
                damageImage.color = flashColour;
            }            
            else
            {
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }            
            damaged = false;
        }


        public void TakeDamage(int amount)
        {            
            damaged = true;
            currentHealth -= amount;
            if (currentHealth <= 0 && !isDead)
            {
                Death();
            }
        }

        //When the player dies
        void Death()
        {
            isDead = true;
            playerShooting.DisableEffects();
            anim.SetTrigger("Die");            
            playerMovement.enabled = false;
            playerShooting.enabled = false;
            PhotonNetwork.LoadLevel("GameOver");
        }

        public void GameOver()
        {
           // gameOverText.text = "Game Over!";
           // gameOver = true;
        }

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