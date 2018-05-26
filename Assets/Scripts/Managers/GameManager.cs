using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Com.MyCompany.MyGame
{
    public class GameManager : Photon.PunBehaviour
    {
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        static public GameManager Instance;
        public GameObject Zombie4;
        public GameObject BasicZombie;          // The enemy prefab to be spawned.
        private float spawnTime = 15f;            // How long between each spawn.
        public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
        public GameObject _payload;
        Payload _payloadScript;

        #region Photon Messages
        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
        #endregion

        #region Private Methods

        void LoadArena()
        {
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
            PhotonNetwork.LoadLevel("scene1");
        }

        #endregion

        #region Photon Messages


        public override void OnPhotonPlayerConnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                LoadArena();
            }
        }


        public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                LoadArena();
            }
        }


        #endregion
        #region Public Methods

        void Start()
        {
            Instance = this;

            _payload = GameObject.FindGameObjectWithTag("Payload");
            _payloadScript = _payload.GetComponent<Payload>();
            

            if (PlayerController2.LocalPlayerInstance == null)
            {
                Debug.Log("We are Instantiating LocalPlayer from " + Application.loadedLevelName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, -20f), Quaternion.identity, 0);
                
            }
            else
            {
                Debug.Log("Ignoring scene load for " + Application.loadedLevelName);
            }
            PhotonNetwork.Instantiate(this.BasicZombie.name, spawnPoints[1].position, spawnPoints[1].rotation, 0);

            // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
            InvokeRepeating("Spawn", spawnTime, spawnTime);

        }
        
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }


        #endregion
               
             
        //Function to spawn The waves of zombies depending on which node the Payload is at
        void Spawn()
        {           
            // Random spawn point          
            float TypeOfZombie = Random.Range(0f, 1.0f);
            int spawnPointIndex;
            //Starting node of the level
            if (_payloadScript.currentTarget == _payloadScript.nodeList[1])
            {
                spawnTime = 10;
                spawnPointIndex = Random.Range(0, 2);
                if (TypeOfZombie >= 0.20f)
                {
                     PhotonNetwork.Instantiate(this.BasicZombie.name, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation, 0);
                }
                else if (TypeOfZombie < 0.20f)
                {
                    PhotonNetwork.Instantiate(this.Zombie4.name, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation, 0);
                }
            }
            else if(_payloadScript.currentTarget == _payloadScript.nodeList[2])
            {
                spawnPointIndex = Random.Range(2, 4);
                spawnTime = 7;
                if (TypeOfZombie >= 0.10f)
                {
                    PhotonNetwork.Instantiate(this.BasicZombie.name, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation, 0);
                }
                else if (TypeOfZombie < 0.20f)
                {
                    PhotonNetwork.Instantiate(this.Zombie4.name, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation, 0);
                }
            }
            else if (_payloadScript.currentTarget == _payloadScript.nodeList[3])
            {
                spawnPointIndex = Random.Range(4, 6);
                spawnTime = 4f;
                if (TypeOfZombie >= 0.10f)
                {
                    PhotonNetwork.Instantiate(this.BasicZombie.name, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation, 0);
                }
                else if (TypeOfZombie < 0.10f)
                {
                    PhotonNetwork.Instantiate(this.Zombie4.name, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation, 0);
                }
            }
            else if(_payloadScript.currentTarget == _payloadScript.nodeList[4] || _payloadScript.currentTarget == _payloadScript.nodeList[5] || _payloadScript.currentTarget == _payloadScript.nodeList[6])
            {
                spawnPointIndex = Random.Range(6, 8);
                spawnTime = 2f;
                if (TypeOfZombie >= 0.20f)
                {
                    PhotonNetwork.Instantiate(this.BasicZombie.name, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation, 0);
                }
                else if (TypeOfZombie < 0.20f)
                {
                    PhotonNetwork.Instantiate(this.Zombie4.name, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation, 0);
                }
            }

        }
    }
}
