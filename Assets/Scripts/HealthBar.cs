/******************************************************************************
 * Health bar script, shared amongst player and enemy objects
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Gradient gradient;

    public float currentHealth;
    public float maxHealth;

    // Sets the values of the health
    public void SetHealth(float max) {
        currentHealth = max;
        maxHealth = max;
    }

    // Updates the health bar
    public void UpdateHealth(float currentHealth, float maxHealth) {
        slider.value = currentHealth / maxHealth;
        currentHealth = currentHealth;
        //GameObject.SetActive(true);
    }

    // private IEnumerator DrainHealthBar()
    // {
    //     float elapsedTime = 0f;
    //     while (elapsedTime < 0.25f)
    //     {
    //         elapsedTime = Time.deltaTime;
    //         slider.value = Mathf.Lerp(slider.value)
    //     }
    // }
}
