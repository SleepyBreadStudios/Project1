using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RecipeUIBehavior : MonoBehaviour
{
    [SerializeField] private Text recipeText = null;

    [SerializeField] private Text titleText = null;

    public void DisplayRecipe(List<string> recipeArray)
    {
        //Debug.Log("Is this called?");
        StringBuilder builder = new StringBuilder();
        //if (text == null)
        //{
        //    Debug.Log("Text null?");
        //}
        titleText.text = recipeArray[0];
        for (int i = 1; i < recipeArray.Count; i++)
		{
            builder.Append(recipeArray[i]).Append("\n");
        }

        recipeText.text = builder.ToString();
    }
}
