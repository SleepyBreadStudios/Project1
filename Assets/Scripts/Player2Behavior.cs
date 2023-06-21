/******************************************************************************
 * Player behavior script, handles movement and collision with items.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
using Unity.Netcode;
using UnityEngine;


public class Player2Behavior : NetworkBehaviour
{

    //[SerializeField]
    //private Camera MainCamera = null;

    public Animator animator;

    [SerializeField]
    private float walkSpeed = 0.2f;

    [SerializeField]
    private Vector2 defaultPositionRange = new Vector2(-4, 4);

    [SerializeField]
    private NetworkVariable<float> forwardBackPosition = new NetworkVariable<float>();

    [SerializeField]
    private NetworkVariable<float> leftRightPosition = new NetworkVariable<float>();

    [SerializeField]
    private NetworkVariable<float> scaleXPosition = new NetworkVariable<float>();

    PlayerInventory playerInventory = null;

    [SerializeField]
    private GameObject InventoryUI = null;

    [SerializeField]
    private GameObject CraftingUI = null;

    // is inventory showing at the moment?
    private bool inventoryEnabled = false;
    private bool craftingEnabled = false;

    private Vector3 originalInvPos = new Vector3(0,0,0);

    // client caches positions
    private float oldForwardBackwardPosition = 0;
    private float oldLeftRightPosition = 0;
    private float oldScaleX = 0;

    private GameObject craftingObject = null;

    void Start()
    {
        //transform.position = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 0,
        //       Random.Range(defaultPositionRange.x, defaultPositionRange.y));
        transform.position = new Vector3(0, 0, 0);
        playerInventory = gameObject.GetComponent<PlayerInventory>();
        InventoryUI.transform.localScale = new Vector3(0, 0, 0);
        CraftingUI.transform.localScale = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(1, 1, 1);
        scaleXPosition.Value = 1;

        // grab original position of inventory for resetting
        originalInvPos = InventoryUI.transform.position;

        // Sets random color when players spawn
        float rand = Random.Range(0, 256);
        float rand2 = Random.Range(0, 256);
        float rand3 = Random.Range(0, 256);
        rand = rand / 255.0f;
        rand2 = rand2 / 255.0f;
        rand3 = rand3 / 255.0f;

        gameObject.GetComponent<Renderer>().material.color = new Color(rand, rand2, rand3);
        craftingObject = GameObject.Find("CraftingTable");
        //MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        if (IsServer)
            UpdateServer();

        if (IsClient && IsOwner)
            UpdateClient();
    }

    private void UpdateServer()
    {
        transform.position = new Vector3(transform.position.x + leftRightPosition.Value,
            transform.position.y + forwardBackPosition.Value, transform.position.z);
        transform.localScale = new Vector3(scaleXPosition.Value, transform.localScale.y, transform.localScale.z);
    }

    private void UpdateClient()
    {
        float forwardBackward = 0;
        float leftRight = 0;
        float scaleX = 0;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            forwardBackward += walkSpeed;
            animator.SetFloat("speed", walkSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            forwardBackward -= walkSpeed;
            animator.SetFloat("speed", walkSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            leftRight -= walkSpeed;
            animator.SetFloat("speed", walkSpeed);
            scaleX = 1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            leftRight += walkSpeed;
            animator.SetFloat("speed", walkSpeed);
            scaleX = -1;
        }
        if(forwardBackward == 0 && leftRight == 0)
        {
            animator.SetFloat("speed", 0);
        }

        if (oldForwardBackwardPosition != forwardBackward ||
            oldLeftRightPosition != leftRight)
        {
            UpdateClientPositionServerRpc(forwardBackward, leftRight, scaleX);
            oldForwardBackwardPosition = forwardBackward;
            oldLeftRightPosition = leftRight;
            oldScaleX = scaleX;
            //animator.SetFloat("speed", walkSpeed);
        }

        // access inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryEnabled)
            {
                InventoryUI.transform.localScale = new Vector3(0, 0, 0);
                InventoryUI.transform.position = originalInvPos; 
                CraftingUI.transform.localScale = new Vector3(0, 0, 0);
                inventoryEnabled = false;
                craftingEnabled = false;
            }
            else
            {
                InventoryUI.transform.localScale = new Vector3(1, 1, 1);
                inventoryEnabled = true;
            }
        }

        // access crafting
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    if (craftingEnabled)
        //    {
        //        CraftingUI.transform.localScale = new Vector3(0, 0, 0);
        //        craftingEnabled = false;
        //    }
        //    else
        //    {
        //        InventoryUI.transform.localScale = new Vector3(1, 1, 1);
        //        CraftingUI.transform.localScale = new Vector3(1, 1, 1);
        //        inventoryEnabled = true;
        //        craftingEnabled = true;
        //    }
        //}

        // close ui menus if they're open
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventoryEnabled || craftingEnabled)
            {
                InventoryUI.transform.localScale = new Vector3(0, 0, 0);
                InventoryUI.transform.position = originalInvPos;
                CraftingUI.transform.localScale = new Vector3(0, 0, 0);
                inventoryEnabled = false;
                craftingEnabled = false;
            }
        }

        // access overworld crafting
        if(Input.GetMouseButtonDown(1))
        {
            if(!craftingEnabled)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.gameObject == craftingObject)
                {
                    if (Vector3.Distance(hit.collider.gameObject.transform.position, transform.position) <= 4)
                    {
                        Debug.Log("Right clicked crafting table, opening menu");
                        // adjust position and size of inventory
                        InventoryUI.transform.localScale = new Vector3(.8f, .8f, .8f);
                        InventoryUI.transform.position = new Vector3(InventoryUI.transform.position.x - 450, InventoryUI.transform.position.y, InventoryUI.transform.position.z);
                        // open crafting
                        CraftingUI.transform.localScale = new Vector3(1, 1, 1);
                        inventoryEnabled = true;
                        craftingEnabled = true;
                    }
                    else
                    {
                        Debug.Log("too far from crafting table");
                    }
                }
            }
            else
            {
                Debug.Log("Crafting table already open");
            }
        }
    }

    [ServerRpc]
    public void UpdateClientPositionServerRpc(float forwardBackward, float leftRight, float scaleX)
    {
        forwardBackPosition.Value = forwardBackward;
        leftRightPosition.Value = leftRight;
        if(scaleX != 0)
        {
            scaleXPosition.Value = scaleX;
        }
        
    }

    // Pick up item and add to player inventory
    private void OnTriggerEnter2D(Collider2D collision)
    {
#if Debug
        Debug.Log("Player collision detected");
        Debug.Log("Item found: " + collision.gameObject.GetComponent<ItemBehavior>().GetItemType().GetName());
#endif
        if (collision.CompareTag("Item"))
        {
            bool delete = playerInventory.AddItem(collision.gameObject.GetComponent<ItemBehavior>(), collision.gameObject.GetComponent<ItemBehavior>().GetItemType());
            if (delete)
            {
                collision.gameObject.GetComponent<ItemBehavior>().Delete();
            }
        }
    }
}