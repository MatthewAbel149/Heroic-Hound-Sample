using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/***************************************
* Filename:            CWeapon.cs
* Date:                08/18/2021
* Mod. Date:           09/03/2021
* Mod. Initials:       MRHA
* Author:              Matthew Hoekstra-Abel
* Purpose:             This script can be applied to a gameobject
*                       WITH A RIGIDBODY to kill it.
***************************************/
public class CHealth : MonoBehaviour
{
    /***************************************
    * m_fMaximumHealth:     The maximum health an object can have
    * m_fCurrentHealth:     The current health an object has
    * g_HealthSlider:       The slider object that will hold the reference for the health bar
    * m_bGodmode:           Skips damage calculation when enabled
    **************************************/
    public float m_fMaximumHealth = 100;
    public float m_fCurrentHealth;
    public Slider g_HealthSlider;
    private bool m_bGodmode = false;

    /*******************************************
     * Start()          Initialize member variables
     * 
     * Mod date:        08/18/2021
     * Mod initials:    MRHA
    *******************************************/
    private void Start()
    {
        FillHealth();
        UpdateHealthBar();
    }
    /*******************************************
     * FillHealth:      Set current health to maximum
     * 
     * Mod date:        08/26/2021
     * Mod initials:    MRHA
    *******************************************/
    public void FillHealth()
    {
        m_fCurrentHealth = m_fMaximumHealth;
        UpdateHealthBar();
    }
    /*******************************************
     * TakeDamage:      Lowers current health by fDamage
     * 
     * Ins:
     *  fDamage:        The amount of damage taken
     * 
     * Mod date:        08/19/2021
     * Mod initials:    ARV
    *******************************************/
    public void TakeDamage(float fDamage)
    {
        if (m_bGodmode)
            return;

        m_fCurrentHealth -= fDamage;

        if (m_fCurrentHealth <= 0)
        {
            m_fCurrentHealth = 0;
            GetComponent<CController>().Die();
        }
        UpdateHealthBar();
    }
    /*******************************************
     * GainHealth:      Increases current health by fHealth
     * 
     * Ins:
     *  fHealth:        The amount of health gained
     * 
     * Mod date:        08/19/2021
     * Mod initials:    ARV
    *******************************************/
    public void GainHealth(float fHealth)
    {
        m_fCurrentHealth += fHealth;
        if (m_fCurrentHealth >= m_fMaximumHealth)
            m_fCurrentHealth = m_fMaximumHealth;

        UpdateHealthBar();
    }
    /*******************************************
     * Regenerate:      Increases current health by fHealth over fTime
     * 
     * Ins:
     *  fHealth:        The amount of health gained
     *  fHealth:        The amount of time it takes to heal
     * 
     * Mod date:        08/18/2021
     * Mod initials:    MRHA
    *******************************************/
    public void Regenerate(float fHealth, float fTime)
    {
        //TODO: Implement regeneration coroutine
    }
    /*******************************************
     * UpdateHealthBar:      Sets the healthbar slider to current health
     * 
     * Mod date:        10/14/2021
     * Mod initials:    MRHA
    *******************************************/
    public void UpdateHealthBar()
    {
        if (g_HealthSlider)
            g_HealthSlider.value = m_fCurrentHealth;
    }
    /*******************************************
     * ToggleDamage:    Activate/Deactivate damage calculation
     * 
     * Mod date:        08/20/2021
     * Mod initials:    MRHA
    *******************************************/
    public void ToggleDamage()
    {
        m_bGodmode = !m_bGodmode;
    }
}
