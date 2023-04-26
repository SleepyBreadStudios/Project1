using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{

    // Should replace this with inventory items later once we have it working
    private Item currentItem;
    //public Image customCursor; 

    public void OnMouseDownItem(Item item) {
        if (currentItem == null) {
            currentItem = item;
            //customCursor.gameObject.SetActive(true);
            //customeCursor.sprite = currentItem.GetComponent<Image>().sprite;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
