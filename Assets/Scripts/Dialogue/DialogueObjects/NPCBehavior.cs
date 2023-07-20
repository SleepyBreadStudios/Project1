using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    #region Dialogue
    /// <summary>
    /// Different Shop Item dialogues
    /// </summary>
    public Dialogue dialogueText;

    public DialogueManager dialogueBox = default;

    private bool currentlyInDialogue = false;
    private bool currentlyInOptionSelection = false;
    #endregion

    /// <summary>
    /// Range for interact pop up
    /// </summary>
    [SerializeField]
    private float kInteractRange = 2.4f;

    private Player2Behavior currPlayer = default;

    void Awake()
    {
        //dialogueBox = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
    }

    //// Update is called once per frame
    //void Update()
    //{
        
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
            //// if player can afford item, purchase it
            //if (currentPositionScript.GetPrice() <= ScoreController.totalScore)
            //{
            //    ScoreController.AddScore(-currentPositionScript.GetPrice());
            //    playerInventory.PurchaseItem(currentPositionScript.shopItem, currentPositionScript.GetObjectType());
            //    currentPositionScript.PurchaseItem();
            //    Debug.Log(ScoreController.totalScore);
            //}
            //// else show dialogue saying that they can't afford it
            //else
            //{
            //    currentlyInOptionSelection = true;
            //    StartCoroutine(WaitForOptionSelected());
            //}
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
