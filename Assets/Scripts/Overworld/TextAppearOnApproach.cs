/******************************************************************************
 * Text popup when players approach object
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextAppearOnApproach : MonoBehaviour
{
    public GameObject textToAppear;
    public float fadeDuration = 2.0f; // Time it takes for the text to fully appear

    private bool playerInRange = false;

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         playerInRange = true;
    //         StartCoroutine(FadeInText());
    //     }
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         playerInRange = false;
    //         StartCoroutine(FadeOutText());
    //     }
    // }

    // private IEnumerator FadeInText()
    // {
    //     float elapsedTime = 0;
    //     Color initialColor = textToAppear.color;
    //     Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1.0f); // Fully opaque color

    //     while (elapsedTime < fadeDuration)
    //     {
    //         elapsedTime += Time.deltaTime;
    //         textToAppear.color = Color.Lerp(initialColor, targetColor, elapsedTime / fadeDuration);
    //         yield return null;
    //     }

    //     textToAppear.color = targetColor; // Ensure the text becomes fully visible
    // }

    // private IEnumerator FadeOutText()
    // {
    //     float elapsedTime = 0;
    //     Color initialColor = textToAppear.color;
    //     Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0.0f); // Fully transparent color

    //     while (elapsedTime < fadeDuration)
    //     {
    //         elapsedTime += Time.deltaTime;
    //         textToAppear.color = Color.Lerp(initialColor, targetColor, elapsedTime / fadeDuration);
    //         yield return null;
    //     }

    //     textToAppear.color = targetColor; // Ensure the text becomes fully transparent
    // }

}
