/******************************************************************************
 * Player behavior script, handles movement and collision with items.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using Unity.Netcode;
using UnityEngine;

public class PlayerBehavior : NetworkBehaviour
{
    [SerializeField]
    private float walkSpeed = 0.2f;

    [SerializeField]
    private Vector2 defaultPositionRange = new Vector2(-4, 4);

    [SerializeField]
    private NetworkVariable<float> forwardBackPosition = new NetworkVariable<float>();

    [SerializeField]
    private NetworkVariable<float> leftRightPosition = new NetworkVariable<float>();

    PlayerInventory playerInventory = null;

    // client caches positions
    private float oldForwardBackwardPosition;
    private float oldLeftRightPosition;

    void Start()
    {
        //transform.position = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 0,
        //       Random.Range(defaultPositionRange.x, defaultPositionRange.y));
        transform.position = new Vector3(0,0,0);
        playerInventory = gameObject.GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (IsServer)
            UpdateServer();

        if(IsClient && IsOwner)
            UpdateClient();
    }

    private void UpdateServer()
    {
        transform.position = new Vector3(transform.position.x + leftRightPosition.Value, 
            transform.position.y + forwardBackPosition.Value, transform.position.z);
    }

    private void UpdateClient()
    {
        float forwardBackward = 0;
        float leftRight = 0;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            forwardBackward += walkSpeed;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            forwardBackward -= walkSpeed;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            leftRight -= walkSpeed;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            leftRight += walkSpeed;
        }

        if (oldForwardBackwardPosition != forwardBackward ||
            oldLeftRightPosition != leftRight)
        {
            UpdateClientPositionServerRpc(forwardBackward, leftRight);
            oldForwardBackwardPosition = forwardBackward;
            oldLeftRightPosition = leftRight;
        }
    }

    [ServerRpc]
    public void UpdateClientPositionServerRpc(float forwardBackward, float leftRight)
    {
        forwardBackPosition.Value = forwardBackward;
        leftRightPosition.Value = leftRight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player collision detected");
        Debug.Log("Item found: " + collision.gameObject.GetComponent<ItemBehavior>().GetItemType().GetName());
        if(collision.CompareTag("Item"))
        {
            playerInventory.AddItem(collision.gameObject.GetComponent<ItemBehavior>().GetItemType());
        }
    }
}