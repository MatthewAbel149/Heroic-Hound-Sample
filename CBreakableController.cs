using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*******************************************
 * Filename:            CController.cs
 * Date:                10/14/2021
 * Mod. Date:           11/01/2021
 * Mod. Initials:       ARV
 * Author:              Matthew Hoekstra-Abel
 * Purpose:             A controller for breakable objects
 ******************************************/
public class CBreakableController : CController
{
    /*********************************************
     * m_cLootDrop              The gameobject this breakable should drop
     * m_fDropProbability       The percentage chance an item will drop
     * g_breakingsound          The sound that the breakable makes when they are destroyed
     ********************************************/
    public GameObject m_cLootDrop;
    public float m_fDropProbability;
    
    /*******************************************
     * Die()            Drops an item and turns the object off
     *                  
     * Mod date:        10/14/2021
     * Mod initials:    MRHA
    *******************************************/
    public override void Die()
    {
        CAudioManager.cAudioManagerInstance.Play("BreakablesDestroyed");
        Debug.Log("Sound played");
        gameObject.SetActive(false);

        float fRandReturn = Random.Range(0,100);
        if (fRandReturn < m_fDropProbability)
        {
            Instantiate(m_cLootDrop, transform.position, Quaternion.identity);
        }
    }
    /*******************************************
     * Respawn()        Reenables the breakable
    *******************************************/
    public override void Respawn()
    {
        gameObject.SetActive(true);
    }
}
