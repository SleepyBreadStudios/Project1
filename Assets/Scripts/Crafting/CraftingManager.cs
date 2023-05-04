using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    [SerializeField]
    private CraftingInventoryManager craftInv = null;

    /*
     * Because I don't fully understand how items are currently set up and the video in part
     * goes over an implementation for items to link to the crafting implementation, this is
     * an implementation that made sense to me. Recipes are stored in a list of arrays which
     * includes which items have to be present and what it creates.
     */
    private List<ItemData[]> recipes = new List<ItemData[]>();

    private void Start()
    {
        
    }


    void Update()
    {
        craftInv.GetSlotByIndex(5);
    }
}
