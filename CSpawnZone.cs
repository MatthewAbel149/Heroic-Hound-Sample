using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*******************************************
 * Filename:            CSpawnZone.cs
 * Date:                I forgot when I made this
 * Mod. Date:           10/07/2021
 * Mod. Initials:       MRHA
 * Author:              Matthew Hoekstra-Abel
 * Purpose:             Interfaces with the Controller Manager to spawn an enemy
 ******************************************/
public class CSpawnZone : MonoBehaviour
{
    /*************************************
     * m_cEnemyPool         The enemy pool held by the game manager
     * m_tSpawnLocation     The positions of all enemies that will spawn in
     * m_cBarriers          A list of all barriers surrounding the spawner
     * m_bConsumed          Has the spawner been used
     * m_bOneTimeOnly       Should the spawner only trigger once
    *************************************/
    private CControllerManager m_cEnemyPool;
    public List<Vector3> m_tSpawnLocations;
    public List<CBarrier> m_cBarriers;
    private bool m_bConsumed;
    public bool m_bOneTimeOnly = false;

    private void Start()
    {
        m_cEnemyPool = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CControllerManager>();
        CPortal.OnLevelComplete += Reset;
    }
    /***************************************
     * Reset            Allows a spawner to activate again
     * 
     * Mod Date:        10/07/20
     * Mod Initials:    MRHA
    ***************************************/
    public void Reset()
    {
        if (!m_bOneTimeOnly)
        {
            m_bConsumed = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!m_bConsumed && other.tag == "Player")
        {
            m_bConsumed = true;
            for (int i = 0; i < m_tSpawnLocations.Count; ++i)
            {
                m_cEnemyPool.SpawnEnemy(m_tSpawnLocations[i]);
            }
            for (int i = 0; i < m_cBarriers.Count; ++i)
            {
                m_cBarriers[i].SetLocked(true);
            }
        }
    }
    private void OnDestroy()
    {
        CPortal.OnLevelComplete -= Reset;
    }
}