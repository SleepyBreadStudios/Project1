using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NPCBehavior : NetworkBehaviour
{
    #region Dialogue
    /// <summary>
    /// Different Shop Item dialogues
    /// </summary>
    public Dialogue dialogueText;

    public Dialogue successfulText;

    public Dialogue unsuccessfulText;

    public DialogueManager dialogueBox = default;

    private bool currentlyInDialogue = false;
    private bool currentlyInOptionSelection = false;
    #endregion

    /// <summary>
    /// Range for interact pop up
    /// </summary>
    [SerializeField]
    private float kInteractRange = 2.4f;

    [SerializeField]
    private int materialCost = 10;

    [SerializeField]
    private string materialName = default;

    [SerializeField]
    protected List<GameObject> allSpawnObjects = new();

    [SerializeField]
    private GameObject itemToSpawn = default;

    private Player2Behavior currPlayer = default;

    private bool spawnObject = false;

    [SerializeField]
    private bool deleteNPCOnSpawn;

    void Awake()
    {
        //dialogueBox = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
    }

	//// Update is called once per frame
	void Update()
	{
        if(spawnObject && !currentlyInDialogue)
		{
            SpawnObjectServerRpc();
            if (deleteNPCOnSpawn)
			{
                Delete();
            }
            spawnObject = false;
        }
	}

    public void Delete()
    {
        DespawnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnServerRpc()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnObjectServerRpc()
    {
        for (int i = 0; i < allSpawnObjects.Count; i++)
		{
            Debug.Log("Spawn objects");
            GameObject newItem = Instantiate(allSpawnObjects[i], new Vector2(transform.position.x + ((i + 1) * 0.5f), transform.position.y + ((i + 1) * 0.5f)), Quaternion.identity);
            newItem.GetComponent<NetworkObject>().Spawn(true);
        }

    }
    // may need to make this script a network behavior
    //public override void OnNetworkDespawn()
    //{
    //    Destroy(gameObject);
    //}

    // called by player to interact with npc to start dialogue
    public void Interact(Player2Behavior player, DialogueManager dialogueManager)
	{
        if(!currentlyInDialogue)
		{
            dialogueBox = dialogueManager;
            currPlayer = player;
            dialogueBox.StartDialogue(dialogueText, GetComponent<NPCBehavior>());
            currentlyInDialogue = true;
		}
	}

    // rework for other npcs
    public void OptionSelected(int optionNumber)
    {
        if (optionNumber == 1)
        {
            if(currPlayer.CheckPlayerInventoryForItem(materialName, materialCost))
			{
                dialogueBox.StartDialogue(successfulText, GetComponent<NPCBehavior>());
                currentlyInDialogue = true;
                spawnObject = true;
            }
            else
			{
                dialogueBox.StartDialogue(unsuccessfulText, GetComponent<NPCBehavior>());
                currentlyInDialogue = true;
            }
        }

    }

    // Makes sure that when enter is pressed it starts the new dialogue and doesn't read the
    // option selected input as advance dialogue input
    // Basically forces player to do 2 inputs
    IEnumerator WaitForOptionSelected()
    {
        // Wait .001 seconds
        yield return new WaitForSeconds(.001f);
        // play dialogue
        //ShopKeepSpeak("Too Poor Dialogue");
        // allow pop up to show up when dialogue ends
        currentlyInOptionSelection = false;
    }

    public void CurrentDialogueEnded()
	{
        currentlyInDialogue = false;
        currPlayer.EnableDialogue(false);
	}
}
