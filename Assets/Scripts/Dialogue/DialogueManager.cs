/******************************************************************************
* Controller for updating dialogue UI.
* 
* Authors: Alicia T, Jason N, Jino C
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class DialogueManager : NetworkBehaviour
{
    private Queue<string> sentences;

    public TMP_Text text;
    public TMP_Text titleText;

    //public TMP_Text toolTipText;
    public GameObject help;

    #region dialogue box UI
    /// <summary>
    /// Reference to Dialogue box
    /// </summary>
    [SerializeField]
    private GameObject dialogueBox = default;

    /// <summary>
    /// Reference to Dialogue box's option triangle
    /// </summary>
    [SerializeField]
    private GameObject optionUI = default;

    /// <summary>
    /// Reference to Dialogue box's advance triangle
    /// </summary>
    [SerializeField]
    private GameObject advanceUI = default;
    #endregion

    #region tool tip 
    /// <summary>
    /// Reference to Tooltip Box
    /// </summary>
    //[SerializeField]
    //private GameObject toolTip = default;
    #endregion


    private PlayerBehavior playerBehavior;
    //private Animator animator = null;

    private bool dialogueStarted = false;

    /// <summary>
    /// How fast the dialogue goes
    /// </summary>
    [SerializeField]
    private float dialogueSpeed = 0.05f;
    [SerializeField]
    private float originalTextSpeed = 0.05f;

    private bool sentenceInProgress = false;

    #region Option triangle movement UI
    private bool optionsPresented = false;
    private int optionCurrentlySelected = 0;
    private int numberOfOptions = 0;
    private RectTransform optionTransform;
    private Vector2 originalOptionPosition;
    private Vector2 currentOptionPosition;
    #endregion

    // RectTransform for moving the dialogue over
    private RectTransform dialogueTransform;
    private Vector2 originalDialoguePosition;

    private NPCBehavior npc = default;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        // Set up option triangle UI 
        optionTransform = optionUI.GetComponent<RectTransform>();
        originalOptionPosition = optionTransform.anchoredPosition;
        currentOptionPosition = originalOptionPosition;

        dialogueTransform = text.gameObject.GetComponent<RectTransform>();
        originalDialoguePosition = dialogueTransform.anchoredPosition;

        originalTextSpeed = dialogueSpeed;

        sentences = new Queue<string>();
        //playerBehavior = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehavior>();
        //playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
        DisableAdvance();
    }

    void Update()
    {
        if(dialogueStarted)
		{
            CheckInput();
        }
    }

    #region Dialogue 
    /// <summary>
    /// Function for starting dialogue. Displays dialogue box and
    /// the first sentence.
    /// Takes the gameobject that called it and the animator
    /// of the NPC speaking as parameters
    /// TODO optimize this, currently only takes ShopKeepBehavior
    /// Would be ideal if it took any type of script
    /// <param name="dialogue"></param>
    /// <param name="gameObject"></param>
    /// <param name="a"></param>
    public void StartDialogue(Dialogue dialogue, NPCBehavior gameObject) //Dialogue dialogue, ShopBehavior gameObject, Animator a)
    {
        npc = gameObject;
        //animator = a

        GameObject temp;
        temp = dialogueBox.gameObject;
        temp.SetActive(true);

        titleText.text = dialogue.title;
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        StartCoroutine(WaitForDialogueStart());
    }

    IEnumerator WaitForDialogueStart()
	{
        // Wait .001 seconds
        yield return new WaitForSeconds(.001f);
        dialogueStarted = true;
        DisplayNextSentence();
    }

    /// <summary>
    /// Shows the next sentence in the dialogue
    /// </summary>
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        // Displays option if there are options
        if (sentence.Contains("[]"))
        {
            // option will always be signified by a [] at the beginning of the sentence
            // and a number following that signifies how many options there are
            numberOfOptions = sentence[2] - '0';
            sentence = sentence.Remove(0, 3);
            EnableOptions();
        }
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
        sentenceInProgress = true;
    }

    #region Text Animate functions 
    // (Code Credit: https://flaredust.com/game-dev/unity/interesting-dialogue-boxes/)
    /// <summary>
    /// Animates sentence
    /// </summary>
    /// <param name="sentence"></param>
    /// <returns></returns>
    IEnumerator TypeSentence(string sentence)
    {
        // animates shopkeep's mustache
        //animator.SetBool("isTalking", true);
        text.text = sentence;
        text.ForceMeshUpdate();
        TMP_TextInfo textInfo = text.textInfo;

        //Color of all characters' vertices.
        Color32[] newVertexColors;

        //Base color for our text.
        Color32 c0 = text.color;

        // hide text so that we can animate it by making each character visible
        SetInvisible();

        int numberOfLetters = text.textInfo.characterCount;
        int i = 0;
        while (i < numberOfLetters)
        {
            numberOfLetters = textInfo.characterCount; // Update visible character count.
                                                       // Get the index of the material used by the current character.
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            // Get the vertex colors of the mesh used by this text element (character or sprite).
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            // Only change the vertex color if the text element is visible. (It's visible, only the alpha color is 0)
            if (textInfo.characterInfo[i].isVisible)
            {

                newVertexColors[vertexIndex + 0] = c0;
                newVertexColors[vertexIndex + 1] = c0;
                newVertexColors[vertexIndex + 2] = c0;
                newVertexColors[vertexIndex + 3] = c0;

                // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
                text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }

            i++;
            yield return new WaitForSeconds(dialogueSpeed);
        }
        sentenceInProgress = false;
        // stops animating shopkeep's mustache
        //animator.SetBool("isTalking", false);
        dialogueSpeed = originalTextSpeed;
        if (!optionsPresented)
            EnableAdvance();
    }

    /// <summary>
    /// Sets text to invisible
    /// </summary>
    private void SetInvisible()
    {
        TMP_TextInfo textInfo = text.textInfo;

        Color32[] newVertexColors;
        Color32 c0 = text.color;

        for (int i = 0; i < textInfo.characterCount; i++)
        {

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            // Get the vertex colors of the mesh used by this text element (character or sprite).
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            //Alpha = 0
            c0 = new Color32(c0.r, c0.g, c0.b, 0);

            //Apply it to all vertex.
            newVertexColors[vertexIndex + 0] = c0;
            newVertexColors[vertexIndex + 1] = c0;
            newVertexColors[vertexIndex + 2] = c0;
            newVertexColors[vertexIndex + 3] = c0;

            // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }
    #endregion

    /// <summary>
    /// Ends dialogue, turning all of the dialogue box
    /// off and reverting player to normal
    /// </summary>
    void EndDialogue()
    {
        Debug.Log("End of conversation.");
        dialogueStarted = false;
        //playerBehavior.ChangeRegularInput();
        //playerMovement.ChangeRegularInput();

        //disable dialogue box when done
        GameObject temp;
        temp = dialogueBox.gameObject;
        temp.SetActive(false);
        npc.CurrentDialogueEnded();
        DisableAdvance();
    }


    /// <summary>
    /// Turns on option triangle and informs that player is in option selection
    /// </summary>
    private void EnableOptions()
    {
        GameObject temp;
        temp = optionUI.gameObject;
        temp.SetActive(true);
        help.SetActive(true);
        optionsPresented = true;
        optionCurrentlySelected = 1;
        // Move dialogue box to make room for option triangle
        dialogueTransform.anchoredPosition = new Vector2(dialogueTransform.anchoredPosition.x + 5, dialogueTransform.anchoredPosition.y);
    }

    /// <summary>
    /// Turns off option triangle and informs that player is
    /// no longer in option selection
    /// </summary>
    private void DisableOptions()
    {
        GameObject temp;
        temp = optionUI.gameObject;
        temp.SetActive(false);
        help.SetActive(false);
        optionsPresented = false;
        optionCurrentlySelected = 0;
    }

    private void EnableAdvance()
    {
        GameObject temp;
        temp = advanceUI.gameObject;
        temp.SetActive(true);
    }
    private void DisableAdvance()
    {
        GameObject temp;
        temp = advanceUI.gameObject;
        temp.SetActive(false);
    }

    /// <summary>
    /// Function checks if player advanced dialogue
    /// Alternatively handles when a player is in option selection
    /// </summary>
    private void CheckInput()
    {
        // Speeds up sentence animation if key is pressed
        if (sentenceInProgress)
        {
            if (Input.anyKeyDown)
            {
                dialogueSpeed = dialogueSpeed / 100;
            }
        }
        // Displays next sentence when enter, spacebar button, or mouse0 is pressed
        else if (dialogueStarted && !optionsPresented)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.E))
            {
                DisplayNextSentence();
                DisableAdvance();
            }
        }
        else if (optionsPresented)
        {
            // Move option selection triangle up and down accordingly
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                if (optionCurrentlySelected > 1)
                {
                    currentOptionPosition = new Vector2(currentOptionPosition.x, currentOptionPosition.y + 24.5f);
                    optionTransform.anchoredPosition = currentOptionPosition;
                    optionCurrentlySelected--;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                if (optionCurrentlySelected < numberOfOptions)
                {
                    currentOptionPosition = new Vector2(currentOptionPosition.x, currentOptionPosition.y - 24.5f);
                    optionTransform.anchoredPosition = currentOptionPosition;
                    optionCurrentlySelected++;
                }
            }
            // exit options
            else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                npc.OptionSelected(optionCurrentlySelected);
                DisplayNextSentence();
                DisableOptions();
                // reset triangle position for next time
                optionTransform.anchoredPosition = originalOptionPosition;
                currentOptionPosition = originalOptionPosition;
                // reset dialogue position for next sentence
                dialogueTransform.anchoredPosition = originalDialoguePosition;
            }
        }
    }
    #endregion
}

[System.Serializable]
public class Dialogue
{
    public string title;

    [TextArea(3, 10)]
    public string[] sentences;

    public string[] options;
}