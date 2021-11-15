using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*******************************************
 * Filename:            CController.cs
 * Date:                09/03/2021
 * Mod. Date:           11/01/2021
 * Mod. Initials:       MRHA
 * Author:              Matthew Hoekstra-Abel
 * Purpose:             This is a parent class for Controller classes
 ******************************************/

abstract public class CController : MonoBehaviour
{
    /*********************************************
     * m_bIsAlive               Boolean used for various checks
     * m_fKnockbackResistance   A value from 0-1 that will lower incoming knockback
     * m_tKnockbackVector       The knockback vector. Stores direction and magnitude of push.
     * m_fKnockbackTime         The muliplier applied to the knockback vector's interpolation
     ********************************************/
    public bool m_bIsAlive = true;
    public float m_fKnockbackMultiplier = 1.0f;
    private Vector3 m_tKnockbackVector = Vector3.zero;
    private float m_fKnockbackTime = 10;
    
    public abstract void Die();
    public abstract void Respawn();
    /********************************
     * IncreaseKnockbackVector      Adds to the knockback vector
     * 
     * Mod date:                    11/02/2021
     * Mod initials:                MRHA
    ********************************/
    public void IncreaseKnockbackVector(Vector3 _tKnockbackVector)
    {
        m_tKnockbackVector.x += _tKnockbackVector.x * m_fKnockbackMultiplier;
        m_tKnockbackVector.z += _tKnockbackVector.z * m_fKnockbackMultiplier;
    }
    /********************************
     * Knockback            Pushes the player along the knockback vector
     * 
     * Mod date:            11/01/2021
     * Mod initials:        MRHA
    ********************************/
    public void Knockback()
    {
        if(m_tKnockbackVector.x < 0.01f && m_tKnockbackVector.x > -0.01f)
            m_tKnockbackVector.x = 0f;
        if(m_tKnockbackVector.z < 0.01f && m_tKnockbackVector.z > -0.01f)
            m_tKnockbackVector.z = 0f;
        gameObject.transform.position += m_tKnockbackVector * m_fKnockbackTime * Time.deltaTime;
        m_tKnockbackVector = Vector3.Lerp(m_tKnockbackVector, Vector3.zero, m_fKnockbackTime * Time.deltaTime);
        
    }
}
