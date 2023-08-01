using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.LowLevel;

public class WeaponBehavior : ItemBehavior
{
    // control for enabling/disabling weapon/item behavior
    private bool held = false;
    private float startTime = 0;
    private float endTime = 0;

    // new version of prefab to use for weapon attack
    private GameObject weaponActive;

    // place the same weapon prefab here
    [SerializeField] private GameObject prefab;

    // two coordinates to control how far weapon starts from player
    [SerializeField] private float startX;
    [SerializeField] private float startY;
    
    // stat fields
    [SerializeField] private int strength;
    [SerializeField] private float knockback;
    [SerializeField] private float reload;

    [SerializeField] private string weaponType;

    private bool swing = true;

    // getters and setters
    public int getStrength()
    {
        return strength;
    }

    public float getKnockback()
    { 
        return knockback; 
    }

    public float getReload() 
    { 
        return reload; 
    }

    public bool isHeld()
    {
        return held;
    }

    public void setHeld(bool x)
    {
        held = x;
    }

    public GameObject getWeaponActive()
    {
        return weaponActive;
    }

    // to attack, the code makes a NEW clone of the weapon prefab to manipulate it separately
    public override string GetItemEffect(Player2Behavior playerBehavior)
	{
        //startTime = Time.time;
        //Debug.Log("startTime: " + startTime + " endTime: " + endTime);
        if (startTime >= endTime)
        {
            Debug.Log("Attempting to swing");
            endTime = startTime + reload;
            //swing = false;
            //StartCoroutine("StartSwingTimer");
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mousePos.x - playerBehavior.transform.position.x < 0)
            {
                //Debug.Log("Ping! " + (mousePos.x - playerBehavior.transform.position.x));
                playerBehavior.Flip(true, false);
            }
            else if (mousePos.x - playerBehavior.transform.position.x > 0)
            {
                //Debug.Log("Pong! " + (mousePos.x - playerBehavior.transform.position.x));
                playerBehavior.Flip(false, true);
            }
            //playerBehavior.CallWeaponSwingPlease(GetComponent<WeaponBehavior>());
            WeaponSwingServerRpc();
        }
        return "Weapon";
    }

 //   IEnumerator StartSwingTimer()
	//{
 //       yield return new WaitForSeconds(reload);
 //       swing = true;
 //   }

	[ServerRpc(RequireOwnership = false)]
	public void WeaponSwingServerRpc(ServerRpcParams serverRpcParams = default)
	{
		Debug.Log("Swing");
		var clientId = serverRpcParams.Receive.SenderClientId;
		// check if clientid is connected
		if (NetworkManager.ConnectedClients.ContainsKey(clientId))
		{
			var client = NetworkManager.ConnectedClients[clientId];

			weaponActive = Instantiate(Resources.Load("Prefabs/Items/" + itemName), client.PlayerObject.transform.position, Quaternion.identity) as GameObject;
            // new Vector2(client.PlayerObject.transform.position.x + startX + 0.25f, client.PlayerObject.transform.position.y + startY)
            weaponActive.GetComponent<NetworkObject>().Spawn(true);
			weaponActive.gameObject.GetComponent<Renderer>().enabled = true;
            weaponActive.gameObject.GetComponent<Collider2D>().enabled = true;
            var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(weaponActive.transform.position);
			var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			weaponActive.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			weaponActive.transform.Translate(startX + (float)0.25, startY, 0);
			weaponActive.transform.parent = client.PlayerObject.transform;
            
            if (weaponActive.GetComponent<WeaponBehavior>() != null)
			{
				weaponActive.GetComponent<WeaponBehavior>().setHeld(true);
			}
		}

	}

	//[ServerRpc]
	//public void WeaponSwingServerRpc()
	//{
	//    Debug.Log("Swing");

	//    // check if clientid is connected
	//    if (NetworkManager.ConnectedClients.ContainsKey(OwnerClientId))
	//    {
	//        var client = NetworkManager.ConnectedClients[OwnerClientId];

	//        weaponActive = Instantiate(prefab, client.PlayerObject.transform.position, Quaternion.identity);
	//        weaponActive.GetComponent<NetworkObject>().Spawn(true);
	//        weaponActive.gameObject.GetComponent<Renderer>().enabled = true;
	//        weaponActive.gameObject.GetComponent<Collider2D>().enabled = true;
	//        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(weaponActive.transform.position);
	//        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
	//        weaponActive.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	//        weaponActive.transform.Translate(startX + (float)0.25, startY, 0);
	//        weaponActive.transform.parent = client.PlayerObject.transform;

	//        if (weaponActive.GetComponent<WeaponBehavior>() != null)
	//        {
	//            weaponActive.GetComponent<WeaponBehavior>().setHeld(true);
	//        }
	//    }
	//}

	protected override void Awake()
    {
        base.Awake();
        startTime = 0;
        endTime = 0;
        leftOrRight = "left";
    }


    protected override void Update()
    {
        base.Update();
        startTime = Time.time;
    }

    public string GetWeaponType()
	{
        return weaponType;
	}

}
