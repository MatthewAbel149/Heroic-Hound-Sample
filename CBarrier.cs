using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*******************************************
 * Filename:            CBarrier.cs
 * Date:                10/01/2021
 * Mod. Date:           10/19/2021
 * Mod. Initials:       MRHA
 * Author:              Matthew Hoekstra-Abel
 * Purpose:             Walls that block the player from moving. Opened via keys or kill goals
 ******************************************/
public class CBarrier : MonoBehaviour
{
    /*********************************************
     * m_cBarrier           The wall collider
     * m_cPlayerInventory   The player's inventory
     * m_cParticleSystem    The particle effect for the wall
     * m_szKeyID            The key ID required to unlock the barrier
     * m_bKeyBarrier        The barrier requires a key to open
     ********************************************/
    private Collider m_cBarrier;
    private CPlayerInventory m_cPlayerInventory;
    private ParticleSystem m_cParticleSystem;
    private CControllerManager m_cControllerManager;
    public bool m_bKeyBarrier = false;
    public string m_nKeyID = "";
    private SpriteRenderer m_cSprite;
    // If =0, the barrier will be inert.
    // if <0, player must have an item with a matching id.

    private void Start()
    {
        //m_cBarrier = GameObject.GetComponentInChildren<BoxCollider>();
        m_cPlayerInventory = CPlayerInventory.instance;
        m_cBarrier = GetComponentsInChildren<Collider>()[1]; // GetComponentInChildren return's parent collider. Access using this line.
        m_cParticleSystem = GetComponentInChildren<ParticleSystem>();
        m_cControllerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CControllerManager>();

        CControllerManager.OnEnemySlain += CheckEnemyCount;
        CPortal.OnLevelComplete += Reset;

        SpriteRenderer spriteVar = GetComponentInChildren<SpriteRenderer>();
        if (spriteVar)
        {
            m_cSprite = spriteVar;
        }

        SetLocked(false);
    }
    /*********************************************
     * SetLocked()      Locks or unlocks the barrier based on the boolean value
     * 
     * Mod date:        10/19/2021
     * Mod initials:    MRHA
     ********************************************/
    public void SetLocked(bool _value)
    {
        m_cBarrier.enabled = _value;
        ParticleSystem.EmissionModule m_cEmitter = m_cParticleSystem.emission;
        m_cEmitter.enabled = _value;

        if (m_cSprite)
            m_cSprite.enabled = _value;
    }
    /*********************************************
     * CheckEnemyCount()  Listens to the game manager. Opens non-key barriers if all enemies are dead
     * 
     * Mod date:        10/07/2021
     * Mod initials:    MRHA
     ********************************************/
    private void CheckEnemyCount()
    {
        if (m_cBarrier.enabled == false)
            return;

        if (!m_bKeyBarrier && m_cControllerManager.m_sEnemyCount == 0)
            SetLocked(false);
    }
    /*********************************************
     * CheckInventory() Looks through the player's inventory for a key
     * 
     * Mod date:        10/19/2021
     * Mod initials:    MRHA
     ********************************************/
    private bool CheckInventory(string _id)
    {
        bool bItemFound = m_cPlayerInventory.FindItem(_id);
        if (!bItemFound)
        {
            return false;
        }
        else
        {
            SetLocked(false);
            return true;
        }
    }
    /*********************************************
     * CheckInventoryAndRemove() Looks through the player's inventory for a key and deletes it
     * 
     * Mod date:        10/19/2021
     * Mod initials:    MRHA
    *********************************************/
    private bool CheckInventoryAndRemove(string _id)
    {
        bool bItemRemoved = m_cPlayerInventory.FindAndRemove(_id);
        if (!bItemRemoved)
        {
            return false;
        }
        else
        {
            SetLocked(false);
            return true;
        }
    }
    /*********************************************
     * Reset()          Resets barrier states. Key gates should reactivate
     * 
     * Mod date:        10/07/2021
     * Mod initials:    MRHA
     ********************************************/
    private void Reset()
    {
        if (m_bKeyBarrier)
        {
            SetLocked(true);
        }
    }
    /*********************************************
     * SetObjective()   Sets objective to the given value
     * 
     * Mod date:        10/04/2021
     * Mod initials:    MRHA
     ********************************************/
    public void SetObjective(string nValue)
    {
        m_nKeyID = nValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && m_cBarrier.enabled)
        {
            if (CheckInventory(m_nKeyID))
                return;
            if (m_nKeyID == "" && m_cControllerManager.m_sEnemyCount == 0)
                SetLocked(false);
        }
    }
    void OnDestroy()
    {
        CControllerManager.OnEnemySlain -= CheckEnemyCount;
        CPortal.OnLevelComplete -= Reset;
    }
}
