/******************************************************************************
 * Player behavior script, handles movement and collision with items.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using DapperDino.Events.CustomEvents;
using System;

public class Player2Behavior : NetworkBehaviour
{

    //player stats
    #region
    [SerializeField]
    private float walkSpeed = 0.2f;
    [SerializeField]
    private float playerHealth = 5;
    #endregion

    [SerializeField]
    private HealthBar healthBar = null;

    public Animator animator;

    [SerializeField]
    private Vector2 defaultPositionRange = new Vector2(-4, 4);

    [SerializeField]
    private NetworkVariable<float> forwardBackPosition = new NetworkVariable<float>();

    [SerializeField]
    private NetworkVariable<float> leftRightPosition = new NetworkVariable<float>();

    PlayerInventory playerInventory = null;

    [SerializeField]
    private GameObject InventoryUI = null;

    [SerializeField]
    private GameObject CraftingUI = null;

    [SerializeField]
    private GameObject HotbarUI = null;

    [SerializeField]
    private GameObject HealthUI = null;

    [SerializeField]
    private GameObject projectile = null;

    public GameObject projectileObj = null;

    [SerializeField]
    Transform m_CameraFollow;

    // is inventory showing at the moment?
    private bool inventoryEnabled = false;
    private bool craftingEnabled = false;
    private bool menuOpen = false;
    private bool escEnabled = false;

    private EscapeMenu escMenu = null;

    private Vector3 originalInvPos = new Vector3(0,0,0);

    // client caches positions
    private float oldForwardBackwardPosition = 0;
    private float oldLeftRightPosition = 0;

    private GameObject craftingObject = null;

    // for flipping sprite
    private SpriteRenderer spriteRenderer;

    // Track hotbar selection
    private int currHotbarSelected = 1;

    // for telling the escape menu that other menus are open
    [SerializeField] private VoidEvent onMenuOpenUpdated = null;
    public Action OnMenuOpenUpdated = delegate { }; 

    void Start()
    {
        //transform.position = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 0,
        //       Random.Range(defaultPositionRange.x, defaultPositionRange.y));
        transform.position = new Vector3(0, 0, 0);
        playerInventory = gameObject.GetComponent<PlayerInventory>();
        InventoryUI.transform.localScale = new Vector3(0, 0, 0);
        CraftingUI.transform.localScale = new Vector3(0, 0, 0);
        HotbarUI.transform.localScale = new Vector3(0, 0, 0);
        HealthUI.transform.localScale = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(1, 1, 1);
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        // for esc menu to know when to open and when not to
        OnMenuOpenUpdated += onMenuOpenUpdated.Raise;

        // grab original position of inventory for resetting
        originalInvPos = InventoryUI.transform.position;

        // Sets random color when players spawn
        // float rand = UnityEngine.Random.Range(0, 256);
        // float rand2 = UnityEngine.Random.Range(0, 256);
        // float rand3 = UnityEngine.Random.Range(0, 256);
        // rand = rand / 255.0f;
        // rand2 = rand2 / 255.0f;
        // rand3 = rand3 / 255.0f;

        // gameObject.GetComponent<Renderer>().material.color = new Color(rand, rand2, rand3);
        craftingObject = GameObject.Find("CraftingTable");
        escMenu = GameObject.Find("MenuCanvas").GetComponent<EscapeMenu>();
        //MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (IsClient && IsOwner)
        {
            HotbarUI.transform.localScale = new Vector3(1, 1, 1);
            HealthUI.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient && IsOwner)
        {
            var cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = transform;
        }

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
        //transform.localScale = new Vector3(scaleXPosition.Value, transform.localScale.y, transform.localScale.z);
    }

    private void UpdateClient()
    {
        float forwardBackward = 0;
        float leftRight = 0;

        #region KEY PRESSES
        #region MOVEMENT
        // don't allow player input if the escape menu is open
        if (!escEnabled)
        {
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
                spriteRenderer.flipX = false;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                leftRight += walkSpeed;
                animator.SetFloat("speed", walkSpeed);
                spriteRenderer.flipX = true;
            }
            if (forwardBackward == 0 && leftRight == 0)
            {
                animator.SetFloat("speed", 0);
            }

            if (oldForwardBackwardPosition != forwardBackward ||
                oldLeftRightPosition != leftRight)
            {
                UpdateClientPositionServerRpc(forwardBackward, leftRight);
                oldForwardBackwardPosition = forwardBackward;
                oldLeftRightPosition = leftRight;
                //animator.SetFloat("speed", walkSpeed);
            }
            #endregion
            #region INVENTORY/CRAFTING
            // access inventory
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (inventoryEnabled)
                {
                    // close
                    InventoryUI.transform.localScale = new Vector3(0, 0, 0);
                    InventoryUI.transform.position = originalInvPos;
                    HotbarUI.transform.localScale = new Vector3(1, 1, 1);
                    CraftingUI.transform.localScale = new Vector3(0, 0, 0);
                    inventoryEnabled = false;
                    craftingEnabled = false;
                    menuOpen = false;
                    playerInventory.inventoryTransferEnabled(false);
                }
                else
                {
                    InventoryUI.transform.localScale = new Vector3(1, 1, 1);
                    HotbarUI.transform.localScale = new Vector3(0, 0, 0);
                    inventoryEnabled = true;
                    menuOpen = true;
                }
                OnMenuOpenUpdated.Invoke();
            }

            // access overworld crafting
            if (Input.GetMouseButtonDown(1))
            {
                // check if interacting with craftingtable
                if (!craftingEnabled)
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
                            HotbarUI.transform.localScale = new Vector3(0, 0, 0);
                            inventoryEnabled = true;
                            craftingEnabled = true;
                            menuOpen = true;
                            playerInventory.inventoryTransferEnabled(true);
                            OnMenuOpenUpdated.Invoke();
                        }
                        else
                        {
                            Debug.Log("too far from crafting table");
                        }
                    }
                    // not interacting with crafting table or any menus and rightclicking nothing
                    else if (!menuOpen)
                    {
                        playerInventory.useHotbarItem(currHotbarSelected, GetComponent<Player2Behavior>(), "right");
                    }
                }
                else
                {
                    //Debug.Log("Crafting table already open");
                }
            }
            if(Input.GetMouseButtonDown(0))
            {
                if(!menuOpen)
                {
                    playerInventory.useHotbarItem(currHotbarSelected, GetComponent<Player2Behavior>(), "left");
                }
            }
        }
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
                HotbarUI.transform.localScale = new Vector3(1, 1, 1);
                menuOpen = false;
                playerInventory.inventoryTransferEnabled(false);
                OnMenuOpenUpdated.Invoke();
            }
        }
        #region HOTBAR
        // check for numkeys for hotbar
        // don't interact with hotbar if a menu is open
        if (!menuOpen)
        {
            bool hotbarChanged = false;
            if (Input.mouseScrollDelta.y != 0)
            {
                if (Input.mouseScrollDelta.y > 0)
                {
                    currHotbarSelected--;
                    if (currHotbarSelected < 0)
                    {
                        currHotbarSelected = 9;
                    }
                }
                else
                {
                    currHotbarSelected++;
                    if (currHotbarSelected > 9)
                    {
                        currHotbarSelected = 0;
                    }
                }
                hotbarChanged = true;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currHotbarSelected = 1;
                hotbarChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                currHotbarSelected = 2;
                hotbarChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                currHotbarSelected = 3;
                hotbarChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                currHotbarSelected = 4;
                hotbarChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                currHotbarSelected = 5;
                hotbarChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                currHotbarSelected = 6;
                hotbarChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                currHotbarSelected = 7;
                hotbarChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                currHotbarSelected = 8;
                hotbarChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                currHotbarSelected = 9;
                hotbarChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                currHotbarSelected = 0;
                hotbarChanged = true;
            }
            if (hotbarChanged)
            {
                playerInventory.updateHotbar(currHotbarSelected);
            }
        }



        #endregion
        #endregion
        #endregion

        // test dmg
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DamagePlayer();
        }

                // shoot projectile
        if (Input.GetMouseButtonDown(0) && !inventoryEnabled)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            ProjectileServerRpc(mousePos);

            // set orientation
         
        }
    }

    public void EnableEscMenuPlayer()
    {
        escEnabled = !escEnabled;
        Debug.Log("Esc pressed " + escEnabled);
        menuOpen = escEnabled;
    }

    #region update player status
    public void HealPlayer()
    {
        playerHealth++;
        healthBar.Heal();
    }

    public void DamagePlayer()
    {
        playerHealth--;
        healthBar.Damage();
    }
    #endregion

    [ServerRpc]
    public void UpdateClientPositionServerRpc(float forwardBackward, float leftRight)
    {
        forwardBackPosition.Value = forwardBackward;
        leftRightPosition.Value = leftRight;
        
    }

    [ServerRpc]
    public void ProjectileServerRpc(Vector3 mousePos)
    {
        // instantiate projectile
        projectileObj = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;

        // IMPORTANT: get network to recognize object
        projectileObj.GetComponent<NetworkObject>().Spawn(true);

        projectileObj.transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - projectileObj.transform.position);
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