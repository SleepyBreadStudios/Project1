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
    private Gradient gradient;

    public float currentHealth;
    public float maxHealth;

    [SerializeField]
    public Image healthImage;

    [SerializeField]
    public bool active = false;

    float lerpSpeed;

    // Sets the values of the health
    public void SetHealth(float max) 
    {
        currentHealth = max;
        maxHealth = max;
    }

    // Updates the health bar
    public void UpdateHealth(float health) 
    {
        //slider.value = currentHealth / maxHealth;
        currentHealth = health;
        //GameObject.SetActive(true);
    }

    public void HealthColor() 
    {
        Color HealthColor = Color.Lerp(Color.red, Color.green, (currentHealth / maxHealth));

        healthImage.color = HealthColor;
    }

    public void Heal(float value) 
    {

    }

    private void Activate() 
    {
        if (currentHealth == maxHealth) {
            healthImage.enabled = false;
        } else if (currentHealth < maxHealth || active) {
            healthImage.enabled = true;
        }
    }

    private void Update() 
    {
        Activate();
        lerpSpeed = 3f * Time.deltaTime;
        healthImage.fillAmount = Mathf.Lerp(healthImage.fillAmount, currentHealth / maxHealth, lerpSpeed);
        HealthColor();
    }

}
