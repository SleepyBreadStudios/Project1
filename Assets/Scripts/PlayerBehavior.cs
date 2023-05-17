/******************************************************************************
 * Player behavior script, handles movement and collision with items.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
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

    [SerializeField]
    private GameObject InventoryUI = null;

    // projectile prefab
    [SerializeField]
    private GameObject projectile = null;

    // is inventory showing at the moment?
    private bool inventoryEnabled = false;

    // client caches positions
    private float oldForwardBackwardPosition;
    private float oldLeftRightPosition;

    void Start()
    {
        //transform.position = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 0,
        //       Random.Range(defaultPositionRange.x, defaultPositionRange.y));
        transform.position = new Vector3(0,0,0);
        playerInventory = gameObject.GetComponent<PlayerInventory>();
        InventoryUI.transform.localScale = new Vector3(0, 0, 0);
        
        // Sets random color when players spawn
        float rand = Random.Range(0, 256);
        float rand2 = Random.Range(0, 256);
        float rand3 = Random.Range(0, 256);
        rand = rand / 255.0f;
        rand2 = rand2 / 255.0f;
        rand3 = rand3 / 255.0f;
        
        gameObject.GetComponent<Renderer>().material.color = new Color(rand, rand2, rand3);
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

        // access inventory
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(inventoryEnabled)
            {
                InventoryUI.transform.localScale = new Vector3(0, 0, 0);
                inventoryEnabled = false;
            }
            else
            {
                InventoryUI.transform.localScale = new Vector3(1, 1, 1);
                inventoryEnabled = true;
            }
        }

        // shoot projectile
        if (Input.GetMouseButtonDown(0) && !inventoryEnabled)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("Mouse coords: " + mousePos.x + ", " + mousePos.y);
            
            // instantiate projectile
            GameObject projectileObj = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;

            // set orientation
            projectileObj.transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - projectileObj.transform.position);

            // IMPORTANT: get network to recognize object
            projectileObj.GetComponent<NetworkObject>().Spawn(true);
            projectileObj.transform.GetComponent<NetworkObject>().Spawn(true);


        }
    }

    [ServerRpc]
    public void UpdateClientPositionServerRpc(float forwardBackward, float leftRight)
    {
        forwardBackPosition.Value = forwardBackward;
        leftRightPosition.Value = leftRight;
    }

    // Pick up item and add to player inventory
    private void OnTriggerEnter2D(Collider2D collision)
    {
#if Debug
        Debug.Log("Player collision detected");
        Debug.Log("Item found: " + collision.gameObject.GetComponent<ItemBehavior>().GetItemType().GetName());
#endif
        if(collision.CompareTag("Item"))
        {
            bool delete = playerInventory.AddItem(collision.gameObject.GetComponent<ItemBehavior>(), collision.gameObject.GetComponent<ItemBehavior>().GetItemType());
            if(delete)
            {
                collision.gameObject.GetComponent<ItemBehavior>().Delete();
            }
        }
    }
}