/******************************************************************************
 * Player behavior script, handles movement and collision with items.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DapperDino.Events.CustomEvents;
using System;
using System.Collections;
using System.Collections.Generic;


public class Player2Behavior : NetworkBehaviour
{

	private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
	//player stats
	#region player stats
	[SerializeField]
	private float walkSpeed = 0.02f;
	[SerializeField]
	private float playerHealth;

	private float maxHealth;

	private float playerDefense;
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

	private NetworkVariable<bool> flippedX = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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
	private GameObject EquipUI = null;

	[SerializeField]
	private GameObject RecipeUI = null;

	[SerializeField]
	private DialogueManager dialogueManager = null;

	[SerializeField]
	private RecipeUIBehavior recipeUImanager = null;

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
	private bool codeEnabled = false;
	private bool dialogueEnabled = false;

	[SerializeField]
	private bool resistColdEnabled = false;
	private bool potResistCold = false;

	// Escape menu access
	private EscapeMenu escMenu = null;
	
	// Code menu access
	private HostCode codeMenu = null;

	// Death menu access
	//private DeathMenu deathMenu = null;

	// Respawn UI
	[SerializeField]
	private GameObject RespawnUI = null;

	// Respawn timer
	[SerializeField]
	private Text Timer;

	public float TimeLeft = 4.0f;

	private bool isDead = false;

	// for potion durations - methods will set this as needed
	private float tempPotDuration;

	private bool playerInSnow = false;

	// Commented out because not used
	//private bool isMoving = true;

	private Vector3 originalInvPos = new Vector3(0, 0, 0);

	// client caches positions
	private float oldForwardBackwardPosition = 0;
	private float oldLeftRightPosition = 0;

	//private GameObject craftingObject = null;

	// for flipping sprite
	private SpriteRenderer spriteRenderer;

	// for hiding weapons/tools
	private SpriteRenderer tempSprite;

	// Track hotbar selection
	private int currHotbarSelected = 1;

	// for telling the escape menu that other menus are open
	[SerializeField] private VoidEvent onMenuOpenUpdated = null;
	public Action OnMenuOpenUpdated = delegate { };


	void Start()
	{

		// transform.position = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 0,
		//       Random.Range(defaultPositionRange.x, defaultPositionRange.y));
		transform.position = new Vector3(0, 0, 0);
		playerInventory = gameObject.GetComponent<PlayerInventory>();
		InventoryUI.transform.localScale = new Vector3(0, 0, 0);
		CraftingUI.transform.localScale = new Vector3(0, 0, 0);
		HotbarUI.transform.localScale = new Vector3(0, 0, 0);
		HealthUI.transform.localScale = new Vector3(0, 0, 0);
		EquipUI.transform.localScale = new Vector3(0, 0, 0);
		RecipeUI.transform.localScale = new Vector3(0, 0, 0);
		transform.localScale = new Vector3(1, 1, 1);
		

		// for esc menu to know when to open and when not to
		OnMenuOpenUpdated += onMenuOpenUpdated.Raise;

		// grab original position of inventory for resetting
		originalInvPos = InventoryUI.transform.position;

		//craftingObject = GameObject.Find("CraftingTable");
		escMenu = GameObject.Find("MenuCanvas").GetComponent<EscapeMenu>();
		codeMenu = GameObject.Find("MenuCanvas").GetComponent<HostCode>();
		//deathMenu = GameObject.Find("MenuCanvas").GetComponent<DeathMenu>();
		//MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
		if (IsClient && IsOwner)
		{
			HotbarUI.transform.localScale = new Vector3(1, 1, 1);
			HealthUI.transform.localScale = new Vector3(1, 1, 1);
		}
		healthBar.SetHealth(playerHealth);
		maxHealth = playerHealth;

	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		flippedX.Value = false;
		flippedX.OnValueChanged += Flip;

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
		if (menuOpen)
		{
			//Debug.Log("Stop moving");
			UpdateClientPositionServerRpc(0, 0);
			animator.SetFloat("speed", 0);
		}
		// don't allow player input if the escape menu is open
		if (!escEnabled && !dialogueEnabled)
		{
			if (!isDead)
			{
				// Resetting rsepawn conditions
				RespawnUI.SetActive(false);
				TimeLeft = 4.0f;
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
					//spriteRenderer.flipX = false;
					flippedX.Value = false;
				}
				if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
				{
					leftRight += walkSpeed;
					animator.SetFloat("speed", walkSpeed);
					//spriteRenderer.flipX = true;
					flippedX.Value = true;
				}
				if (forwardBackward == 0 && leftRight == 0)
				{
					animator.SetFloat("speed", 0);
				}

				if (oldForwardBackwardPosition != forwardBackward ||
					oldLeftRightPosition != leftRight)
				{
					//Debug.Log("forwardBackward post change: " + forwardBackward);
					//Debug.Log("leftRight post change: " + leftRight);
					forwardBackward = forwardBackward * Time.fixedDeltaTime;
					leftRight = leftRight * Time.fixedDeltaTime;
					//Debug.Log("forwardBackward post multiply: " + forwardBackward);
					//Debug.Log("leftRight post multiply: " + leftRight);
					UpdateClientPositionServerRpc(forwardBackward, leftRight);
					oldForwardBackwardPosition = forwardBackward; // * Time.deltaTime;
					oldLeftRightPosition = leftRight; //* Time.deltaTime;
					
					//animator.SetFloat("speed", walkSpeed);
				}
				#endregion

				#region INVENTORY/CRAFTING/INTERACTS
				// access inventory
				#region Inventory access w/ E key
				if (Input.GetKeyDown(KeyCode.E))
				{
					if (inventoryEnabled)
					{
						// close
						InventoryUI.transform.localScale = new Vector3(0, 0, 0);
						EquipUI.transform.localScale = new Vector3(0, 0, 0);
						InventoryUI.transform.position = originalInvPos;
						HotbarUI.transform.localScale = new Vector3(1, 1, 1);
						CraftingUI.transform.localScale = new Vector3(0, 0, 0);
						RecipeUI.transform.localScale = new Vector3(0, 0, 0);
						inventoryEnabled = false;
						craftingEnabled = false;
						menuOpen = false;
						playerInventory.inventoryTransferEnabled(false, false);
					}
					else
					{
						InventoryUI.transform.localScale = new Vector3(1, 1, 1);
						EquipUI.transform.localScale = new Vector3(1, 1, 1);
						HotbarUI.transform.localScale = new Vector3(0, 0, 0);
						HotbarUI.transform.localScale = new Vector3(0, 0, 0);
						RecipeUI.transform.localScale = new Vector3(0, 0, 0);
						inventoryEnabled = true;
						menuOpen = true;
						playerInventory.inventoryTransferEnabled(true, false);
					}
					OnMenuOpenUpdated.Invoke();
				}
				#endregion
				// right click
				#region Right click/Interacts
				if (Input.GetMouseButtonDown(1))
				{
					if(!menuOpen)
					{
						RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
						if (hit.collider != null)
						{
							#region Right clicking objects
							// right click crafting object?
							if (hit.collider.tag == "CraftingObject")
							{
								if (Vector3.Distance(hit.collider.gameObject.transform.position, transform.position) <= 3)
								{
									Debug.Log("Right clicked crafting table, opening menu");
									// adjust position and size of inventory
									InventoryUI.transform.localScale = new Vector3(.8f, .8f, .8f);
									InventoryUI.transform.position = new Vector3(InventoryUI.transform.position.x - 450, InventoryUI.transform.position.y, InventoryUI.transform.position.z);
									// open crafting
									CraftingUI.transform.localScale = new Vector3(1, 1, 1);
									// close hotbar and equip 
									HotbarUI.transform.localScale = new Vector3(0, 0, 0);
									EquipUI.transform.localScale = new Vector3(0, 0, 0);
									inventoryEnabled = true;
									craftingEnabled = true;
									menuOpen = true;
									playerInventory.inventoryTransferEnabled(true, true);
									OnMenuOpenUpdated.Invoke();
									return;
								}
								else
								{
									Debug.Log("too far from crafting table");
								}

							}
							else if(hit.collider.tag == "NPC")
							{
								if (Vector3.Distance(hit.collider.gameObject.transform.position, transform.position) <= 3)
								{
									EnableDialogue(true);
									InventoryUI.transform.localScale = new Vector3(0, 0, 0);
									InventoryUI.transform.position = originalInvPos;
									CraftingUI.transform.localScale = new Vector3(0, 0, 0);
									EquipUI.transform.localScale = new Vector3(0, 0, 0);
									RecipeUI.transform.localScale = new Vector3(0, 0, 0);
									HotbarUI.transform.localScale = new Vector3(0, 0, 0);
									hit.collider.gameObject.GetComponent<NPCBehavior>().Interact(GetComponent<Player2Behavior>(), dialogueManager);
									menuOpen = true;
									OnMenuOpenUpdated.Invoke();
									return;
								}
								else
								{
									Debug.Log("too far from npc");
								}

							}
							// right clicking bush?
							else if (hit.collider.tag == "Bush")
							{
								if (Vector3.Distance(hit.collider.gameObject.transform.position, transform.position) <= 3)
								{
									hit.collider.gameObject.GetComponent<BushBehavior>().HarvestBerries();
									return;
								}
								else
								{
									Debug.Log("too far from bush");
								}
							}
							// right clicking pond?
							else if (hit.collider.tag == "Pond")
							{
	
								if (Vector3.Distance(hit.collider.gameObject.transform.position, transform.position) <= 3)
								{
									hit.collider.gameObject.GetComponent<PondBehavior>().Fish(GetComponent<Player2Behavior>());
									return;
								}
								else
								{
									Debug.Log("too far from pond");
								}
							}
							// right clicking mushroom?
							//else if (hit.collider.tag == "Mushroom")
							//{
							//	hit.collider.gameObject.GetComponent<MushroomBehavior>().HarvestSpores();
							//}
							#endregion
						}
						playerInventory.useHotbarItem(currHotbarSelected, GetComponent<Player2Behavior>(), "right");
					}
					
				}
				#endregion
				#region Left click
				if (Input.GetMouseButton(0))
				{
					if (!menuOpen)
					{
						//Debug.Log("DOING SMTHN?");
						playerInventory.useHotbarItem(currHotbarSelected, GetComponent<Player2Behavior>(), "left");
					}
				}
				#endregion
			} else { // while dead
				TimeLeft -= Time.deltaTime;
				updateTime(TimeLeft);
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
					//Debug.Log(currHotbarSelected);
					playerInventory.updateHotbar(currHotbarSelected);
				}
			}
			#endregion
			// close ui menus if they're open
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if(escEnabled)
				{
					//close escape menu
					Debug.Log(">:(");
					EnableEscMenuPlayer();
				}
				else if (inventoryEnabled || craftingEnabled || menuOpen)
				{
					// close every other menu
					InventoryUI.transform.localScale = new Vector3(0, 0, 0);
					InventoryUI.transform.position = originalInvPos;
					CraftingUI.transform.localScale = new Vector3(0, 0, 0);
					EquipUI.transform.localScale = new Vector3(0, 0, 0);
					RecipeUI.transform.localScale = new Vector3(0, 0, 0);
					inventoryEnabled = false;
					craftingEnabled = false;
					HotbarUI.transform.localScale = new Vector3(1, 1, 1);
					menuOpen = false;
					playerInventory.inventoryTransferEnabled(false, false);
					Debug.Log("CLOSE RECIPE");
					OnMenuOpenUpdated.Invoke();
				}
				else if (!escEnabled)
				{
					// open escape menu
					Debug.Log(":(");
					EnableEscMenuPlayer();
				}
			} 

			#endregion
			#endregion

			// test dmg
			//if (Input.GetKeyDown(KeyCode.Space))
			//{
			//	DamagePlayer();
			//}

			// shoot projectile

			if (Input.GetMouseButtonDown(0) && !inventoryEnabled)
			{
				Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				string tag = playerInventory.fetchHotbarItem(currHotbarSelected);

				//ProjectileServerRpc(mousePos);

				// set orientation

			}
		}
	}

	public void EnableEscMenuPlayer()
	{
		escEnabled = !escEnabled;
		Debug.Log("Esc pressed " + escEnabled);
		codeEnabled = !codeEnabled;

	}

	public void EnableDialogue(bool enable)
	{
		dialogueEnabled = enable;
		menuOpen = enable;
		if(!enable)
		{
			HotbarUI.transform.localScale = new Vector3(1, 1, 1);
		}
	}

	public float GetTransformX()
	{
		return transform.position.x;
	}

	public float GetTransformY()
	{
		return transform.position.y;
	}

	#region update player status
	public void HealPlayer()
	{
		playerHealth++;
		healthBar.Heal();
	}

	public void HealPlayer(float healAmount)
	{
		var newHealth = playerHealth + healAmount;
		if(newHealth >= maxHealth)
		{
			playerHealth = maxHealth;
		}
		else
		{
			playerHealth = newHealth;
		}
		healthBar.Heal(healAmount);
	}

	public void DamagePlayer()
	{
		playerHealth--; // For testing
		healthBar.Damage();
		if (playerHealth <= 0)
		{
			Deactivate();
			StartCoroutine("Respawn");
		}
	}

	public void DamagePlayer(float damageAmount)
	{
		//playerHealth -= damageAmount; // For testing
		// apply defense
		// round up 
		float reducedDef = Mathf.Ceil(playerDefense / 2);
		float reducedDamage = damageAmount - reducedDef;
		if (reducedDamage < 1)
		{
			// player always take one damage no matter how much armor they have
			reducedDamage = 1;
		}
		playerHealth -= reducedDamage;
		healthBar.Damage(reducedDamage);
		animator.Play("Player_Hurt");
		if (playerHealth <= 0)
		{
			Deactivate();
			//RespawnUI.SetActive(true);
			StartCoroutine("Respawn");
		}
	}

	public void UpdateDefense(float newDef)
	{
		playerDefense = newDef;
	}

	public void ResistCold(bool enable)
	{
		// don't bother with updating resist cold if a pot is in progress
		if(potResistCold)
		{
			return;
		}
		resistColdEnabled = enable;
		CheckInSnow();
	}

	public void ResistColdDuration(float duration)
	{
		ResistCold(true);
		tempPotDuration = duration;
		potResistCold = true;
		StartCoroutine("StartResistTimer");
		Debug.Log("Drank a cold potion");
		CheckInSnow();
	}

	public IEnumerator StartResistTimer()
	{
		yield return new WaitForSeconds(tempPotDuration);
		potResistCold = false;
		Debug.Log("Cold potion depleted");
		ResistCold(false);
		CheckInSnow();
	}

	// check if in snow and update based on that
	// in case of putting on armor mid biome or taking off
	public void CheckInSnow()
	{
		if(playerInSnow && !resistColdEnabled)
		{
			EnterSnow();
		}
		else if(playerInSnow && resistColdEnabled)
		{
			walkSpeed = 0.02f;
		}
	}

	#endregion

	public void OpenRecipe(List<string> recipeText)
	{
		Debug.Log("test");
		recipeUImanager.DisplayRecipe(recipeText);
		RecipeUI.transform.localScale = new Vector3(1, 1, 1);
		menuOpen = true;
		OnMenuOpenUpdated.Invoke();
	}

	// wrapper method to ask inventory if they have the item that is being looked for
	public bool CheckPlayerInventoryForItem(string itemName, int cost)
	{
		return playerInventory.CheckForItems(itemName, cost);
	}

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
		bool hideWeapon = false;
		if(IsOwner && IsClient)
		{
			if (collision.CompareTag("Item"))
			{

				bool delete = playerInventory.AddItem(collision.gameObject.GetComponent<ItemBehavior>(), collision.gameObject.GetComponent<ItemBehavior>().GetItemType());
				if (delete)
				{
					collision.gameObject.GetComponent<ItemBehavior>().Delete();
				}
			}

			if (collision.CompareTag("Weapon") || collision.CompareTag("Tool"))
			{
				if (collision.gameObject.GetComponent<WeaponBehavior>() != null)
				{
					if (!collision.gameObject.GetComponent<WeaponBehavior>().isHeld())
					{
						bool delete = playerInventory.AddItem(collision.gameObject.GetComponent<ItemBehavior>(), collision.gameObject.GetComponent<ItemBehavior>().GetItemType());
						if (delete)
						{
							collision.gameObject.GetComponent<WeaponBehavior>().Hide();
							hideWeapon = true;
							tempSprite = collision.gameObject.GetComponent<SpriteRenderer>();
						}
					}
				}
			}

			if (collision.CompareTag("Enemy"))
			{
				if (collision.gameObject.GetComponent<EnemyBehavior>() != null)
				{
					// either enemy or
					DamagePlayer(collision.gameObject.GetComponent<EnemyBehavior>().getStrength());
				}
				else
				{
					// projectile
					DamagePlayer(collision.gameObject.GetComponent<ProjectileBehavior>().getStrength());
				}
			}
			
			// if (collision.CompareTag("Overworld"))
			// {

			// }

			if (collision.CompareTag("Snow"))
			{
				EnterSnow();
			}
		}
		if (IsServer)
		{
			if (hideWeapon)
			{
				//collision.gameObject.GetComponent<WeaponBehavior>().Hide();\
				
				HideObjectClientRpc();
			}
		}

	}

	[ClientRpc]
	public void HideObjectClientRpc()
	{
		tempSprite.enabled = false;
	}

	// private void OnTriggerStay2D(Collider2D collider) 
	// {
	// 	if (collider.CompareTag("Snow")) {
	// 		StartCoroutine("SnowDOT");
	// 	}
	// }

	private void OnTriggerExit2D(Collider2D collider) {
		if (collider.CompareTag("Snow")) {
			ExitSnow();
		}
	}

	public void EnterSnow()
	{
		playerInSnow = true;
		if (!resistColdEnabled)
		{
			Debug.Log("SNOW SLOW");
			//walkSpeed = 0.6f;
			StartCoroutine("SnowDOT");
		}
		else
		{
			walkSpeed = 1f; 
		}
	}

	public void ExitSnow()
	{
		playerInSnow = false;
		//walkSpeed = 1f; // Speed returns back to normal upon exiting snow
		StopCoroutine("SnowDOT");
	}


	public void Flip(bool oldValue, bool newValue)
	{
		if (spriteRenderer.flipX != newValue)
		{
			spriteRenderer.flipX = newValue;
		}
	}

	// Respawns the player after 3 seconds
	/*
    Want to add in item drops from the player here 
    Need to test if this works in multiplayer
    */
	IEnumerator Respawn()
	{
		yield return new WaitForSeconds(3.0f);
		transform.position = new Vector3(0, 0, 0);
		isDead = false;
		menuOpen = false;
		craftingEnabled = false;
		inventoryEnabled = false;
		InventoryUI.SetActive(true);
		CraftingUI.SetActive(true);
		HotbarUI.SetActive(true);
		HotbarUI.transform.localScale = new Vector3(1, 1, 1);
		HealthUI.SetActive(true);
		EquipUI.SetActive(true);
		RecipeUI.SetActive(true);
		spriteRenderer.enabled = true;
		playerHealth = maxHealth;
		healthBar.SetHealth(playerHealth);
		StopCoroutine("Respawn");
	}

	IEnumerator SnowDOT()
	{
		yield return new WaitForSeconds(5.0f);
		if(!resistColdEnabled)
		{
			DamagePlayer(1);
			animator.Play("Player_Cold");
			StartCoroutine("SnowDOT");
		}
		else
		{
			Debug.Log("Yay snow resistance");
		}
	}

	// TESTING: Deactivates set of the player's UI including inventory, crafting, etc.
	public void Deactivate()
	{

		//playerInventory = gameObject.GetComponent<PlayerInventory>();
		InventoryUI.SetActive(false);
		CraftingUI.SetActive(false);
		HotbarUI.SetActive(false);
		HealthUI.SetActive(false);
		EquipUI.SetActive(false);
		RecipeUI.SetActive(false);
		// make sure everything is not showing 
		InventoryUI.transform.localScale = new Vector3(0, 0, 0);
		InventoryUI.transform.position = originalInvPos;
		CraftingUI.transform.localScale = new Vector3(0, 0, 0);
		EquipUI.transform.localScale = new Vector3(0, 0, 0);
		RecipeUI.transform.localScale = new Vector3(0, 0, 0);

		spriteRenderer.enabled = false;
		isDead = true;

		// // for esc menu to know when to open and when not to
		OnMenuOpenUpdated += onMenuOpenUpdated.Raise;

		// // grab original position of inventory for resetting
		// originalInvPos = InventoryUI.transform.position;
	}

	public void updateTime(float time) {
		float seconds = Mathf.FloorToInt(time % 60);
		Timer.text = seconds.ToString();
	}
}