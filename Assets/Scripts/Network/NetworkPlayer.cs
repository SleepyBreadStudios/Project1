using Unity.Netcode;
using UnityEngine;

    public class NetworkPlayer : NetworkBehaviour
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

        /// <summary>
        /// Captures inputs for a character on a client and sends them to the server.
        /// </summary>
        //[RequireComponent(typeof(ServerCharacter))]
        public NetworkVariable<Vector2> Position = new NetworkVariable<Vector2>();

        public override void OnNetworkSpawn()
        {
            // if (!IsClient || !IsOwner)
            // {
            //     enabled = false;
            //     // dont need to do anything else if not the owner
            //     return;
            // }
            if(IsOwner) 
            {
                Move();
            }
        }

        public void Move()
        {
            // if (IsOwner)
            // {
                if (NetworkManager.Singleton.IsServer)
                {
                    var randomPosition = GetRandomPositionOnPlane();
                    transform.position = randomPosition;
                    Position.Value = randomPosition;
                    //Position.Value = PlayerMovement();
                    //PlayerMove();
                }
                else
                {
                    SubmitPositionRequestServerRpc();
                }
            // }
        }

        [ServerRpc]
        void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            Position.Value = GetRandomPositionOnPlane();
            //PlayerMove();
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        }

        Vector2 PlayerMovement()
        {
            //return new Vector2(Position.Value.x, Position.Value.y + 10);
            return new Vector2(2, Position.Value.y + 1);
        }

        void Update()
        {
            transform.position = Position.Value;
            //Movement();
        }

        void FixedUpdate() 
        {
            //Move();
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
        Vector2 moveAmount = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);

        #if (DEBUG)
                //Debug.Log("Movement: " + moveAmount);
                //Debug.Log(playerRigidBody.position);
        #endif

        // Move the player
        playerRigidBody.MovePosition(playerRigidBody.position + moveAmount * Time.fixedDeltaTime);
        

    }
    }


