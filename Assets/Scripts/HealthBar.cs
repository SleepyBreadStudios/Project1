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

    [SerializeField]
    public Text healthPoints;

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

    public void Heal() 
    {
        if (currentHealth < maxHealth) 
        {
            currentHealth++;
        }
    }

    public void Damage()
    {
        if(currentHealth > 0)
        {
            currentHealth--;
        }
    }

    // Activates the health bar when the current health is less than max (for enemies)
    private void Activate() 
    {
        if (currentHealth == maxHealth && !active) {
            healthImage.enabled = false;
        } else {
            healthImage.enabled = true;
        }
    }

    private void Update() 
    {
        Activate();
        //Heal();
        // Testing health decrementing
        //if (Input.GetKeyDown(KeyCode.Space)) {
            //currentHealth--;
        //}
        // Testing healing
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (currentHealth < maxHealth)
            {
                currentHealth++;
            }
        }

        healthPoints.text = currentHealth + " / " + maxHealth;
        lerpSpeed = 3f * Time.deltaTime;
        healthImage.fillAmount = Mathf.Lerp(healthImage.fillAmount, currentHealth / maxHealth, lerpSpeed);
        HealthColor();
    }

}
