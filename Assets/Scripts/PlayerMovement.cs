using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{

    /// <summary>
    /// Get the rigidbody for the Player.
    /// </summary>
    [SerializeField]
    Rigidbody2D playerRigidBody = null;

    /// <summary>
    /// Player speed.
    /// </summary>
    [SerializeField]
    private float moveSpeed = 7f;
            /// <summary>
    /// Direction to move the player.
    /// </summary>
    private Vector2 moveDir;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }
             // new code here - above code is all from tutorial

    void FixedUpdate() {
        PlayerMove();
    }


    void Movement() {
            // Note: GetAxisRaw was used so that values are always -1, 0, or 1.
        //          This makes it easier to switch between sprite animations
        //          since the values stay the same. With GetAxis the values
        //          could be 1.1, 0.5, etc making it harder to animate.

        // Grab input (A, D, Joystick left, Joystick right)
        float h = Input.GetAxisRaw("Horizontal");
        // Grab input (W, S, Joystick up, Joystick down)
        float v = Input.GetAxisRaw("Vertical");

        // Calculate move direction based on input and normalize it to prevent
        // faster movement in the diagonal direction
        moveDir = new Vector2(h, v).normalized;
    }

        /// <summary>
    /// Function to move player
    /// </summary>
    private void PlayerMove()
    {
        if (NetworkManager.Singleton.IsServer)
        {
                    Vector2 moveAmount = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);

        #if (DEBUG)
                //Debug.Log("Movement: " + moveAmount);
                //Debug.Log(playerRigidBody.position);
        #endif

        // Move the player
        playerRigidBody.MovePosition(playerRigidBody.position + moveAmount * Time.fixedDeltaTime);
        }

    }
}
