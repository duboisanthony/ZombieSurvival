using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Script to move and rotate the player
public class PlayerController2 : Photon.PunBehaviour
{

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;
    public float speed = 6f;            // Speed of the player.
    Vector3 movement;                   // Vector of the player movement.
    Animator anim;                      // Animator.
    Rigidbody rb;                       // rigidbody.
    int FloorLayer;                     // The layer of the floor.
    float CameraRay = 100f;             // Length of the camera Ray.

        void Awake()
        {        
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.isMine)
        {
            PlayerController2.LocalPlayerInstance = this.gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
        FloorLayer = LayerMask.GetMask("Floor");
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
        if (photonView.isMine == false && PhotonNetwork.connected == true)
        {
            return;
        }
        //Input Axes.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

            // Moving the player
            Move(h, v);
            //Turn the player towards the mouse
            Turning();
            //Animate the player
            Animating(h, v);
        }

        void Move(float h, float v)
        {
        // Set the movement vector based on the axis input.
            movement.Set(h, 0f, v);

        // Normalize vector and normalize
            movement = movement.normalized * speed * Time.deltaTime;

        //Move the player
            rb.MovePosition(transform.position + movement);
        }

        //Function for turning the player
        void Turning()
        {
            // Ray from mouse to camera position
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            //What is hit by the Ray
            RaycastHit RayHit;

            // If the ray it the floor layer
            if (Physics.Raycast(camRay, out RayHit, CameraRay, FloorLayer))
            {
                // Vector from the player to the hit point of the ray
                Vector3 playerToMouse = RayHit.point - transform.position;

                //Make sure the Ray doesnt hit something below the floor
                playerToMouse.y = 0f;

                //Rotation vector from the player to the mouse.
                Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

                //Send the rotation to rigidbody
                rb.MoveRotation(newRotation);
            }
        }

        void Animating(float h, float v)
        {
            // Bool that is true if the player is moving
            bool walking = h != 0f || v != 0f;
  
            //Send the walking bool to the animator
            anim.SetBool("IsWalking", walking);
        }
    }