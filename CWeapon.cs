using UnityEngine;

/***************************************
* Filename:            CWeapon.cs
* Date:                08/18/2021
* Mod. Date:           11/08/2021
* Mod. Initials:       MRHA
* Author:              Matthew Hoekstra-Abel
* Purpose:             This script can be applied to an item to give it damage
***************************************/
public class CWeapon : MonoBehaviour
{
    /***************************************
    * m_fBaseDamage:            This is the base damage of the weapon. 
    * m_tDamageMultiplier:      This is the multiplicative increase of the base damage
    * m_cHitbox:                A reference to the hitbox of the weapon
    * m_cTrail:                 A reference to the trail renxderer
    * m_fKnockbackMultiplier    Multiplies knockback vector by this variable
    **************************************/
    public float m_fBaseDamage;
    private float m_fDamageMultiplier = 1;
    private Collider m_cHitbox;
    private TrailRenderer m_cTrail;
    public float m_fKnockbackMuliplier = 1;

    private void Start()
    {
        m_cHitbox = gameObject.GetComponent<Collider>();
        m_cTrail = gameObject.GetComponentInChildren<TrailRenderer>();
        ToggleWeaponHitbox(false);
    }

    /*******************************************
     * OnTriggerEnter() Calculates damage on objects that enter this weapon's collider
     *         
     * Mod date:        11/08/2021
     * Mod initials:    MRHA
    *******************************************/
    private void OnTriggerEnter(Collider cCollider)
    {
        // Debug.Log(cCollider.name + " was hit by " + this.name);
        if (cCollider.gameObject.layer != gameObject.layer)
            if (cCollider.gameObject.GetComponent<CHealth>())
            {
                Vector3 tPushVector = cCollider.transform.position - transform.parent.position;
                if (cCollider.gameObject.GetComponent<CController>())
                {
                    cCollider.gameObject.GetComponent<CController>().IncreaseKnockbackVector(tPushVector.normalized/* * m_fBaseDamage*/ * m_fKnockbackMuliplier * m_fDamageMultiplier);
                }

                    cCollider.gameObject.GetComponent<CHealth>().TakeDamage(m_fBaseDamage * m_fDamageMultiplier);
                    // Debug.Log(this.gameObject.name + " has struck " + cCollider.gameObject.name);
            }
    }
    /*******************************************
     * ToggleWeaponHitbox()        Toggle Weapon's Collider's enabled variable
     * 
     * Ins:
     *   bState         The desired state of the weapon
     *         
     * Mod date:        08/19/2021
     * Mod initials:    MRHA
    *******************************************/
    public void ToggleWeaponHitbox(bool bState)
    {
        m_cHitbox.enabled = bState;
        if (m_cTrail)
            m_cTrail.emitting = bState;
    }
}
